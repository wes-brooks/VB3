using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using VBCommon;


namespace VBProjectManager
{
    /// <summary>
    /// Class is to be used as a singleton. The controller will instantiate by calling the
    /// getLogger method. An Event is defined to fire when the messageIntent flag is not
    /// LogFileOnly, i.e., when the UI wants to see the message. 
    /// Logging will open/close the log file, so that cleanup is not required.
    /// </summary>
    public sealed class VBLogger
    {
        private static volatile VBLogger me = null;
        private static object syncRoot = new Object();
        private String logFileName = String.Empty;
        private const int i_max = 100;

        public delegate void MessageLoggedEventHandler(String message, Globals.targetSStrip target);
        public event MessageLoggedEventHandler MessageEventLogged;


        private bool lastsession;

        public bool LastSessionClosed
        {
            get
            {
                return lastsession;
            }
            set
            {
                lastsession = value;
            }
        }

        /// <summary>
        /// Will return instance, or instantiate if instance is null. Will trigger
        /// the first log entry if file name has been set since program launch and
        /// instantiation was needed.
        /// </summary>
        /// <returns></returns>
        public static VBLogger GetLogger()
        {
            if (me == null)
            {
                lock (syncRoot)
                {
                    me = new VBLogger();
                    if (!me.logFileName.Equals(String.Empty))
                    {
                        LogFirstTime();
                    }
                    
                }
            }
            return me;
        }

        private VBLogger()
        {
            lastsession = true;
        }

        /// <summary>
        /// Destructor makes final log entry to signal normal program termination.
        /// </summary>
        //       ~VBLogger() {
        //if (!logFileName.Equals(String.Empty)) {
        //    logClose();
        //}
        //        }

        public static void CloseLog()
        {
            if (me != null)
            {
                lock (syncRoot)
                {
                    if (me != null)
                    {
                        me.LogEvent("Log closed normally.");
                        if (me.MessageEventLogged != null)
                        {
                            foreach (Delegate del in me.MessageEventLogged.GetInvocationList())
                            {
                                me.MessageEventLogged -= (MessageLoggedEventHandler)del;
                            }
                        }
                        me.logFileName = String.Empty;
                        me = null;
                    }
                }
            }
        }


        /// <summary>
        /// Sets the static log file path. Triggers first log entry if instance is made.
        /// </summary>
        /// <param name="filePath">Full path to text file. File doesn't need to exist.</param>
        public static void SetLogFileName(String filePath)
        {
            if (filePath.Equals(String.Empty))
            {
                if (me != null)
                {
                    CloseLog();
                }
            }
            else
            {
                me.logFileName = filePath;
                if (me != null)
                {
                    LogFirstTime();
                }
            }
        }


        /// <summary>
        /// Add handler to event that fires when user is to see log entry.
        /// </summary>
        /// <param name="handler">see delegate this class</param>
        public void AddHandler(MessageLoggedEventHandler handler)
        {
            MessageEventLogged += handler;
        }


        internal static void LogFirstTime()
        {
            try
            {
                if (File.Exists(me.logFileName))
                {
                    using (FileStream fs = File.Open(me.logFileName, FileMode.Open))
                    {
                        TextReader reader = new StreamReader(fs);
                        String ln = null;
                        String prevLn = null;
                        while ((ln = reader.ReadLine()) != null)
                        {
                            prevLn = ln;
                        }
                        if (prevLn != null)
                        {
                            if (!prevLn.Contains("Log closed normally"))
                            {
                                me.LastSessionClosed = false;
                            }
                        }
                        fs.Close();
                    }
                    me.LogEvent("Log opened.");
                }
                else
                {
                    InitLogFile(me.logFileName);
                }
            }
            catch (IOException ioe)
            {
                throw new ApplicationException("Cannot use log file. " + ioe.Message);
            }
            catch (Exception other)
            {
                throw new ApplicationException("Cannot use log file. " + other.Message);
            }
        }


        static void InitLogFile(string logFileName)
        {
            string logFileDir = logFileName.Substring(0, logFileName.LastIndexOf(Path.DirectorySeparatorChar));
            if (!Directory.Exists(logFileDir))
                Directory.CreateDirectory(logFileDir);

            FileStream fs = File.Create(logFileName);
            if (fs.CanWrite)
            {
                TextWriter writer = new StreamWriter(fs);
                writer.WriteLine("Log file created on " + DateTime.Today.ToLongDateString());
                writer.Flush();
                writer.Close();
            }
            fs.Close();
            me.LastSessionClosed = true;
        }

        /// <summary>
        /// Returns the field.
        /// </summary>
        /// <returns></returns>
        public static String GetLogFileName()
        {
            return me.logFileName;
        }


        /// <summary>
        /// Append the message to the log file.
        /// On errors, repeated attempts (file busy/open).
        /// </summary>
        /// <param name="message"></param>
        private void LogEvent(String message, int iteration)
        {
            TextWriter writer = null;
            if (!logFileName.Equals(String.Empty) && iteration < i_max)
            {
                try
                {
                    //create the log file if necessary
                    if (!File.Exists(logFileName))
                        InitLogFile(logFileName);

                    //open the file and append to it.
                    using (writer = File.AppendText(logFileName))
                    {
                        writer.WriteLine(DateTime.Now.ToShortDateString() + " " +
                        DateTime.Now.ToLongTimeString() + " - " + message);
                        if (iteration > 0)
                        {
                            writer.WriteLine("file open attemps: " + iteration);
                        }
                        writer.Flush();
                    }
                }
                catch (IOException)
                {
                    if (writer != null) writer.Close();
                    //this is if an IO error has occurred where the file is already open
                    //by another process.  Try to write again until success.
                    System.Threading.Thread.Sleep(200);
                    LogEvent(message, iteration + 1);
                }
                catch (Exception)
                {
                    // not fatal?
                    if (writer != null) writer.Close();
                }
            }
        }

        /// <summary>
        /// If there are any registered handlers, call them now with message.
        /// </summary>
        /// <param name="message"></param>
        private void OnMessageEventLogged(String message, Globals.targetSStrip target)
        {
            if (MessageEventLogged != null)
            {
                MessageEventLogged(message, target);
            }
        }

        /// <summary>
        /// Here is the main API. Log message as indicated. Method will 
        /// prepend date and time and append NewLine to message.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="intent"></param>
        public void LogEvent(String message, Globals.messageIntent intent, Globals.targetSStrip target)        
        {
            if (intent == Globals.messageIntent.LogFileOnly)
            {
                LogEvent(message, 0); //call the method that will 
            }
            else if (intent == Globals.messageIntent.UserAndLogFile)
            {
                LogEvent(message, 0);
                OnMessageEventLogged(message, target);
            }
            else if (intent == Globals.messageIntent.UserOnly)
            {
                OnMessageEventLogged(message, target);
            }
        }

        /// <summary>
        /// Overloaded method to send the message only to the log file.
        /// </summary>
        /// <param name="message"></param>
        public void LogEvent(string message)
        {
            this.LogEvent(message, Globals.messageIntent.LogFileOnly, Globals.targetSStrip.None);
        }
    }
}

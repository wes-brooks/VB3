using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Xml;
using System.Xml.XPath;
using System.IO;

namespace VBTools
{
    public class VBCSHelp
    {

        private bool _status = true;
        private string _apppath = string.Empty;
        private string _helpfile = @"\Documentation\Virtual_Beach_2_User_Guide.pdf";
        private string _helpinfofile = @"\Documentation\CSHelpPages.xml";
        private XmlDocument _helpinfo = null;

        private static Process _proc = null;

        //smarter way to do this is to read pages info from a file
        //private enum pn { Menu = 1, Intro = 5, Location = 12, 
        //    DataProc = 17, Modeling = 34, Residuals = 49, Prediction = 52 };

        public VBCSHelp(string appPath, object context)
        {
            _apppath = appPath;
            //open and read the page info file
            _helpinfo = getPageNumbers();
            //if I find and read the page info file without issue...
            if (_status)
            {
                //translate context into a page number and launch the reader
                string topic = getPagefromContext(context);
                launchHelpReader(topic);
            }
   
        }

        private XmlDocument getPageNumbers()
        {
            //throw new NotImplementedException();
            string filepath = _apppath + _helpinfofile;
            XmlDocument xmldoc = new XmlDocument();
            try { xmldoc.Load(filepath); }
            catch (XmlException ex)
            {
                Console.WriteLine("Can't read CSHelpInfo file: " + filepath.ToString() + " **xmlException:" + ex.Message.ToString());
                _status = false;
                return null;
                //throw new ApplicationException("Problem reading the project file, file name:" + filepath.ToString());
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("Can't find CSHelpInfo file: " + filepath.ToString());
                _status = false;
                return null;
                //throw new ApplicationException("Can't find project file: " + filepath.ToString());
            }
            catch (DirectoryNotFoundException)
            {
                Console.WriteLine("Can't find CSHelpInfo file path: " + filepath.ToString());
                _status = false;
                return null;
                //throw new ApplicationException("Can't find project file path: " + filepath.ToString());
               
            }
            _status = true;
            return xmldoc;
        }

        private string getPagefromContext(object context)
        {
            //throw new NotImplementedException();

            //navigate the help info file to the help context node and return the page number
            //context is <namespace.formname>
            string [] sa = context.ToString().Split(',');
            string s = sa[0];
            string node = "/PageNumbers/" + s;
            string page = string.Empty;

            //get the page number of the topic...
            XPathNavigator nav = _helpinfo.CreateNavigator();
            try
            {
                XPathNodeIterator hits = nav.Select(node);
                if (hits.Count == 1 && hits.MoveNext() && hits.Current.NodeType == XPathNodeType.Element)
                {
                    page = "page=" + hits.Current.Value;
                }
            }
            //...or return with the 1st page if can't find topic
            catch (XPathException)
            {
                page = "page=1";
            }
            return page;
        }

        //private string getPagefromContext(object context)
        //{
        //    //throw new NotImplementedException();
        //    string page = "page=";
        //    string [] sa = context.ToString().Split(',');
        //    string s = sa[0];

        //    switch (s)
        //    {
        //        case "User Guide":
        //            page += ((int)pn.Menu).ToString();
        //            break;
        //        case "DockSample.MainForm":
        //            page += ((int)pn.Intro).ToString();
        //            break;
        //        case "VBLocation.frmLocation":
        //            page += ((int)pn.Location).ToString();
        //            break;
        //        case "VBDatasheet.frmDatasheet":
        //            page += ((int)pn.DataProc).ToString();
        //            break;
        //        case "GALibForm.frmModel":
        //            page += ((int)pn.Modeling).ToString();
        //            break;
        //        case "VBResiduals.frmResiduals":
        //            page += ((int)pn.Residuals).ToString();
        //            break;
        //        case "VBPrediction.frmPrediction":
        //            page += ((int)pn.Prediction).ToString();
        //            break;
        //        default:
        //            page += ((int)pn.Menu).ToString();
        //            break;
        //    }

        //    return page;
        //}

        private void launchHelpReader(string topic)
        {
            //this doesn't work too well... sometimes does not close and aborts if not running.
            //can fix abort issue by checking the process table but wtf for not closing??? timing?
            //if (_proc != null)
            //{
            //    _proc.Kill();
            //    _proc.Close();
            //    _proc = null;     
            //}
            //for now, users will just have to close adobe for consecutive CS help requests

            string pdfpath = _apppath + _helpfile;

            Process pAcrobat = new Process();
            pAcrobat.StartInfo.FileName = "acrobat.exe";
            pAcrobat.StartInfo.Arguments = @"/A " + "pagemode=bookmarks&" + topic + '"' + " " + '"' + pdfpath + '"';

            Process pAdobeReader = new Process();
            pAdobeReader.StartInfo.FileName = "acroRd32.exe";
            pAdobeReader.StartInfo.Arguments = @"/A " + "pagemode=bookmarks&" + topic + '"' + " " + '"' + pdfpath + '"';

            //try launching acrobat
            try
            {
                pAcrobat.Start();
                _proc = pAcrobat;
                _status = true;
            }
            catch (Exception aberr)
            {
                pAcrobat.Close();
                Console.WriteLine("Launch doc reader acrobat failed: " + aberr.Message.ToString());
                _status = false;

                //try launching adobe reader
                try
                {
                    pAdobeReader.Start();
                    _proc = pAdobeReader;
                    _status = true;
                }
                catch (Exception arerr)
                {
                    pAdobeReader.Close();
                    Console.WriteLine("Launch doc reader failed: " + arerr.Message.ToString());
                    _status = false;
                }
            }

        }

        public bool Status
        {
            set { _status = value; }
            get { return _status; }
        }

    }
}

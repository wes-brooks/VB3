using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using System.IO;
using System.Windows.Forms;
using System.Reflection;


namespace VBTools
{
    public class ProjectData
    {
        private static volatile ProjectData me = null;
        private static object syncRoot = new Object();

        public delegate void readPDEventHandler(out object data, object sender, string message);
        public event readPDEventHandler readPDEvent;
        public delegate void witePDEventHandler(object data, object sender, string message);
        public event witePDEventHandler writePDEvent;

        private static XPathNavigator _nav = null;
        public static string _projectfile = string.Empty;
        private static XmlDocument _xmldoc = null;
        private static VBProjectManager _projMgr;

        public string ProjectFile
        {
            set { _projectfile = value; }
            get { return _projectfile; }
        }

        //public static ProjectData getPD(XPathNavigator navigator)
        public static ProjectData getPD()
        {
            if (me == null)
            {
                lock (syncRoot)
                {
                    if (me == null)
                    {
                        //me = new ProjectData(navigator);
                        me = new ProjectData();
                    }
                }
            }
            return me;
        }

        public void addreadHandler(readPDEventHandler handler)
        {
            this.readPDEvent += handler;
        }

        public void addwriteHandler(witePDEventHandler handler)
        {
            this.writePDEvent += handler;
        }

        //public ProjectData(XPathNavigator navigator)
        public ProjectData()
        {
            if (_projectfile != string.Empty)
            {
                _xmldoc = new XmlDocument();
                try { _xmldoc.Load(_projectfile); }
                catch (XmlException)
                {
                    throw new ApplicationException("Problem reading the project file, file name:" + _projectfile.ToString());
                }
                catch (FileNotFoundException)
                {
                    throw new ApplicationException("Can't find project file: " + _projectfile.ToString() +
                        " -- probably need to delete recentProjects.xml in the \bin directory");
                }
                catch (DirectoryNotFoundException)
                {
                    throw new ApplicationException("Can't find project file: " + _projectfile.ToString() +
                        " -- probably need to delete recentProjects.xml in the \bin directory");
                }
                _nav = _xmldoc.CreateNavigator();
                _projMgr = VBProjectManager.GetProjectManager();
            }
        }

        private void OnreadPDEvent(out object data, object sender, string message)
        {
            data = null;
            if (readPDEvent != null)
            {
                readPD(out data, sender, message);
            }
        }

        private void ONwritePDEvent(object data, object sender, string message)
        {
            if (writePDEvent != null)
            {
                writePD(data, sender, message);
            }
        }

        public void readPD(out object data, object sender, string message)
        {
            string loc = string.Empty;

            if (message.Contains("raw"))
            {
                loc = XmlUtils.getRawDataFileName(_nav);
            }
            else if (message.Contains("modeled"))
            {
                loc = XmlUtils.getModeledDataFileName(_nav);
            }
            else if (message.Equals("get project name"))
            {
                loc = XmlUtils.getProjectname(_nav);
            }
            else if (message.Equals("get datasheet data"))
            {
                loc = XmlUtils.getDataSheetDataFileName(_nav);
            }
            else if (message.Equals("get prediction season data"))
            {
                loc = XmlUtils.getPredictionTabDataFileName(_nav);
            }

            data = (object)loc;
        }

        public void writePD(object data, object sender, string message)
        {
            if (message.Contains("raw"))
            {
                XmlUtils.setRawDataFileName(_nav, (string)data);
            }
            else if (message.Contains("modeled"))
            {
                XmlUtils.setModeledDataFileName(_nav, (string)data);
            }
            else if (message.Equals("save project name"))
            {
                XmlUtils.setProjectname(_nav, (string)data);
            }
            else if (message.Equals("save datasheet info"))
            {
                XmlUtils.setDataSheetDataFileName(_nav, (string)data);
            }
            else if (message.Equals("save prediction season info"))
            {
                XmlUtils.setPredictionTabDataFileName(_nav, (string)data);
            }
            _xmldoc.Save(_projectfile);
        }

        public void open()
        {
            getSiteData();
        }

        public void Save()
        {
            SaveAs(_projectfile);
        }

        public void SaveAs(string projectFile)
        {
            //ProjectData pd = new ProjectData();

            setSiteData();
            _xmldoc.Save(projectFile);
        }


        public void setSiteData()
        {
            XmlUtils.setProjectname(_nav, _projMgr.SiteInfo.Project);
            XmlUtils.setBeachname(_nav, _projMgr.SiteInfo.Name);
            XmlUtils.setBeachorientation(_nav, _projMgr.SiteInfo.Orientation);
            XmlUtils.setSitelocation(_nav, _projMgr.SiteInfo.Location);
            XmlUtils.setMarker1(_nav, _projMgr.SiteInfo.LeftMarker);
            XmlUtils.setMarker2(_nav, _projMgr.SiteInfo.RightMarker);
            XmlUtils.setWM(_nav, _projMgr.SiteInfo.WaterMarker);
        }

        public void getSiteData()
        {
            _projMgr.SiteInfo.Project = XmlUtils.getProjectname(_nav);
            _projMgr.SiteInfo.Name = XmlUtils.getBeachname(_nav);
            _projMgr.SiteInfo.Orientation = XmlUtils.getBeachorientation(_nav);
            _projMgr.SiteInfo.Location = XmlUtils.getSitelocation(_nav);
            _projMgr.SiteInfo.LeftMarker = XmlUtils.getMarker1(_nav);
            _projMgr.SiteInfo.RightMarker = XmlUtils.getMarker2(_nav);
            _projMgr.SiteInfo.WaterMarker = XmlUtils.getWatermarker(_nav);
            
        }

        public void saveImportedDataFileName(string file)
        {
            XmlUtils.setRawDataFileName(_nav, file);
        }

        public string getRawDataFileName()
        {
             string file = XmlUtils.getRawDataFileName(_nav);
             return file;
        }
    

    }
}

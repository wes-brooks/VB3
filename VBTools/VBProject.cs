using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.IO;
using Serialization;

namespace VBTools
{



    /// <summary>
    /// This class contains references to all the objects necessary to save and open a project.
    /// It also accesses the Xmlserialization library which is used to persist project data.
    /// Currently a project consists of:
    ///     Site
    ///     
    /// </summary>
    /// 
    public class VBProject
    {
        private static VBProject _project;
        private string _projectName = "";
        private Globals.ProjectType _projectType;

        private Site _site = null;

        private DataTable _dtImportedData = null;
        private DataTable _dtCorrelationData = null;
        //private DataTable _dtPolynomialInfo = null;

        private MLRModelingInfo _modelingInfo = null;


        //This guy holds references to all the project objects we want to save.
        private object[] _graph;

        //events for when a project is opened
        public event ProjectOpenedHandler ProjectOpened;
        //public ProjectOpenedStatus ePD = null;
        public delegate void ProjectOpenedHandler(VBProject project);

        //events for when a project is saved
        public event ProjectSavedHandler ProjectSaved;
        //public ProjectOpenedStatus ePD = null;
        public delegate void ProjectSavedHandler(VBProject project);





        /// <summary>
        /// Only want one project in the system at a time.
        /// </summary>
        private VBProject()
        {
            _graph = new object[4];            
        }

        static internal VBProject GetProject()
        {
            if (_project == null)
                _project = new VBProject();

            return _project;
        }

        static internal VBProject NewProject()
        {            
           _project = new VBProject();
            return _project;
        }

        public string Name
        {
            get { return _projectName; }
            set { _projectName = value; }
        }

        public void Open(string projectFile)
        {
            VBProjectManager vbproj = VBProjectManager.GetProjectManager();

            XmlDeserializer deserializer = new XmlDeserializer();
            _graph = deserializer.Deserialize(projectFile) as object[];

            _projectType = (Globals.ProjectType)_graph[0];

            _site = _graph[1] as Site;


            DataSet ds = null;

            //_dtImportedData = null;
            //if (_graph[1] != null)
            //{
            //    string xmlImportedData = _graph[1] as string;
            //    if (!String.IsNullOrEmpty(xmlImportedData) || (xmlImportedData != ""))
            //    {
            //        _dtImportedData = new DataTable("ImportedData");
            //        ds = new DataSet();
            //        ds.ReadXml(new StringReader(xmlImportedData),XmlReadMode.ReadSchema);
            //        _dtImportedData = ds.Tables[0];
            //        ds = null;

            //        vbproj.ImportedData = _dtImportedData;
            //    }

            //}
           

            _dtCorrelationData = null;
            if (_graph[2] != null)
            {
                string xmlCorrelationData = _graph[2] as string;
                if (!String.IsNullOrEmpty(xmlCorrelationData) || (xmlCorrelationData != ""))
                {
                    _dtCorrelationData = new DataTable("CorrelationData");
                    ds = new DataSet();
                    ds.ReadXml(new StringReader(xmlCorrelationData), XmlReadMode.ReadSchema);                    
                    _dtCorrelationData = ds.Tables[0];
                    vbproj.CorrelationDataTable = _dtCorrelationData;
                }
            }


            if (_graph[3].GetType() == typeof(MLRModelingInfo))
                _modelingInfo = _graph[3] as MLRModelingInfo;
            else _modelingInfo = null;

            FileInfo fi = new FileInfo(projectFile);
            _projectName = fi.Name;
            ProjectOpened(this);

        }

        public void Save()
        {
            Save(_projectName, _projectType);
        }

        public void Save(string projectFile, Globals.ProjectType projectType)
        {

            List<object> lgraph = new List<object>();

            lgraph.Add(projectType);

            ProjectSaved(this);

            FileInfo fi = new FileInfo(projectFile);
            _projectName = fi.Name;

            //_graph[0] = _site;
            if (_site != null) 
                lgraph.Add(_site);
            

            StringWriter sw = null; 
           
            //Save ImportedData datatable
            //string xmlImportedData = "";
            //if (_dtImportedData != null)
            //{
            //    _dtImportedData.TableName = "ImportedData";
            //    sw = new StringWriter();
            //    _dtImportedData.WriteXml(sw, XmlWriteMode.WriteSchema, false);
            //    xmlImportedData = sw.ToString();
            //    sw.Close();
            //    sw = null;
            //}
            
            //_graph[1] = xmlImportedData;
            //if (xmlImportedData != null)
            //    lgraph.Add(xmlImportedData);

            
            //Save CorrelatedData datatable
            string xmlCorrelationData = "";
            if (_dtCorrelationData != null)
            {
                _dtCorrelationData.TableName = "CorrelationData";
                sw = new StringWriter();
                //_dtImportedData.WriteXml(sw, XmlWriteMode.IgnoreSchema, false); //wrong table (mog 3/14)
                _dtCorrelationData.WriteXml(sw, XmlWriteMode.WriteSchema, false);
                xmlCorrelationData = sw.ToString();
                sw.Close();
                sw = null;
            }
            
            //_graph[2] = xmlCorrelationData;
            if (xmlCorrelationData != null)
                lgraph.Add(xmlCorrelationData);


            //_graph[3] = _modelingInfo;
            if (_modelingInfo != null)            
                lgraph.Add(_modelingInfo);            
            else            
                lgraph.Add("");            

            XmlSerializer serializer = new XmlSerializer();
            serializer.Serialize(lgraph.ToArray(), projectFile);            

        }

        public DataTable ProjectImportedData
        {
            get { return _dtImportedData; }
            set { _dtImportedData = value; }
        }

        public DataTable ProjectCorrelationData
        {
            get { return _dtCorrelationData; }
            set { _dtCorrelationData = value; }
        }

        //public DataTable ProjectPolynomialData
        //{
        //    get { return _dtPolynomialInfo; }
        //    set { _dtPolynomialInfo = value; }
        //}

        public Site Site
        {
            get { return _site; }
            set { _site = value; }
        }

        public MLRModelingInfo ModelingInfo
        {
            get { return _modelingInfo; }
            set { _modelingInfo = value; }
        }

    }
}

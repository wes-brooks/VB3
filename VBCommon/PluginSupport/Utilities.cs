using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using DotSpatial.Controls;

namespace VBCommon.PluginSupport
{
    /*public class Utilities
    {
        private static AppManager _appMgr = null;

        public static AppManager AppMgr
        {
            get {return _appMgr;}
            set { _appMgr = value; }
        }


        public static T GetPluginByInterface<T>() where T : class
        {
            if (_appMgr == null)
                throw new Exception("AppManager has not been set.");

            if (!typeof(T).IsInterface)
                return null;

            foreach (DotSpatial.Extensions.IExtension ext in _appMgr.Extensions)
            {
                T plugin = ext as T;
                if (plugin != null)
                    return plugin;
            }

            return null;

        }
    }*/
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VBCommon;
using Microsoft.Scripting.Runtime;
using IronPython;


namespace IPyCommon
{
    //Declare the IPyInterface class static so that only one link is made into IronPython:
    public static class IPyInterface
    {
        //A link to the IronPython Engine (and the associated Interface object) we'll use for some modeling methods
        private static Microsoft.Scripting.Hosting.ScriptEngine engine;
        private static dynamic ipyInterface;

        static IPyInterface()
        {
            //Create the python execution engine
            engine = IronPython.Hosting.Python.CreateEngine();
            Microsoft.Scripting.Hosting.ScriptScope scriptScope = engine.CreateScope();

            //Link to the scripts that create the Beach_Interface object
            string strInitialize = "import Interface";
            string strGet_interface = "Interface.Interface";

            //Return the Beach_Interface object
            engine.Execute(strInitialize, scriptScope);
            ipyInterface = engine.Execute(strGet_interface, scriptScope);
        }


        //The following is a property to access the IronPython interface:
        public static dynamic Interface
        {
            get { return ipyInterface; }
        }
    }
}




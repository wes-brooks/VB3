using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VBCommon
{
    
	/// <summary>
	/// Summary description for ListItem.
	/// </summary>
	public class ListItem
	{
		private string itemDisplay;
		private string itemValue;

		public string DisplayItem
		{
			get {return itemDisplay;}
            set { itemDisplay = value; }
		}

		
		public string ValueItem
		{
			get	{return itemValue;	}
            set { itemValue = value; }
		}


        public ListItem()
        {
            itemDisplay = "";
            itemValue = "";
        }


		public ListItem(string newDisplay, string newValue)
		{
			itemDisplay = newDisplay;
			itemValue = newValue;
		}


		public override string ToString()
		{
			return itemDisplay;
		}
	}


    /// <summary>
	/// Summary description for ListItem.
	/// </summary>
    public class ListObject
    {
        private string itemDisplay;
        private object itemValue;

        public string DisplayItem
        {
            get { return itemDisplay; }
            set { itemDisplay = value; }
        }


        public object ValueItem
        {
            get { return itemValue; }
            set { itemValue = value; }
        }


        public ListObject()
        {
            itemDisplay = "";
            itemValue = null;
        }


        public ListObject(string newDisplay, object newValue)
        {
            itemDisplay = newDisplay;
            itemValue = newValue;
        }


        public override string ToString()
        {
            return itemDisplay;
        }
    }
}

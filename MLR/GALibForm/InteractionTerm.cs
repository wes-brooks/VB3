using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GALibForm
{
    public class Term
    {
        public string name;
        public int index;

    }
    public class InteractionTerm
    {
        private Term term1 = null;
        private Term term2 = null;


        public InteractionTerm(string name, int index)
        {
            term1 = new Term();
            term1.name = name;
            term1.index = index;
        }
        public InteractionTerm(string name1, int idx1, string name2, int idx2)
        {
            term1 = new Term();
            term1.name = name1;
            term1.index = idx1;

            term2 = new Term();
            term2.name = name2;
            term2.index = idx2;
        }

        public Term First
        {
            get { return term1; }
        }

        public Term Second
        {
            get { return term2; }
        }

        public string FullName
        {
            get 
            {
                if (term2 == null)
                    return term1.name;
                else
                    return term1.name + "_" + term2.name;
            }
        }


    }
}

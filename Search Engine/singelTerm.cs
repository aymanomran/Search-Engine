using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Search_Engine
{
    public class singelTerm
    {
        public string name;
        public List<int> doc_id;
        public List<int> frequency;
        public List<string> position;

        public singelTerm(string _name, List<int> _docs_id, List<int> _freq, List<string> _pos)
        {

            name = _name;
            doc_id = _docs_id;
            frequency = _freq;
            position = _pos;
        }

        public singelTerm()
        {

            doc_id = new List<int>();
            frequency = new List<int>();
            name = "";
            position = new List<string>();
        }


    }
}
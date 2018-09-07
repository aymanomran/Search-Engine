using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Search_Engine
{
    public class Term
    {
        public String name;
        public List<Details> details;
        public Term()
        {
            name = "";
            details = new List<Details>();
        }
    }
}
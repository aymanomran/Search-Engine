using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Search_Engine
{
    public class Details
    {
        public List<int> positions;
        public int doc_id;
        public int frequency;

        public Details()
        {
            positions = new List<int>();
            frequency = 0;
            doc_id = 0;
        }
    }

}
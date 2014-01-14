using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tallycalc
{
    public class TallyItem
    {
        public string name { get; set; }
        public int count { get; set; }
        public List<string> comments { get; set; }

        public TallyItem()
        {
            name = "";
            count = 0;
            comments = new List<string>();
        }

        public TallyItem(string a)
        {
            name = a;
            count = 0;
            comments = new List<string>();
        }
    }
}

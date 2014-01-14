using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tallycalc
{
    public class TallyGroup
    {
        public int total { get; set; }
        public string name { get; set; }
        public List<TallyItem> tallyItems { get; set; }

        public TallyGroup()
        {
            total = 0;
            name = "";
            tallyItems = new List<TallyItem>();
        }
        public TallyGroup(string a)
        {
            total = 0;
            name = a;
            tallyItems = new List<TallyItem>();
        }

        public TallyItem GetHighestCount()
        {
            TallyItem highest = this.tallyItems[0];
            foreach (TallyItem item in tallyItems)
            {
                if (item.count > highest.count)
                    highest = item;
            }
            return highest;
        }



    }
}

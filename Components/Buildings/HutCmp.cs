using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paleon
{
    public class HutCmp : BuildingCmp
    {
        public SettlerCmp Owner { get; set; }


        public HutCmp() : base("hut")
        {
            AddBuildingItem(ItemDatabase.STICK, 4);
        }

    }
}

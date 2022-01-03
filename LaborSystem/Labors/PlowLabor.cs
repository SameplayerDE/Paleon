using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paleon
{
    public class PlowLabor : Labor
    {

        public PlowLabor(FarmArea farmland) : base(LaborType.Farmer)
        {
            tasks.Add(new GotoPlowTask(farmland));
        }

        public PlowLabor(Tile tile) : base(LaborType.Farmer)
        {
            tasks.Add(new GotoPlowTask(tile));
        }

    }
}

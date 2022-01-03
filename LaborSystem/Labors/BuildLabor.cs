using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paleon
{
    public class BuildLabor : Labor
    {

        public BuildLabor(Tile targetTile, BuildingCmp building) : base(LaborType.Builder)
        {
            tasks.Add(new GotoBuildTask(targetTile, true, building));
        }

    }
}

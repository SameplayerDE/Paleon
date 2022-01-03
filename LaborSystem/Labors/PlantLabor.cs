using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paleon
{
    public class PlantLabor : Labor
    {

        public PlantLabor(FarmArea farmingArea) : base(LaborType.Farmer)
        {
            tasks.Add(new GotoTakeFromStockpile(farmingArea.Plant));
            tasks.Add(new GotoPlantTask(farmingArea));
        }

        public PlantLabor(Tile tile, Item seed) : base(LaborType.Farmer)
        {
            tasks.Add(new GotoTakeFromStockpile(seed));
            tasks.Add(new GotoPlantTask(tile));
        }

    }
}

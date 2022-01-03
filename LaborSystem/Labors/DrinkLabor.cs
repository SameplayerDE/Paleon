using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paleon
{
    public class DrinkLabor : Labor
    {

        public DrinkLabor(List<WaterSource> waterSources) : base(LaborType.NONE)
        {
            tasks.Add(new GotoDrinkTask(waterSources));
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paleon
{
    public class HuntLabor : Labor
    {

        public HuntLabor(AnimalCmp animal) :base(LaborType.Hunter)
        {
            tasks.Add(new GotoTakeToolFromStockpile(ItemDatabase.SHARP_FLINT));
            tasks.Add(new GotoHuntTask(animal));
            tasks.Add(new GotoFlayTask(animal));
        }


    }
}

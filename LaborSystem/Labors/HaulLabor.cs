using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Paleon
{

    public class HaulLabor : Labor
    {

        /// <summary>
        /// 1. Take item from target
        /// 2. Put item to stockpile
        /// </summary>
        /// <param name="takeFrom"></param>
        public HaulLabor(ItemContainer takeFrom, bool adjacent) : base(LaborType.Porter)
        {
            takeFrom.Labor = this;
            tasks.Add(new GotoTakeTask(takeFrom, adjacent));
            tasks.Add(new GotoPutToStockpileTask(takeFrom.Item));
        }

        /// <summary>
        /// 1. Take item from stockpile
        /// 2. Put item to target
        /// </summary>
        /// <param name="takeItem"></param>
        /// <param name="putTo"></param>
        public HaulLabor(Item takeItem, Tile putTo, BuildingCmp building) : base(LaborType.Porter)
        {
            tasks.Add(new GotoTakeFromStockpile(takeItem));
            tasks.Add(new GotoPutTask(putTo, true, building));
        }

        public override void Cancel()
        {
            base.Cancel();

            if (currentTask is GotoTakeTask)
                GameplayScene.WorldManager.LaborManager.Add(new HaulLabor((currentTask as GotoTakeTask).ItemCntr, false));
            else if(currentTask is GotoPutToStockpileTask)
                GameplayScene.WorldManager.LaborManager.Add(new HaulLabor((currentTask as GotoPutToStockpileTask).ItemCntr, false));
        }

    }
}

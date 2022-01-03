using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paleon
{
    public class MineLabor : Labor
    {
        private Tile tile;

        public MineLabor(Tile tile) : base(LaborType.Miner)
        {
            this.tile = tile;

            tile.SetMark(8);

            tasks.Add(new GotoTakeToolFromStockpile(ItemDatabase.FLINT_PICKAXE));

            var gotoMineTask = new GotoMineTask(tile);
            
            gotoMineTask.AddOnCompletedCallback(RemoveMarkOnTaskCompleted);

            tasks.Add(gotoMineTask);
        }

        private void RemoveMarkOnTaskCompleted(Task task)
        {
            tile.SetMark(0);
        }
    }
}

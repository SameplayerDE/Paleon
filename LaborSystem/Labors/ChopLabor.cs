using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paleon
{
    public class ChopLabor : Labor
    {

        private Tile tile;

        public ChopLabor(Tile tile) : base(LaborType.Woodcutter)
        {
            this.tile = tile;

            tile.SetMark(1);

            tasks.Add(new GotoTakeToolFromStockpile(ItemDatabase.FLINT_AXE));
            tasks.Add(new GotoChopTask(tile));

            AddOnLaborCompletedCallback(RemoveMarkOnLaborCompleted);            
        }

        private void RemoveMarkOnLaborCompleted(Labor labor)
        {
            tile.SetMark(0);
        }

    }
}

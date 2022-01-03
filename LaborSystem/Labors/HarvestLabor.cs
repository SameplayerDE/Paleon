using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paleon
{
    public class HarvestLabor : Labor
    {

        private Tile tile;

        public HarvestLabor(Tile tile) : base(LaborType.Farmer)
        {
            this.tile = tile;

            tile.SetMark(4);

            tasks.Add(new GotoHarvestTask(tile));

            AddOnLaborCompletedCallback(RemoveMarkOnLaborCompleted);
        }

        private void RemoveMarkOnLaborCompleted(Labor labor)
        {
            tile.SetMark(0);
        }

    }
}

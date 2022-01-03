using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paleon
{
    public class CutLabor : Labor
    {

        private Tile tile;

        public CutLabor(Tile tile) : base(LaborType.Woodcutter)
        {
            this.tile = tile;

            tile.SetMark(24);

            GotoCutTask cutTask = new GotoCutTask(tile);

            cutTask.AddOnCompletedCallback(RemoveMarkOnTaskCompleted);

            tasks.Add(cutTask);
        }

        private void RemoveMarkOnTaskCompleted(Task task)
        {
            tile.SetMark(0);
        }

    }
}

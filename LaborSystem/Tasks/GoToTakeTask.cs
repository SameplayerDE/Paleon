using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paleon
{
    public class GotoTakeTask : Task
    {

        public ItemContainer ItemCntr { get; private set; }

        private Tile targetTile;

        private bool adjacent;

        public GotoTakeTask(ItemContainer targetItemCntr, bool adjacent)
        {
            targetTile = targetItemCntr.Tile;
            ItemCntr = targetItemCntr;

            this.adjacent = adjacent;
        }

        public override bool Check(SettlerCmp settler)
        {
            return settler.Pathfinder.IsPathAvailable(targetTile, adjacent);
        }

        public override void Begin(SettlerCmp settler)
        {
            ItemCntr.Owner = settler;
        }

        public override void BeforeUpdate(SettlerCmp settler)
        {
            settler.Pathfinder.SetPath(targetTile, adjacent);
        }

        public override TaskState Update(SettlerCmp settler)
        {
            switch (settler.Pathfinder.MovementState)
            {
                case MovementState.Success:
                    {
                        // Подбираем предмет по достижению целевого тайла
                        settler.Inventory.Cargo = ItemCntr.Item;
                        ItemCntr.Item = null;
                        ItemCntr.Owner = null;

                        settler.PlayIdleAnimation();
                        State = TaskState.Success;
                    }
                    break;
                case MovementState.Fail:
                    {
                        settler.PlayIdleAnimation();
                        State = TaskState.Fail;
                    }
                    break;
                case MovementState.Running:
                    {
                        settler.PlayWalkAnimation();
                        State = TaskState.Running;
                    }
                    break;
            }

            return State;
        }

        public override void Cancel(SettlerCmp settler)
        {
            settler.Pathfinder.ResetPath();

            ItemCntr.Owner = null;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paleon
{
    public class GotoPutTask : Task
    {

        private Tile targetTile;
        public ItemContainer ItemCntr { get; private set; }

        private bool adjacent;

        private BuildingCmp building;

        public GotoPutTask(Tile targetTile, bool adjacent, BuildingCmp building)
        {
            this.targetTile = targetTile;

            this.adjacent = adjacent;

            this.building = building;
        }

        public override bool Check(SettlerCmp settler)
        {
            return settler.Pathfinder.IsPathAvailable(targetTile, adjacent);
        }

        public override void Begin(SettlerCmp settler)
        {
            ItemCntr = targetTile.CreateItemContainer(settler);
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
                        // Кладем предмет по достижению целевого тайла
                        ItemCntr.Item = settler.Inventory.Cargo;

                        // Если перенос предмета был создан для строительства здания
                        if (building != null)
                            building.DeliverItem(ItemCntr.Item);

                        ItemCntr.Owner = null;

                        settler.Inventory.Cargo = null;

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

            // Бросаем перетаскиваемый предмет на землю
            if (settler.Inventory.Cargo != null)
            {
                ItemCntr = settler.Pathfinder.CurrentTile.CreateItemContainer(settler.Inventory.Cargo);
                settler.Inventory.Cargo = null;
            }
        }

    }
}

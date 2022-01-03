using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paleon
{
    public class GotoTakeFromStockpile : Task
    {
        public Item TargetItem { get; private set; }

        private ItemContainer itemCntr;

        public GotoTakeFromStockpile(Item item)
        {
            TargetItem = item;   
        }

        public override bool Check(SettlerCmp settler)
        {
            itemCntr = GameplayScene.WorldManager.StorageManager.GetItemContainerWith(settler.Pathfinder.CurrentTile.Room.Id, TargetItem);
            
            return itemCntr != null;
        }

        public override void Begin(SettlerCmp settler)
        {
            itemCntr.Owner = settler;
        }

        public override void BeforeUpdate(SettlerCmp settler)
        {
            settler.Pathfinder.SetPath(itemCntr.Tile, false);
        }

        public override TaskState Update(SettlerCmp settler)
        {
            switch (settler.Pathfinder.MovementState)
            {
                case MovementState.Success:
                    {
                        // Подбираем предмет по достижению целевого тайла
                        settler.Inventory.Cargo = itemCntr.Item;
                        itemCntr.Item = null;
                        itemCntr.Owner = null;

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

            itemCntr.Owner = null;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paleon
{
    public class GotoTakeToolFromStockpile : Task
    {
        public Item TargetItem { get; private set; }

        private ItemContainer itemCntr;

        private bool hasTool = false;

        public GotoTakeToolFromStockpile(Item item)
        {
            TargetItem = item;
        }

        public override bool Check(SettlerCmp settler)
        {
            hasTool = false;

            if (settler.Inventory.Tool != null && settler.Inventory.Tool.Id == TargetItem.Id)
            {
                hasTool = true;
                return true;
            }

            itemCntr = GameplayScene.WorldManager.StorageManager.GetItemContainerWith(settler.Pathfinder.CurrentTile.Room.Id, TargetItem);

            return itemCntr != null;
        }

        public override void Begin(SettlerCmp settler)
        {
            if(!hasTool)
                itemCntr.Owner = settler;
        }

        public override void BeforeUpdate(SettlerCmp settler)
        {
            if (!hasTool)
                settler.Pathfinder.SetPath(itemCntr.Tile, false);
        }

        public override TaskState Update(SettlerCmp settler)
        {
            if (hasTool)
            {
                State = TaskState.Success;
                return TaskState.Success;
            }

            switch (settler.Pathfinder.MovementState)
            {
                case MovementState.Success:
                    {
                        // Подбираем предмет по достижению целевого тайла
                        settler.Inventory.Tool = itemCntr.Item;
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
            
            if(itemCntr != null)
                itemCntr.Owner = null;
        }
    }
}

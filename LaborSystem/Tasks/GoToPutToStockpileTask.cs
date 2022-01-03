using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Paleon
{
    public class GotoPutToStockpileTask : Task
    {
        private Tile stockpileTile;
        private Tile storageTile;
        private StorageBuildingCmp storage;

        private Item putItem;

        public ItemContainer ItemCntr { get; private set; }

        public GotoPutToStockpileTask(Item putItem)
        {
            this.putItem = putItem;
        }

        public override bool Check(SettlerCmp settler)
        {
            // Ищем тайл куда можно положить груз поселенца
            if(putItem.FoodValue > 0)
            {
                storage = GameplayScene.WorldManager.StorageManager.GetFreeStorage(settler);

                if (storage != null)
                {
                    storageTile = storage.GetReachableTile(settler);
                    return true;
                }
            }

            stockpileTile = GameplayScene.WorldManager.StorageManager.GetEmptyTile(settler.Pathfinder.CurrentTile.Room.Id);
            
            return stockpileTile != null;
        }

        public override void Begin(SettlerCmp settler)
        {
            if(stockpileTile != null)
                ItemCntr = stockpileTile.CreateItemContainer(settler);
            else
                storage.CountToAdd += 1;
        }

        public override void BeforeUpdate(SettlerCmp settler)
        {
            if (stockpileTile != null)
                settler.Pathfinder.SetPath(stockpileTile, true);
            else
                settler.Pathfinder.SetPath(storageTile, true);
        }

        public override TaskState Update(SettlerCmp settler)
        {
            switch (settler.Pathfinder.MovementState)
            {
                case MovementState.Success:
                    {
                        if(stockpileTile != null)
                        {
                            // Кладем предмет на склад
                            ItemCntr.Item = settler.Inventory.Cargo;
                            ItemCntr.Owner = null;
                        }
                        else if(storageTile != null)
                        {
                            // Добавляем предмет в хранилище
                            storage.AddItem(settler.Inventory.Cargo, 1);
                        }

                        // Убираем предмет из поселенца
                        settler.Inventory.Cargo = null;

                        settler.PlayIdleAnimation();
                        State = TaskState.Success;
                    }
                    break;
                case MovementState.Fail:
                    {
                        // Сбрасываем первый контейнер предметов
                        ItemCntr.Owner = null;

                        // Находим и устанавливаем новый путь к складку
                        stockpileTile = GameplayScene.WorldManager.StorageManager.GetEmptyTile(settler.Pathfinder.CurrentTile.Room.Id);
                        if (stockpileTile != null)
                        {
                            settler.Pathfinder.SetPath(stockpileTile, false);

                            // Создаем и занимаем контейнер для предмета
                            ItemCntr = stockpileTile.CreateItemContainer(settler);
                            State = TaskState.Running;
                        }
                        else
                        {
                            settler.PlayIdleAnimation();
                            State = TaskState.Fail;
                        }
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

            if (stockpileTile != null)
                ItemCntr.Owner = null;
            else if (storageTile != null)
                storage.CountToAdd -= 1;

            // Бросаем перетаскиваемый предмет на землю
            if (settler.Inventory.Cargo != null)
            {
                ItemCntr = settler.Pathfinder.CurrentTile.CreateItemContainer(settler.Inventory.Cargo);
                settler.Inventory.Cargo = null;
            }
        }

    }
}

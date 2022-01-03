using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paleon
{
    public class StorageManager
    {

        public List<Stockpile> Stockpiles { get; private set; }

        private List<StorageBuildingCmp> storages;

        public int Count
        {
            get { return Stockpiles.Count; }
        }

        public StorageManager()
        {
            Stockpiles = new List<Stockpile>();

            storages = new List<StorageBuildingCmp>();
        }

        public Stockpile CreateStockpile()
        {
            Stockpile stockpile = new Stockpile();
            Stockpiles.Add(stockpile);
            return stockpile;
        }

        public Tile GetEmptyTile(int roomId)
        {
            if (Stockpiles.Count == 0)
                return null;

            for (int i = 0; i < Stockpiles.Count; i++)
            {
                Stockpile stockpile = Stockpiles[i];
                if (stockpile.RoomId == roomId && stockpile.EmptyTilesCount > 0)
                    return Stockpiles[i].GetEmptyTile();
            }

            return null;
        }

        public ItemContainer GetItemContainerWith(int roomId, Item item)
        {
            for(int i = 0; i < Stockpiles.Count; i++)
            {
                Stockpile stockpile = Stockpiles[i];
                if (stockpile.RoomId == roomId)
                {
                    ItemContainer itemCntr = stockpile.GetItemContainerWith(item);
                    if (itemCntr != null)
                        return itemCntr;
                }
            }

            return null;
        }

        public ItemContainer GetItemContainerWithFood(int roomId)
        {
            for (int i = 0; i < Stockpiles.Count; i++)
            {
                Stockpile stockpile = Stockpiles[i];
                if (stockpile.RoomId == roomId)
                {
                    ItemContainer itemCntr = stockpile.GetItemContainerWithFood();
                    if (itemCntr != null)
                        return itemCntr;
                }
            }

            return null;
        }

        public void AddStorage(StorageBuildingCmp storageBuilding)
        {
            storages.Add(storageBuilding);
        }

        public StorageBuildingCmp GetFreeStorage(SettlerCmp settler)
        {
            for(int i = 0; i < storages.Count; i++)
            {
                StorageBuildingCmp storage = storages[i];

                if(storage.Completed && storage.FreeCount > 0)
                {
                    Tile tile = storage.GetReachableTile(settler);

                    if (tile != null)
                        return storage;
                }
            }

            return null;
        }

    }
}

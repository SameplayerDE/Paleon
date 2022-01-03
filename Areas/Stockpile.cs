using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paleon
{
    public class Stockpile : Area
    {
        public CrafterBuildingCmp LinkedBuilding { get; set; }

        private Dictionary<Item, List<Tile>> itemsWithoutOwner;
        private List<Tile> emptyTiles;
        private List<Tile> foodTiles;

        public int RoomId { get { return Tiles[0].Room.Id; } }

        public int EmptyTilesCount
        {
            get { return emptyTiles.Count; }
        }

        public Stockpile() : base("Stockpile", Color.Blue)
        {
            itemsWithoutOwner = new Dictionary<Item, List<Tile>>();
            emptyTiles = new List<Tile>();
            foodTiles = new List<Tile>();
        }

        public override void AddTiles(List<Tile> tiles)
        {
            base.AddTiles(tiles);

            for (int i = 0; i < tiles.Count; i++)
                Update(tiles[i]);
        }

        public void Update(Tile tile)
        {
            // Удаляем тайл с коллекции занятых предметов
            RemoveFromCollection(tile, itemsWithoutOwner);

            // Удаляем тайл с коллекции с незанятыми предметами
            if (emptyTiles.Contains(tile))
                emptyTiles.Remove(tile);

            if (foodTiles.Contains(tile))
                foodTiles.Remove(tile);

            // Добавляем тайлы в коллекцию с незанятыми предметами
            foreach (ItemContainer itemCntr in tile.ItemContainers)
            {
                if(itemCntr.Item != null && itemCntr.Owner == null)
                {
                    if (itemCntr.Item.FoodValue > 0)
                        foodTiles.Add(itemCntr.Tile);

                    if(itemsWithoutOwner.ContainsKey(itemCntr.Item))
                    {
                        itemsWithoutOwner[itemCntr.Item].Add(tile);
                    }
                    else
                    {
                        itemsWithoutOwner.Add(itemCntr.Item, new List<Tile>() { tile });
                    }
                }
            }

            // Добавляем тайлы без контейнеров в список пустых тайлов
            if (tile.ItemContainers.Count == 0)
                emptyTiles.Add(tile);
        }

        public ItemContainer GetItemContainerWith(Item item)
        {
            List<Tile> tiles;
            if (itemsWithoutOwner.TryGetValue(item, out tiles))
            {
                if (tiles.Count > 0)
                {
                    foreach(ItemContainer itemCntr in tiles[0].ItemContainers)
                    {
                        if (itemCntr.Item.Id == item.Id)
                            return itemCntr;
                    }
                }
            }

            return null;
        }

        public ItemContainer GetItemContainerWithFood()
        {
            if (foodTiles.Count == 0)
                return null;

            for(int i = 0; i < foodTiles[0].ItemContainers.Count; i++)
            {
                ItemContainer ic = foodTiles[0].ItemContainers[i];
                if (ic.Item.FoodValue > 0 && ic.Owner == null)
                    return ic;
            }

            return null;
        }

        public Tile GetEmptyTile()
        {
            if (emptyTiles.Count > 0)
                return emptyTiles[0];

            return null;
        }

        private void RemoveFromCollection(Tile tile, Dictionary<Item, List<Tile>> collection)
        {
            if (collection.Count > 0)
            {
                foreach (Item item in collection.Keys)
                {
                    List<Tile> tiles = collection[item];
                    if (tiles.Count > 0 && tiles.Contains(tile))
                    {
                        tiles.Remove(tile);
                        break;
                    }
                }
            }
        }

    }
}

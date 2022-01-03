using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paleon
{
    public class ItemContainer
    {

        public Tile Tile { get; private set; }
        public Labor Labor { get; set; }

        private Item item;
        private SettlerCmp owner;

        public Item Item
        {
            get { return item; }
            set
            {
                if (item == value)
                    return;

                item = value;

                if (item != null)
                {
                    Tile.ItemTileMap.SetCell(Tile.X, Tile.Y, item.Id);
                }
                else
                {
                    bool itemWasFound = false;

                    // После удаление предмета из контейнера, смотрим - нет ли предмета в 
                    // других контейнерах этого тайла, чтобы было что изобразить в TileMap
                    foreach(ItemContainer itemCntr in Tile.ItemContainers)
                    {
                        if(itemCntr.Item != null)
                        {
                            Tile.ItemTileMap.SetCell(Tile.X, Tile.Y, itemCntr.Item.Id);
                            itemWasFound = true;
                        }
                    }

                    if(!itemWasFound)
                        Tile.ItemTileMap.SetCell(Tile.X, Tile.Y, 0);
                }

                if (item == null && owner == null)
                {
                    Tile.ItemContainers.Remove(this);
                }

                if(Tile.Area != null && Tile.Area is Stockpile)
                {
                    Stockpile stockpile = Tile.Area as Stockpile;
                    stockpile.Update(Tile);
                }
            }
        }

        public SettlerCmp Owner
        {
            get { return owner; }
            set
            {
                if (owner == value)
                    return;

                owner = value;

                if (item == null && owner == null)
                {
                    Tile.ItemContainers.Remove(this);
                }

                if(Tile.Area != null && Tile.Area is Stockpile)
                {
                    Stockpile stockpile = Tile.Area as Stockpile;
                    stockpile.Update(Tile);
                }
            }
        }

        public ItemContainer(Tile tile)
        {
            Tile = tile;
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paleon
{
    public class Room
    {

        public int Id { get; set; }

        private List<Tile> tiles;

        public List<Room> Neighbours { get; private set; }

        public int TilesCount
        {
            get { return tiles.Count; }
        }

        public Room(int id)
        {
            Id = id;
            tiles = new List<Tile>();
            Neighbours = new List<Room>();
        }

        public void AddTile(Tile tile)
        {
            tiles.Add(tile);
            tile.Room = this;
        }

        public void RemoveTile(Tile tile)
        {
            tiles.Remove(tile);
            tile.Room = null;
        }

        public bool Contains(Tile tile)
        {
            return tiles.Contains(tile);
        }

    }
}

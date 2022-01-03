using System.Collections.Generic;

namespace Paleon
{
    public class River
    {

        public int Length;
        public List<PixelTile> Tiles;
        public int ID;

        public int Intersections;
        public float TurnCount;
        public Direction CurrentDirection;

        public River(int id)
        {
            ID = id;
            Tiles = new List<PixelTile>();
        }

        public void AddTile(PixelTile tile)
        {
            tile.SetRiverPath(this);
            Tiles.Add(tile);
        }
    }

    public class RiverGroup
    {
        public List<River> Rivers = new List<River>();
    }
}

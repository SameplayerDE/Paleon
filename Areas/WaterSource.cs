using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Paleon
{
    public class WaterSource : Area
    {

        private List<Tile> coastTiles;

        public WaterSource() : base("Water source", Color.Pink)
        {
            coastTiles = new List<Tile>();
        }

        public override void AddTiles(List<Tile> tiles)
        {
            base.AddTiles(tiles);

            // Выбираем только береговые тайлы
            for (int i = 0; i < tiles.Count; i++)
            {
                Tile tile = tiles[i];

                if (IsCoastTile(tile))
                    coastTiles.Add(tile);
            }
        }

        private bool IsCoastTile(Tile tile)
        {
            // Т.к. нам необходимы только тайлы земли
            if (tile.GroundTopType == GroundTopType.WATER)
                return false;

            List<Tile> neighbours = tile.Neighbours;

            for (int i = 0; i < neighbours.Count; i++)
            {
                if (neighbours[i].GroundTopType == GroundTopType.WATER)
                    return true;
            }

            return false;
        }

        public Tile GetCoastTile(SettlerCmp settler)
        {
            for (int i = 0; i < coastTiles.Count; i++)
            {
                Tile tile = coastTiles[i];
                if (tile.Room.Id == settler.Pathfinder.CurrentTile.Room.Id)
                    return tile;
            }

            return null;
        }

    }
}

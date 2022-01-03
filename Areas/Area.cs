using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paleon
{
    public class Area
    {
        public string Name { get; private set; }
        public Color Color { get; private set; }

        public Vector2 Position { get; private set; }

        public Stockpile OutputStockpile { get; set; }
        public CrafterBuildingCmp OutputCrafterBuilding { get; set; }

        protected List<Labor> labors; 
        public int WorkerCount { get; private set; }

        public List<Tile> Tiles;

        private Rectangle rect;

        private int minX = int.MaxValue;
        private int maxX = int.MinValue;

        private int minY = int.MaxValue;
        private int maxY = int.MinValue;

        public Area(string name, Color color)
        {
            Name = name;
            Color = color * 0.4f;
            

            Tiles = new List<Tile>();
            rect = new Rectangle();

            labors = new List<Labor>();
        }

        private void AddTile(Tile tile)
        {
            Tiles.Add(tile);
            tile.Area = this;

            RecalculateCenterPosition(tile);
        }

        public virtual void AddTiles(List<Tile> tiles)
        {
            for (int i = 0; i < tiles.Count; i++)
                AddTile(tiles[i]);
        }

        public virtual void Render()
        {
            RenderManager.HollowRect(rect, Color);
        }

        public virtual void Remove()
        {
            for(int i = 0; i < Tiles.Count; ++i)
            {
                Tile tile = Tiles[i];
                tile.Area = null;
            }
        }

        private void RecalculateCenterPosition(Tile tile)
        {
            if (tile.X < minX)
                minX = tile.X;

            if (tile.X > maxX)
                maxX = tile.X;

            if (tile.Y < minY)
                minY = tile.Y;

            if (tile.Y > maxY)
                maxY = tile.Y;

            int width = (maxX + 1) - minX;
            int height = (maxY + 1) - minY;

            rect.X = minX * GameplayScene.TILE_SIZE;
            rect.Y = minY * GameplayScene.TILE_SIZE;

            rect.Width = width * GameplayScene.TILE_SIZE;
            rect.Height = height * GameplayScene.TILE_SIZE;

            Position = new Vector2(rect.Center.X - GameplayScene.TILE_SIZE / 2, rect.Center.Y - GameplayScene.TILE_SIZE / 2);
        }

    }
}

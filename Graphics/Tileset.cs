using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Paleon
{
    public class Tileset
    {

        public MyTexture[,] tiles;

        public MyTexture Texture { get; private set; }

        public int TileWidth { get; private set; }
        public int TileHeight { get; private set; }

        public MyTexture this[int x, int y]
        {
            get { return tiles[x, y]; }
        }

        public MyTexture this[int index]
        {
            get { return tiles[index % tiles.GetLength(0), index / tiles.GetLength(0)]; }
        }

        public Tileset(MyTexture texture, int tileWidth, int tileHeight)
        {
            Texture = texture;
            TileWidth = tileWidth;
            TileHeight = tileHeight;

            int columns = Texture.Width / TileWidth;
            int rows = Texture.Height / TileHeight;

            tiles = new MyTexture[columns, rows];
            for (int x = 0; x < columns; x++)
                for (int y = 0; y < rows; y++)
                    tiles[x, y] = Texture.GetSubtexture(x * TileWidth, y * TileHeight, TileWidth, TileHeight);
        }
    }
}

using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paleon
{
    public class Chunk
    {

        public int X { get; private set; }
        public int Y { get; private set; }

        private GridMesh mesh;

        private int[,] tiles;

        private bool dirty = false;

        private bool visible = false;

        public Chunk(int x, int y, int chunkSize, int tileSize)
        {
            X = x;
            Y = y;

            mesh = new GridMesh(x, y, chunkSize, chunkSize, tileSize, tileSize);

            tiles = new int[chunkSize, chunkSize];
            for (int i = 0; i < tiles.GetLength(0); i++)
                for (int j = 0; j < tiles.GetLength(1); j++)
                    tiles[i, j] = 0;

            dirty = true;
            Update();
        }

        public void Update()
        {
            if(dirty)
            {
                // If chunk doesn't has any tile it will be invisible
                visible = false;
                for (int i = 0; i < tiles.GetLength(0); i++)
                {
                    for (int j = 0; j < tiles.GetLength(1); j++)
                    {
                        if (tiles[i, j] != 0)
                        {
                            visible = true;
                            break;
                        }
                    }

                    if (visible)
                        break;
                }

                mesh.RefreshUVs();

                dirty = false;
            }
        }

        public void Render()
        {
            if(visible)
                mesh.Render();
        }

        public void SetCell(int x, int y, int tile, Vector4 uv)
        {
            if (tiles[x, y] == tile)
                return;

            tiles[x, y] = tile;
            mesh.SetCellUV(x, y, uv);
            dirty = true;
        }

        public int GetCell(int x, int y)
        {
            return tiles[x, y];
        }

    }
}

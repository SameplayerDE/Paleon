using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paleon
{
    public enum MapSize
    {
        S256 = 256,
        S128 = 128,
        S64 = 64,
        S32 = 32
    }

    public class TileMap
    {
        public Tileset Tileset { get; private set; }

        public int TileSize { get; private set; }

        public const int CHUNK_SIZE = 8;

        public int TileColumns { get; private set; }
        public int TileRows { get; private set; }

        public int ChunkColumns { get; private set; }
        public int ChunkRows { get; private set; }

        private Chunk[,] chunks;

        private Point min = new Point();
        private Point max = new Point();

        private Effect shader;

        private EffectParameter projectionParam;
        private EffectParameter viewParam;
        private EffectParameter ambientColorParam;
        private EffectParameter spriteTextureParam;

        public TileMap(Tileset tileset, int tileSize, int tilemapSize)
        {
            Tileset = tileset;

            TileSize = tileSize;

            TileColumns = tilemapSize;
            TileRows = tilemapSize;

            ChunkColumns = TileColumns / CHUNK_SIZE;
            ChunkRows = TileRows / CHUNK_SIZE;

            chunks = new Chunk[ChunkColumns, ChunkRows];
            for (int x = 0; x < ChunkColumns; x++)
            {
                for (int y = 0; y < ChunkRows; y++)
                {
                    chunks[x, y] = new Chunk(x * CHUNK_SIZE * tileSize, y * CHUNK_SIZE * tileSize, CHUNK_SIZE, tileSize);
                }
            }

            shader = ResourceManager.GetEffect("Tilemap");

            projectionParam = shader.Parameters["projection"];
            viewParam = shader.Parameters["view"];
            ambientColorParam = shader.Parameters["ambientColor"];
            spriteTextureParam = shader.Parameters["SpriteTexture"];
    }

        public void Update(MainCamera camera)
        {
            for (int x = 0; x < ChunkColumns; x++)
            {
                for (int y = 0; y < ChunkRows; y++)
                {
                    chunks[x, y].Update();
                }
            }

            Rectangle viewport = camera.GetViewport();

            int xCamPoint = viewport.X / (TileSize * CHUNK_SIZE);
            int yCamPoint = viewport.Y / (TileSize * CHUNK_SIZE);

            int xViewPort = viewport.Right / (TileSize * CHUNK_SIZE);
            int yViewPort = viewport.Bottom / (TileSize * CHUNK_SIZE);

            min.X = Math.Max(0, xCamPoint);
            min.Y = Math.Max(0, yCamPoint);
            max.X = Math.Min(xViewPort + 1, ChunkColumns);
            max.Y = Math.Min(yViewPort + 1, ChunkRows);
        }

        public void Render(Color color)
        {
            RenderManager.Begin(SamplerState.PointClamp, BlendState.AlphaBlend);

            projectionParam.SetValue(RenderManager.MainCamera.Projection);
            viewParam.SetValue(RenderManager.MainCamera.Transformation);
            ambientColorParam.SetValue(color.ToVector4());
            spriteTextureParam.SetValue(Tileset.Texture.Texture);

            shader.CurrentTechnique.Passes[0].Apply();

            for (int x = min.X; x < max.X; x++)
                for (int y = min.Y; y < max.Y; y++)
                    chunks[x, y].Render();
        }

        public void SetCell(int x, int y, int tile)
        {
            if (x < 0 || y < 0 || x >= TileColumns || y >= TileRows)
                return;

            int chunkX = x / CHUNK_SIZE;
            int chunkY = y / CHUNK_SIZE;

            MyTexture texture = Tileset[tile];
            Vector4 uv = new Vector4(texture.LeftUV, texture.TopUV, texture.RightUV, texture.BottomUV);

            chunks[chunkX, chunkY].SetCell(x - chunkX * CHUNK_SIZE, y - chunkY * CHUNK_SIZE, tile, uv);
        }

        public int GetCell(int x, int y)
        {
            if (x < 0 || y < 0 || x >= TileColumns || y >= TileRows)
                throw new Exception("Out of bounds!");

            int chunkX = x / CHUNK_SIZE;
            int chunkY = y / CHUNK_SIZE;

            return chunks[chunkX, chunkY].GetCell(x - chunkX * CHUNK_SIZE, y - chunkY * CHUNK_SIZE);
        }

    }
}

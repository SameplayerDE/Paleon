using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;

namespace Paleon
{
    public class GameplayScene : Scene
    {
        public static GameplayScene Instance { get; private set; }

        public const int TILE_SIZE = 16;
        public const int TILEMAP_SIZE = 128;

        private static Tile[,] tiles;

        public static PathTileGraph<Tile> PathTileGraph { get; private set; }

        public static Tile MouseTile { get; private set; }
        public static Vector2 MouseTilePosition { get; private set; }
        public static Vector2 MouseWorldPosition { get; private set; }

        public static UIRenderer UIRenderer { get; private set; }
        public static bool MouseOnUI { get; set; } = false;

        public static WorldManager WorldManager { get; private set; }

        public static List<SettlerCmp> Settlers = new List<SettlerCmp>();
        public static List<AnimalCmp> Animals = new List<AnimalCmp>();

        private static TileMap groundTileMap;
        private static TileMap groundTopTileMap;
        private static TileMap blockTileMap;
        private static TileMap itemTileMap;
        private static TileMap markTileMap;

        public static Layer EntityLayer;

        private CameraController cameraController;

        // Для отображения регионов
        private static bool showRooms = false;
        // Для отображения путей поселенцев
        private static bool showPaths = false;

        private static Dictionary<int, Color> randomColors = new Dictionary<int, Color>();

        public static Effect EntityShader { get; private set; }

        public GameplayScene()
        {
            Instance = this;

            for (int i = 0; i < 100; i++)
                randomColors.Add(i, Utils.GetRandomColor(0.3f));

            groundTileMap = new TileMap(TextureBank.GroundTileset, TILE_SIZE, TILEMAP_SIZE);
            groundTopTileMap = new TileMap(TextureBank.GroundTopTileset, TILE_SIZE, TILEMAP_SIZE);
            blockTileMap = new TileMap(TextureBank.BlockTileset, TILE_SIZE, TILEMAP_SIZE);
            itemTileMap = new TileMap(TextureBank.GroundTileset, TILE_SIZE, TILEMAP_SIZE);
            markTileMap = new TileMap(TextureBank.UiTileset, TILE_SIZE, TILEMAP_SIZE);

            tiles = new Tile[TILEMAP_SIZE, TILEMAP_SIZE];
            for(int x = 0; x < TILEMAP_SIZE; x++)
            {
                for (int y = 0; y < TILEMAP_SIZE; y++)
                {
                    tiles[x, y] = new Tile(x, y, groundTileMap, groundTopTileMap, blockTileMap, itemTileMap, markTileMap);
                    tiles[x, y].GroundType = GroundType.GROUND;
                }
            }

            // Adding neighbours to tiles
            for (int x = 0; x < TILEMAP_SIZE; x++)
            {
                for (int y = 0; y < TILEMAP_SIZE; y++)
                {
                    Tile tile = tiles[x, y];

                    for (int nX = x - 1; nX <= x + 1; nX++)
                    {
                        for (int nY = y - 1; nY <= y + 1; nY++)
                        {
                            if (nX == x && nY == y)
                                continue;

                            if (nX < 0 || nY < 0 || nX >= TILEMAP_SIZE || nY >= TILEMAP_SIZE)
                                continue;

                            tile.AllNeighbours.Add(tiles[nX, nY]);

                            if (nX != x && nY == y)
                                tile.Neighbours.Add(tiles[nX, nY]);

                            if (nX == x && nY != y)
                                tile.Neighbours.Add(tiles[nX, nY]);
                        }
                    }
                }
            }

            PathTileGraph = new PathTileGraph<Tile>(tiles);

            MChunk[,] chunks = new MChunk[TILEMAP_SIZE / MChunk.CHUNK_SIZE, TILEMAP_SIZE / MChunk.CHUNK_SIZE];

            // Chunks creating
            for (int x = 0; x < chunks.GetLength(0); x++)
            {
                for (int y = 0; y < chunks.GetLength(1); y++)
                {
                    MChunk chunk = new MChunk(x, y);
                    chunks[x, y] = chunk;

                    int tX = x * MChunk.CHUNK_SIZE;
                    int tY = y * MChunk.CHUNK_SIZE;

                    for (int i = tX; i < tX + MChunk.CHUNK_SIZE; i++)
                    {
                        for (int j = tY; j < tY + MChunk.CHUNK_SIZE; j++)
                        {
                            chunk.AddTile(tiles[i, j]);
                        }
                    }
                }
            }

            // Chunks neighbours setting
            for (int x = 0; x < chunks.GetLength(0); x++)
            {
                for (int y = 0; y < chunks.GetLength(1); y++)
                {
                    MChunk chunk = chunks[x, y];

                    if (x + 1 >= 0 && x + 1 < chunks.GetLength(0))
                        chunk.AddNeighbour(chunks[x + 1, y]);

                    if(y + 1 >= 0 && y + 1 < chunks.GetLength(1))
                        chunk.AddNeighbour(chunks[x, y + 1]);

                    if(x - 1 >= 0 && x - 1 < chunks.GetLength(0))
                        chunk.AddNeighbour(chunks[x - 1, y]);

                    if(y - 1 >= 0 && y - 1 < chunks.GetLength(1))
                        chunk.AddNeighbour(chunks[x, y - 1]);
                }
            }

            WorldManager = new WorldManager();

            cameraController = new CameraController();
            cameraController.Begin();

            ItemDatabase.Initialize();

            EntityLayer = new Layer(this, "Entity");

            //Animal animal = new Animal(this, tiles);
            //animal.Position.X = 16 * TILE_SIZE;
            //animal.Position.Y = 10 * TILE_SIZE;
            //Animals.Add(animal.Get<AnimalCmp>());
            //EntityLayer.Add(animal);

            HeightsGenerator plantGenerator = new HeightsGenerator(TILEMAP_SIZE, TILEMAP_SIZE, 24124, 10, 3, 0.5f);
            HeightsGenerator waterGenerator = new HeightsGenerator(TILEMAP_SIZE, TILEMAP_SIZE, 24124, 7, 5, 0.3f);
            HeightsGenerator grassGenerator = new HeightsGenerator(TILEMAP_SIZE, TILEMAP_SIZE, 1521, 7, 3, 0.3f);
            HeightsGenerator stoneGenerator = new HeightsGenerator(TILEMAP_SIZE, TILEMAP_SIZE, 5135, 7, 3, 0.3f);
            HeightsGenerator clayGenerator = new HeightsGenerator(TILEMAP_SIZE, TILEMAP_SIZE, 5315, 7, 3, 0.3f);
            HeightsGenerator flintGenerator = new HeightsGenerator(TILEMAP_SIZE, TILEMAP_SIZE, 5151, 7, 3, 0.3f);

            for (int x = 0; x < TILEMAP_SIZE; x++)
            {
                for (int y = 0; y < TILEMAP_SIZE; y++)
                {
                    Tile tile = GetTile(x, y);

                    float waterHeight = waterGenerator.GenerateHeight(x, y);
                    if (waterHeight > 1)
                    {
                        tile.GroundTopType = GroundTopType.WATER;
                    }

                    float stoneHeight = stoneGenerator.GenerateHeight(x, y);
                    if (stoneHeight > 1 && tile.GroundTopType != GroundTopType.WATER)
                    {
                        tile.SetBlock(new Block(BlockType.STONE, false, ItemDatabase.STONE, 10));
                    }

                    float clayHeight = clayGenerator.GenerateHeight(x, y);
                    if(clayHeight > 2 && tile.GroundTopType != GroundTopType.WATER && tile.BlockType == BlockType.NONE)
                    {
                        tile.SetBlock(new Block(BlockType.CLAY, false, ItemDatabase.CLAY, 5));
                    }

                    float grassHeight = grassGenerator.GenerateHeight(x, y);
                    if (grassHeight > -2.0f && tile.GroundTopType != GroundTopType.WATER && tile.BlockType == BlockType.NONE)
                    {
                        tile.GroundTopType = GroundTopType.GRASS;
                    }

                    float flintHeight = flintGenerator.GenerateHeight(x, y);
                    if (flintHeight > 2.2f && tile.GroundTopType != GroundTopType.WATER && tile.BlockType == BlockType.NONE)
                    {
                        int random = MyRandom.Range(2);

                        if(random == 0)
                        {
                            tile.CreateItemContainer(ItemDatabase.FLINT);
                        }
                        else if(random == 1)
                        {
                            tile.CreateItemContainer(ItemDatabase.STICK);
                        }
                    }

                    float height = plantGenerator.GenerateHeight(x, y);
                    if (height > 1)
                    {
                        bool nearWaterTile = tile.Neighbours.Where(n => n.GroundTopType == GroundTopType.WATER).Count() > 0;

                        int random = MyRandom.Range(1, 20);
                        if (tile.GroundTopType != GroundTopType.WATER && tile.ItemContainers.Count == 0
                             && tile.BlockType == BlockType.NONE)
                        {
                            if (random == 1)
                            {
                                RaspberryBush raspberry = new RaspberryBush(tile, this);
                                AddEntity(raspberry, tile);
                            }
                            else if(random == 2)
                            {
                                Fern fern = new Fern(tile, this);
                                AddEntity(fern, tile);
                            }
                            else if (random == 11)
                            {
                                PineTree pine = new PineTree(tile, this);
                                AddEntity(pine, tile);
                            }
                            else if(random > 3 && random < 10 && nearWaterTile)
                            {
                                Cattail cattail = new Cattail(tile, this);
                                AddEntity(cattail, tile);
                            }

                        }
                    }
                }
            }

            Settler settler2 = new Settler(this, tiles, "Misi", false);
            settler2.Position.X = 50 * TILE_SIZE;
            settler2.Position.Y = 20 * TILE_SIZE;
            Settlers.Add(settler2.Get<SettlerCmp>());
            EntityLayer.Add(settler2);

            Settler settler1 = new Settler(this, tiles, "Jesta", false);
            settler1.Position.X = 52 * TILE_SIZE;
            settler1.Position.Y = 20 * TILE_SIZE;
            Settlers.Add(settler1.Get<SettlerCmp>());
            EntityLayer.Add(settler1);

            Settler settler3 = new Settler(this, tiles, "Hulu", true);
            settler3.Position.X = 54 * TILE_SIZE;
            settler3.Position.Y = 20 * TILE_SIZE;
            Settlers.Add(settler3.Get<SettlerCmp>());
            EntityLayer.Add(settler3);

            EntityLayer.UpdateLists();

            UIRenderer = new UIRenderer();

            SetTime(6);

            EntityShader = ResourceManager.GetEffect("Ambient");
        }

        public override void Begin()
        {
            WorldManager.Begin();
        }

        public override void Update()
        {
            MouseOnUI = false;
            UIRenderer.Update();

            UpdateMouseWorldPosition();
            UpdateMouseTilePosition();
            cameraController.Update();

            WorldManager.Update();

            EntityLayer.UpdateLists();
            EntityLayer.Update();

            groundTileMap.Update(RenderManager.MainCamera);
            groundTopTileMap.Update(RenderManager.MainCamera);
            blockTileMap.Update(RenderManager.MainCamera);
            itemTileMap.Update(RenderManager.MainCamera);
            markTileMap.Update(RenderManager.MainCamera);
        }

        public override void Render()
        {
            groundTileMap.Render(WorldManager.TimeOfDayColor);
            groundTopTileMap.Render(WorldManager.TimeOfDayColor);
            blockTileMap.Render(WorldManager.TimeOfDayColor);
            itemTileMap.Render(WorldManager.TimeOfDayColor);

            EntityShader.Parameters["ambientColor"].SetValue(WorldManager.TimeOfDayColor.ToVector4());

            RenderManager.SpriteBatch.Begin(
                SpriteSortMode.Deferred,
                BlendState.AlphaBlend,
                SamplerState.PointClamp,
                null, null, EntityShader, RenderManager.MainCamera.Transformation);

            EntityLayer.Render();

            RenderManager.SpriteBatch.End();

            markTileMap.Render(Color.White);

            RenderManager.SpriteBatch.Begin(
                SpriteSortMode.Deferred,
                BlendState.AlphaBlend,
                SamplerState.PointClamp,
                null, null, null, RenderManager.MainCamera.Transformation);

            WorldManager.Render();

            if (showRooms)
            {
                for (int x = 0; x < tiles.GetLength(0); x++)
                    for (int y = 0; y < tiles.GetLength(1); y++)
                    {
                        Tile tile = tiles[x, y];
                        if (tile.Room != null)
                        {
                            RenderManager.Rect(x * TILE_SIZE, y * TILE_SIZE, TILE_SIZE, TILE_SIZE, randomColors[tiles[x, y].Room.Id]);
                        }
                    }
            }

            if(showPaths)
            {
                for(int i = 0; i < Settlers.Count; i++)
                {
                    Settlers[i].Pathfinder.DebugPathRender();
                }
            }

            RenderManager.SpriteBatch.End();

            UIRenderer.Render();
        }

        public static Tile GetTile(int x, int y)
        {
            if (x < 0 || x >= tiles.GetLength(0))
                return null;

            if (y < 0 || y >= tiles.GetLength(1))
                return null;

            return tiles[x, y];
        }

        public static void AddEntity(Entity entity)
        {
            EntityLayer.Add(entity);
        }

        public static void AddEntity(Entity entity, Tile tile)
        {
            tile.Entity = entity;
            EntityLayer.Add(entity);
        }

        private void UpdateMouseWorldPosition()
        {
            float mposx = (MInput.Mouse.X - Engine.HalfWidth) / RenderManager.MainCamera.Zoom;
            float mposy = (MInput.Mouse.Y - Engine.HalfHeight) / RenderManager.MainCamera.Zoom;

            MouseWorldPosition = new Vector2((int)mposx + RenderManager.MainCamera.Position.X, (int)mposy + RenderManager.MainCamera.Position.Y);
        }

        private void UpdateMouseTilePosition()
        {
            Vector2 globalPos = MouseWorldPosition;
            int x = (int)globalPos.X / TILE_SIZE;
            int y = (int)globalPos.Y / TILE_SIZE;

            if (x < 0)
                x = 0;
            else if (x >= tiles.GetLength(0))
                x = tiles.GetLength(0) - 1;

            if (y < 0)
                y = 0;
            else if (y >= tiles.GetLength(1))
                y = tiles.GetLength(1) - 1;

            MouseTile = tiles[x, y];
            MouseTilePosition = new Vector2(x * TILE_SIZE, y * TILE_SIZE);
        }

        [Command("ShowRooms", "Shows colored rooms")]
        public static void ShowRooms()
        {
            showRooms = true;
        }

        [Command("HideRooms", "Hides colored rooms")]
        public static void HideRooms()
        {
            showRooms = false;
        }

        [Command("ShowPaths", "Shows all creatures paths")]
        public static void ShowPaths()
        {
            showPaths = true;
        }

        [Command("HidePaths", "Hides all creatures paths")]
        public static void HidePaths()
        {
            showPaths = false;
        }

        [Command("SetTime", "Sets current time")]
        public static void SetTime(int hour)
        {
            WorldManager.WorldTimer.SetTime(hour);
        }
    }
}

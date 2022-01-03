using Microsoft.Xna.Framework;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;

namespace Paleon
{
    public class BuildingCmp : Component
    {
        public string Name { get; private set; }
        public MyTexture Icon { get; private set; }
        public Sprite Sprite { get; private set; }
        public TileInfo[,] TilesInfo { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }

        public List<TileInfo> OccupiedTiles { get; private set; }
        public Vector2 CenterPosition { get; private set; }
        public bool Completed { get; private set; }

        public List<Item> CraftingRecipe { get; private set; }

        private Image buildingTemplateImage;

        private List<Tile> targetTiles;

        private List<Tuple<Item, bool>> craftingProgress;

        private BuildLabor buildLabor;
        private List<HaulLabor> haulLabors;

        private int minX = int.MaxValue;
        private int minY = int.MaxValue;

        private int maxX = int.MinValue;
        private int maxY = int.MinValue;

        public BuildingCmp(string json) : base(true, true)
        {
            JObject jobject = JObject.Parse(File.ReadAllText(Engine.ContentDirectory + "\\Buildings\\" + json + ".json"));

            Name = jobject["name"].Value<string>();

            Width = jobject["width"].Value<int>();
            Height = jobject["height"].Value<int>();

            MyTexture texture = ResourceManager.GetTexture(jobject["texture"]["name"].Value<string>());

            int textureWidth = jobject["texture"]["width"].Value<int>();
            int textureHeight = jobject["texture"]["height"].Value<int>();

            Icon = texture.GetSubtexture(0, 0, textureWidth, textureHeight);

            int frameCount = texture.Width / textureWidth;

            Sprite = new Sprite(textureWidth, textureHeight);
            Sprite.Add("Idle", new Animation(texture, 1, 0, textureWidth, textureHeight, 0, 0, 1));

            if (jobject["animated"].Value<bool>())
            {
                Sprite.Add("Process", new Animation(texture, frameCount, 0, textureWidth, textureHeight, 0, textureHeight, 5));
            }

            TilesInfo = new TileInfo[Width, Height];

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    bool walkable = jobject["walkable_pattern"][y][x].Value<bool>();
                    bool target = jobject["target_pattern"][y][x].Value<bool>();
                    TilesInfo[x, y] = new TileInfo(walkable, target);
                }
            }

            CraftingRecipe = new List<Item>();

            if(Width == 1 && Height == 1)
                buildingTemplateImage = new Image(TextureBank.BlockTexture.GetSubtexture(64, 64, 16, 16));
            else if(Width == 1 && Height == 2)
                buildingTemplateImage = new Image(TextureBank.BlockTexture.GetSubtexture(64, 80, 16, 32));
            else if(Width == 2 && Height == 1)
                buildingTemplateImage = new Image(TextureBank.BlockTexture.GetSubtexture(80, 64, 32, 16));
            else if(Width == 2 && Height == 2)
                buildingTemplateImage = new Image(TextureBank.BlockTexture.GetSubtexture(80, 80, 32, 32));
            else if(Width == 3 && Height == 2)
                buildingTemplateImage = new Image(TextureBank.BlockTexture.GetSubtexture(112, 80, 48, 32));
        }

        public override void Update()
        {
            if (Completed)
                Sprite.Update();
        }

        public override void Render()
        {
            if (Completed)
                Sprite.Render();
            else
                buildingTemplateImage.Render();
        }

        public void SetToBuild(Tile[,] tiles)
        {
            buildingTemplateImage.Entity = Entity;

            OccupiedTiles = new List<TileInfo>();
            for(int x = 0; x < Width; ++x)
            {
                for(int y = 0; y < Height; ++y)
                {
                    Tile tile = tiles[x, y];
                    TileInfo tileInfo = TilesInfo[x, y];

                    tileInfo.Tile = tile;

                    // Устанавливаем временный шаблон
                    tile.IsWalkable = false;
                    tile.Entity = Entity;

                    OccupiedTiles.Add(tileInfo);
                }
            }

            craftingProgress = new List<Tuple<Item, bool>>();
            haulLabors = new List<HaulLabor>();

            // Создаем работы по доставке предметов
            for (int i = 0; i < CraftingRecipe.Count; i++)
            {
                Item item = CraftingRecipe[i];
                Tile tile = OccupiedTiles[i].Tile;

                craftingProgress.Add(new Tuple<Item, bool>(item, false));

              
                HaulLabor haulLabor = new HaulLabor(item, tile, this);
                haulLabors.Add(haulLabor);
                GameplayScene.WorldManager.LaborManager.Add(haulLabor);
            }
        }

        public void DeliverItem(Item item)
        {
            for (int i = 0; i < craftingProgress.Count; i++)
            {
                Item craftingItem = craftingProgress[i].Item1;
                if (craftingItem.Id == item.Id && craftingProgress[i].Item2 == false)
                {
                    craftingProgress[i] = new Tuple<Item, bool>(craftingItem, true);
                    break;
                }
            }

            bool deliveryCompleted = true;
            for (int i = 0; i < craftingProgress.Count; i++)
            {
                if (craftingProgress[i].Item2 == false)
                {
                    deliveryCompleted = false;
                    break;
                }
            }

            if(deliveryCompleted)
            {
                for (int i = 0; i < OccupiedTiles.Count; i++)
                {
                    if (OccupiedTiles[i].IsTarget)
                    {
                        buildLabor = new BuildLabor(OccupiedTiles[i].Tile, this);
                        GameplayScene.WorldManager.LaborManager.Add(buildLabor);
                        break;
                    }
                }
            }

        }

        public virtual void Complete()
        {
            Completed = true;

            Sprite.Entity = Entity;
            Sprite.Play("Idle");

            targetTiles = new List<Tile>();

            for (int i = 0; i < OccupiedTiles.Count; i++)
            {
                TileInfo tileInfo = OccupiedTiles[i];
                Tile tile = tileInfo.Tile;

                tile.IsWalkable = tileInfo.IsWalkable;

                if(tile.ItemContainers.Count > 0)
                    tile.ItemContainers[0].Item = null;

                if (tileInfo.IsTarget)
                    targetTiles.Add(tileInfo.Tile);
            }

            CalculateCenterPosition();
        }

        public void CancelBuilding()
        {
            if (buildLabor != null)
            {
                buildLabor.Cancel();
                buildLabor = null;
            }

            for (int i = 0; i < haulLabors.Count; i++)
            {
                haulLabors[i].Cancel();
            }

            for (int i = 0; i < OccupiedTiles.Count; i++)
            {
                Tile tile = OccupiedTiles[i].Tile;
                tile.RemoveEntity();
            }
        }

        private void CalculateCenterPosition()
        {
            for (int i = 0; i < OccupiedTiles.Count; i++)
            {
                Tile tile = OccupiedTiles[i].Tile;

                if (tile.X < minX)
                    minX = tile.X;

                if (tile.X > maxX)
                    maxX = tile.X;

                if (tile.Y < minY)
                    minY = tile.Y;

                if (tile.Y > maxY)
                    maxY = tile.Y;
            }

            CenterPosition = new Vector2(
                (minX + (maxX - minX) / 2) * GameplayScene.TILE_SIZE + GameplayScene.TILE_SIZE / 2,
                (minY + (maxY - minY) / 2) * GameplayScene.TILE_SIZE + GameplayScene.TILE_SIZE / 2);
        }

        public void AddBuildingItem(Item item, int count)
        {
            for (int i = 0; i < count; i++)
                CraftingRecipe.Add(item);
        }

        public Tile GetReachableTile(SettlerCmp settler)
        {
            for (int i = 0; i < targetTiles.Count; ++i)
            {
                if (settler.Pathfinder.IsPathAvailable(targetTiles[i], !targetTiles[i].IsWalkable))
                    return targetTiles[i];
            }

            return null;
        }

        public class TileInfo
        {
            public Tile Tile { get; set; }
            public bool IsWalkable { get; private set; }
            public bool IsTarget { get; private set; }

            public TileInfo(bool walkable, bool target)
            {
                IsWalkable = walkable;
                IsTarget = target;
            }

        }
    }
}

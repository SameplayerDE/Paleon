using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Paleon
{
    public class WorldManager
    {
        public WorldTimer WorldTimer { get; private set; }
       
        public Color TimeOfDayColor { get; private set; }

        public StorageManager StorageManager { get; private set; }
        public LaborManager LaborManager { get; private set; }

        private MyAction currentAction;

        private Tile firstSelectedTile;

        private Tile[,] selectedTiles;

        private BuildingCmp currentBuilding;

        private List<CrafterBuildingCmp> crafterBuildings;

        private const int MIN_AREA_WIDTH = 2;
        private const int MIN_AREA_HEIGHT = 2;

        private List<FarmArea> farmAreas;
        public List<WaterSource> WaterAreas;

        private Image mouseSelector;
        private Selector selector;
        private AreaSelector areaSelector;

        private SelectableCmp selectedEntity;

        public WorldManager()
        {
            WorldTimer = new WorldTimer();
            StorageManager = new StorageManager();
            LaborManager = new LaborManager();

            crafterBuildings = new List<CrafterBuildingCmp>();

            farmAreas = new List<FarmArea>();
            WaterAreas = new List<WaterSource>();

            mouseSelector = new Image(TextureBank.UITexture.GetSubtexture(176, 0, 16, 16));
            selector = new Selector();
            areaSelector = new AreaSelector(Color.LightBlue);
        }

        public void Begin()
        {
            WorldTimer.Begin();
        }

        public void Update()
        {
            WorldTimer.Update();
            TimeOfDayColor = DayColorChanger.GetTimeOfDayColor(WorldTimer.CurrentTimeInDegrees);
            LaborManager.Update(GameplayScene.Settlers);

            if (!GameplayScene.MouseOnUI)
            {
                switch (currentAction)
                {
                    case MyAction.STOCKPILE:
                    case MyAction.FARM:
                    case MyAction.WATER_AREA:
                        DoAreaSelectionAction();
                        break;
                    case MyAction.BUILD:
                        DoBuildAction();
                        break;
                    case MyAction.CHOP:
                    case MyAction.GATHER:
                    case MyAction.MINE:
                    case MyAction.HAUL:
                        mouseSelector.Position = GameplayScene.MouseTilePosition;
                        DoLaborAction(currentAction);
                        break;
                    case MyAction.NONE:
                        DoNoneAction();
                        break;
                }
            }
        }

        public void Render()
        {
            // Render stockpile and farmland
            for (int i = 0; i < farmAreas.Count; ++i)
                farmAreas[i].Render();

            for (int i = 0; i < StorageManager.Stockpiles.Count; i++)
                StorageManager.Stockpiles[i].Render();

            for (int i = 0; i < WaterAreas.Count; i++)
                WaterAreas[i].Render();

            switch (currentAction)
            {
                case MyAction.BUILD:
                    if (currentBuilding != null)
                    {
                        int x = GameplayScene.MouseTile.X;
                        int y = GameplayScene.MouseTile.Y;
                        for (int i = x; i < x + currentBuilding.Width; i++)
                        {
                            for (int j = y; j < y + currentBuilding.Height; j++)
                            {
                                Tile tile = GameplayScene.GetTile(i, j);
                                Color color = new Color(116, 240, 19);

                                if (tile == null ||
                                    tile.ItemContainers.Count > 0 ||
                                    tile.BlockType != BlockType.NONE ||
                                    tile.Entity != null ||
                                    tile.Area != null ||
                                    tile.IsWalkable == false)
                                {
                                    color = Color.Red;
                                }

                                RenderManager.HollowRect(i * GameplayScene.TILE_SIZE, j * GameplayScene.TILE_SIZE,
                                    GameplayScene.TILE_SIZE, GameplayScene.TILE_SIZE, color);
                            }
                        }
                    }
                    break;
                case MyAction.CHOP:
                case MyAction.MINE:
                case MyAction.GATHER:
                case MyAction.HAUL:
                    {
                        // Выделять тайл под мышью только когда зона не выделена
                        if (areaSelector.Active == false)
                            mouseSelector.Render();

                        areaSelector.Render();
                    }
                    break;
                default:
                    {
                        // Render area selections
                        if (selectedTiles != null)
                            RenderSelectedTiles(AreaSelectionCondition);

                        if (selectedEntity != null)
                            selector.Render(selectedEntity.BoundingBox, Color.Orange);
                    }
                    break;

            }
        }

        private bool AreaSelectionCondition(Tile tile)
        {
            switch(currentAction)
            {
                case MyAction.WATER_AREA:
                    return tile.Area == null;
                case MyAction.FARM:
                case MyAction.STOCKPILE:
                    return tile.Area == null && tile.IsWalkable && tile.Entity == null;
            }

            return false;
        }

        public void SetMyAction(MyAction myAction)
        {
            selectedEntity = null;

            currentAction = myAction;
        }

        public void SetBuilding(BuildingCmp building)
        {
            currentBuilding = building;
        }

        private void DoAreaSelectionAction()
        {
            if (MInput.Mouse.PressedLeftButton)
            {
                firstSelectedTile = GameplayScene.MouseTile;
            }

            if(firstSelectedTile != null && MInput.Mouse.CheckLeftButton)
                selectedTiles = SelectTiles(firstSelectedTile, GameplayScene.MouseTile);

            if (firstSelectedTile != null && MInput.Mouse.ReleasedLeftButton)
            {
                List<Tile> areaTiles = new List<Tile>();
                bool valid = true;

                int width = selectedTiles.GetLength(0);
                int height = selectedTiles.GetLength(1);

                if (width <= MIN_AREA_WIDTH || height <= MIN_AREA_HEIGHT)
                    valid = false;
                else
                {
                    for (int i = 0; i < width; i++)
                    {
                        for (int j = 0; j < height; j++)
                        {
                            Tile tile = selectedTiles[i, j];
                            if (AreaSelectionCondition(tile))
                            {
                                areaTiles.Add(tile);
                            }
                            else
                            {
                                valid = false;
                                break;
                            }
                        }

                        if (!valid)
                            break;
                    }
                }

                if(valid)
                {
                    Area newArea = null;

                    switch(currentAction)
                    {
                        case MyAction.STOCKPILE:
                            newArea = StorageManager.CreateStockpile();
                            break;
                        case MyAction.FARM:
                            newArea = new FarmArea();
                            farmAreas.Add(newArea as FarmArea);
                            break;
                        case MyAction.WATER_AREA:
                            newArea = new WaterSource();
                            WaterAreas.Add(newArea as WaterSource);
                            break;
                    }
                    newArea.AddTiles(areaTiles);
                }

                firstSelectedTile = null;
                selectedTiles = null;
            }
        }

        private void DoLaborAction(MyAction action)
        {
            if (MInput.Mouse.PressedLeftButton)
            {
                areaSelector.Active = true;

                firstSelectedTile = GameplayScene.MouseTile;
            }

            if (firstSelectedTile != null && MInput.Mouse.CheckLeftButton)
            {
                SelectRect(firstSelectedTile, GameplayScene.MouseTile);
            }

            if (firstSelectedTile != null && MInput.Mouse.ReleasedLeftButton)
            {
                areaSelector.Active = false;

                Tile[,] tiles = SelectTiles(firstSelectedTile, GameplayScene.MouseTile);

                switch (action)
                {
                    case MyAction.CHOP:
                        for (int x = 0; x < tiles.GetLength(0); x++)
                        {
                            for (int y = 0; y < tiles.GetLength(1); y++)
                            {
                                Tile tile = tiles[x, y];
                                if(tile.Entity != null && tile.Entity.Has<SaplingCmp>())
                                {
                                    LaborManager.Add(new ChopLabor(tile));
                                }
                            }
                        }
                        break;
                    case MyAction.MINE:
                        for (int x = 0; x < tiles.GetLength(0); x++)
                        {
                            for (int y = 0; y < tiles.GetLength(1); y++)
                            {
                                Tile tile = tiles[x, y];
                                if (tile.Block != null && tile.Block.MineLabor == null)
                                {
                                    MineLabor mineLabor = new MineLabor(tile);
                                    tile.Block.MineLabor = mineLabor;
                                    LaborManager.Add(mineLabor);
                                }
                            }
                        }
                        break;
                    case MyAction.GATHER:
                        for (int x = 0; x < tiles.GetLength(0); x++)
                        {
                            for (int y = 0; y < tiles.GetLength(1); y++)
                            {
                                Tile tile = tiles[x, y];
                                if (tile.Entity != null)
                                {
                                    SaplingCmp plant = tile.Entity.Get<SaplingCmp>();
                                    if(plant != null && plant.Fruit != null)
                                    {
                                        plant.AutoHarvest = true;
                                    }
                                }
                            }
                        }
                        break;
                    case MyAction.HAUL:
                        for (int x = 0; x < tiles.GetLength(0); x++)
                        {
                            for (int y = 0; y < tiles.GetLength(1); y++)
                            {
                                Tile tile = tiles[x, y];
                                // Помечаем контейнеры (которые не были помечены) на перенос
                                foreach (var ic in tile.ItemContainers)
                                {
                                    if (ic.Labor == null)
                                    {
                                        LaborManager.Add(new HaulLabor(ic, false));
                                    }
                                }
                            }
                        }
                        break;
                }
            }
        }

        private void DoBuildAction()
        {
            if(currentBuilding != null)
            {
                if (MInput.Mouse.PressedLeftButton)
                {
                    int x = GameplayScene.MouseTile.X;
                    int y = GameplayScene.MouseTile.Y;

                    Tile[,] tiles = new Tile[currentBuilding.Width, currentBuilding.Height];

                    bool isValid = true;

                    for (int i = x; i < x + currentBuilding.Width; i++)
                    {
                        for (int j = y; j < y + currentBuilding.Height; j++)
                        {
                            Tile tile = GameplayScene.GetTile(i, j);

                            if (tile == null || tile.ItemContainers.Count > 0 ||
                                tile.BlockType != BlockType.NONE || tile.Entity != null ||
                                tile.Area!= null || tile.IsWalkable == false)
                            {
                                isValid = false;
                                break;
                            }
                            else
                            {
                                tiles[i - x, j - y] = tile;
                            }
                        }

                        if (!isValid)
                            break;
                    }

                    if (isValid)
                    {
                        Entity entity = new Entity(GameplayScene.Instance);
                        BuildingCmp newBuilding = (BuildingCmp)Activator.CreateInstance(currentBuilding.GetType());
                        
                        entity.Add(newBuilding);
                        entity.Add(new SelectableCmp(0, 0, newBuilding.Width * GameplayScene.TILE_SIZE, newBuilding.Height * GameplayScene.TILE_SIZE));
                       
                        newBuilding.Entity.X = x * GameplayScene.TILE_SIZE;
                        newBuilding.Entity.Y = y * GameplayScene.TILE_SIZE;

                        newBuilding.SetToBuild(tiles);

                        GameplayScene.AddEntity(newBuilding.Entity);

                        if (newBuilding is CrafterBuildingCmp)
                            crafterBuildings.Add(newBuilding as CrafterBuildingCmp);
                        else if (newBuilding is StorageBuildingCmp)
                            StorageManager.AddStorage(newBuilding as StorageHutCmp);
                    }
                }
            }
        }

        private void DoNoneAction()
        {
            if (selectedTiles != null)
                selectedTiles = null;

            if (MInput.Mouse.PressedLeftButton)
            {
                // Check settlers here
                for (int i = 0; i < GameplayScene.Settlers.Count; i++)
                {
                    SettlerCmp settler = GameplayScene.Settlers[i];
                    SelectableCmp selectable = settler.Entity.Get<SelectableCmp>();
                    if (selectable.Intersects((int)GameplayScene.MouseWorldPosition.X, (int)GameplayScene.MouseWorldPosition.Y))
                    {
                        selectedEntity = selectable;
                        GameplayScene.UIRenderer.OpenSettlerUI(settler);
                        return;
                    }
                }

                // Check animals here
                for (int i = 0; i < GameplayScene.Animals.Count; i++)
                {
                    AnimalCmp animal = GameplayScene.Animals[i];
                    SelectableCmp selectable = animal.Entity.Get<SelectableCmp>();
                    if (selectable.Intersects((int)GameplayScene.MouseWorldPosition.X, (int)GameplayScene.MouseWorldPosition.Y))
                    {
                        selectedEntity = selectable;
                        // TODO: open UI
                        return;
                    }
                }

                // Check buildings and vegetables here
                Tile tile = GameplayScene.MouseTile;
                if (tile.Entity != null && tile.Entity.Has<SelectableCmp>())
                {
                    selectedEntity = tile.Entity.Get<SelectableCmp>();

                    if (tile.Entity.Has<BuildingCmp>())
                    {
                        GameplayScene.UIRenderer.OpenBuildingUI(tile.Entity.Get<BuildingCmp>());
                    }
                    else if(tile.Entity.Has<PlantCmp>())
                    {
                        GameplayScene.UIRenderer.OpenPlantUI(tile.Entity.Get<PlantCmp>());
                    }
                    return;
                }

                // Check areas
                if (tile.Area != null)
                {
                    GameplayScene.UIRenderer.OpenAreaUI(tile.Area);
                    return;

                }

                selectedEntity = null;
                GameplayScene.UIRenderer.Close();
            }
        }


        private Tile[,] SelectTiles(Tile firstTile, Tile lastTile)
        {
            int firstX = firstTile.X;
            int firstY = firstTile.Y;

            int lastX = lastTile.X;
            int lastY = lastTile.Y;

            if (firstX > lastX)
                MathUtils.Replace(ref firstX, ref lastX);

            if (firstY > lastY)
                MathUtils.Replace(ref firstY, ref lastY);

            Tile[,] tiles = new Tile[(lastX + 1) - firstX, (lastY + 1) - firstY];

            for (int x = firstX; x < lastX + 1; x++)
            {
                for (int y = firstY; y < lastY + 1; y++)
                {
                    tiles[x - firstX, y - firstY] = GameplayScene.GetTile(x, y);
                }
            }

            return tiles;
        }

        private void SelectRect(Tile firstTile, Tile lastTile)
        {
            int firstX = firstTile.X;
            int firstY = firstTile.Y;

            int lastX = lastTile.X;
            int lastY = lastTile.Y;

            if (firstX > lastX)
                MathUtils.Replace(ref firstX, ref lastX);

            if (firstY > lastY)
                MathUtils.Replace(ref firstY, ref lastY);

            areaSelector.SetRect(firstX, firstY, lastX - firstX + 1, lastY - firstY + 1, GameplayScene.TILE_SIZE);
        }

        private void RenderSelectedTiles(Func<Tile, bool> condition)
        {
            Tile firstTile = selectedTiles[0, 0];
            Tile lastTile = selectedTiles[selectedTiles.GetLength(0) - 1, selectedTiles.GetLength(1) - 1];

            int width = selectedTiles.GetLength(0);
            int height = selectedTiles.GetLength(1);

            for (int x = firstTile.X; x < lastTile.X + 1; x++)
            {
                for (int y = firstTile.Y; y < lastTile.Y + 1; y++)
                {
                    if (condition(selectedTiles[x - firstTile.X, y - firstTile.Y]) && width > MIN_AREA_WIDTH && height > MIN_AREA_HEIGHT)
                        RenderManager.HollowRect(x * GameplayScene.TILE_SIZE, y * GameplayScene.TILE_SIZE, GameplayScene.TILE_SIZE, GameplayScene.TILE_SIZE, new Color(116, 240, 19));
                    else
                        RenderManager.HollowRect(x * GameplayScene.TILE_SIZE, y * GameplayScene.TILE_SIZE, GameplayScene.TILE_SIZE, GameplayScene.TILE_SIZE, Color.Red);
                }
            }
        }

    }
}

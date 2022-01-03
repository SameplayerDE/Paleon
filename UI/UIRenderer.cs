using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Paleon
{
    public enum CursorType
    {
        DEFAULT,
        BUILDING
    }

    public class UIRenderer
    {
        public ActionPanel ActionPanel { get; private set; }
        public BuildingPanel BuildingPanel { get; private set; }
        public TimeControllerUI TimeControllerUI { get; private set; }
        public CrafterUI CrafterUI { get; private set; }
        public BuildingUI BuildingUI { get; private set; }
        public PlantUI PlantUI { get; private set; }
        public HutUI HutUI { get; private set; }
        public StorageUI StorageUI { get; private set; }
        public SettlerUI SettlerUI { get; private set; }
        public TechnologyTreeUI TechnologyTreeUI { get; private set; }
        public LaborPriorityUI LaborPriorityUI { get; private set; }
        public FarmAreaUI FarmingAreaUI { get; private set; }
        public AreaUI<Stockpile> StockpileUI { get; private set; }

        private ButtonUI techTreeButton;
        private ButtonUI laborPriorityButton;
        private ButtonUI worldMapButton;

        private UI currentUI;

        private Image cursor;
        private MyTexture defaultCursor;
        private MyTexture buildingCursor;

        private static ImageUI popUpBackground;
        private static PixelTextUI popUpText;
        private static bool showPopUp = false;

        public static Color SelectedColor { get; set; } = Color.Orange;
        public static Color DefaultColor { get; set; } = new Color(150, 150, 150, 255);
        public static Color HoveredColor { get; set; } = Color.White;

        public UIRenderer()
        {
            defaultCursor = TextureBank.UITexture.GetSubtexture(128, 32, 16, 16);
            buildingCursor = TextureBank.UITexture.GetSubtexture(144, 32, 16, 16);

            cursor = new Image(defaultCursor, 32, 32);

            ActionPanel = new ActionPanel();
            BuildingPanel = new BuildingPanel(false);
            TimeControllerUI = new TimeControllerUI(GameplayScene.WorldManager.WorldTimer);
            CrafterUI = new CrafterUI();
            BuildingUI = new BuildingUI();
            PlantUI = new PlantUI();
            FarmingAreaUI = new FarmAreaUI();
            FarmingAreaUI.AddItem(ItemDatabase.OAT_SEEDS);
            FarmingAreaUI.AddItem(ItemDatabase.BARLEY_SEEDS);

            StockpileUI = new AreaUI<Stockpile>();

            HutUI = new HutUI();
            for (int i = 0; i < GameplayScene.Settlers.Count; i++)
                HutUI.AddSettler(GameplayScene.Settlers[i]);

            StorageUI = new StorageUI();

            SettlerUI = new SettlerUI();
            TechnologyTreeUI = new TechnologyTreeUI();
            TechnologyTreeUI.AddTechnology("Fishing", 10, TextureBank.UITexture.GetSubtexture(32, 0, 16, 16), 0, 0);
            TechnologyTreeUI.AddTechnology("Grain processing", 25, TextureBank.UITexture.GetSubtexture(240, 0, 16, 16), 0, 2);
            TechnologyTreeUI.AddTechnology("Pottery", 25, TextureBank.UITexture.GetSubtexture(224, 16, 16, 16), 0, 4);
            TechnologyTreeUI.AddTechnology("Pottery", 25, TextureBank.UITexture.GetSubtexture(224, 16, 16, 16), 0, 6);

            LaborPriorityUI = new LaborPriorityUI();

            for (int i = 0; i < GameplayScene.Settlers.Count; i++)
                LaborPriorityUI.AddSettler(GameplayScene.Settlers[i]);

            LaborPriorityUI.AddLabor(LaborType.Miner, TextureBank.UITexture.GetSubtexture(128, 0, 16, 16));
            LaborPriorityUI.AddLabor(LaborType.Woodcutter, TextureBank.UITexture.GetSubtexture(16, 0, 16, 16));
            LaborPriorityUI.AddLabor(LaborType.Farmer, TextureBank.UITexture.GetSubtexture(144, 0, 16, 16));
            LaborPriorityUI.AddLabor(LaborType.Fisher, TextureBank.UITexture.GetSubtexture(208, 0, 16, 16));
            LaborPriorityUI.AddLabor(LaborType.Builder, TextureBank.UITexture.GetSubtexture(160, 0, 16, 16));
            LaborPriorityUI.AddLabor(LaborType.Porter, TextureBank.UITexture.GetSubtexture(96, 0, 16, 16));
            LaborPriorityUI.AddLabor(LaborType.Cook, TextureBank.UITexture.GetSubtexture(96, 16, 16, 16));
            LaborPriorityUI.AddLabor(LaborType.Crafter, TextureBank.UITexture.GetSubtexture(112, 16, 16, 16));

            techTreeButton = new ButtonUI(TextureBank.ButtonTexture, TextureBank.UITexture.GetSubtexture(224, 0, 16, 16), 48, 48, 32, 32);
            techTreeButton.X = Engine.Width - (techTreeButton.Width + 5);
            techTreeButton.Y = 5;
            techTreeButton.HoveredColor = HoveredColor;
            techTreeButton.DefaultColor = DefaultColor;
            techTreeButton.SelectedColor = SelectedColor;
            techTreeButton.Selectable = false;
            techTreeButton.AddOnPressedCallback(OpenTechnologyTree);

            laborPriorityButton = new ButtonUI(TextureBank.ButtonTexture, TextureBank.UITexture.GetSubtexture(240, 16, 16, 16), 48, 48, 32, 32);
            laborPriorityButton.X = techTreeButton.X - (laborPriorityButton.Width + 5);
            laborPriorityButton.Y = 5;
            laborPriorityButton.HoveredColor = HoveredColor;
            laborPriorityButton.DefaultColor = DefaultColor;
            laborPriorityButton.SelectedColor = SelectedColor;
            laborPriorityButton.Selectable = false;
            laborPriorityButton.AddOnPressedCallback(OpenLaborPriority);

            worldMapButton = new ButtonUI(TextureBank.ButtonTexture, TextureBank.UITexture.GetSubtexture(192, 16, 16, 16), 48, 48, 32, 32);
            worldMapButton.X = laborPriorityButton.X - (worldMapButton.Width + 5);
            worldMapButton.Y = 5;
            worldMapButton.HoveredColor = HoveredColor;
            worldMapButton.DefaultColor = DefaultColor;
            worldMapButton.SelectedColor = SelectedColor;
            worldMapButton.Selectable = true;

            ActionPanel.GetButton(MyAction.BUILD).AddOnSelectionChangedCallback(OpenBuildingPanel);

            popUpBackground = new ImageUI(RenderManager.Pixel, 1, 1);
            popUpBackground.Color = new Color(0, 0, 0, 220);
            popUpText = new PixelTextUI(RenderManager.PixelFont, "A", Color.White);
        }

        public void Update()
        {
            ActionPanel.Update();
            BuildingPanel.Update();

            techTreeButton.Update();
            laborPriorityButton.Update();
            worldMapButton.Update();

            if (techTreeButton.Intersects(MInput.Mouse.X, MInput.Mouse.Y) || 
                laborPriorityButton.Intersects(MInput.Mouse.X, MInput.Mouse.Y))
                GameplayScene.MouseOnUI = true;

            TimeControllerUI.Update();

            if (MInput.Mouse.PressedRightButton)
            {
                techTreeButton.Selected = false;

                Close();
            }

            if (currentUI != null)
                currentUI.Update();

            cursor.X = MInput.Mouse.X;
            cursor.Y = MInput.Mouse.Y;

            if (showPopUp)
            {
                popUpBackground.X = MInput.Mouse.X + 16;
                popUpBackground.Y = MInput.Mouse.Y - 32;

                popUpText.X = popUpBackground.X + 5;
                popUpText.Y = popUpBackground.Y + 5;
            }
        }

        public void Render()
        {
            RenderManager.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp);

            BuildingPanel.Render();
            ActionPanel.Render();
            TimeControllerUI.Render();
            techTreeButton.Render();
            laborPriorityButton.Render();
            worldMapButton.Render();

            if (currentUI != null)
                currentUI.Render();

            cursor.Render();

            if (showPopUp)
            {
                popUpBackground.Render();
                popUpText.Render();
                showPopUp = false;
            }

            RenderManager.SpriteBatch.End();
        }

        private void OpenBuildingPanel(ButtonUI button)
        {
            BuildingPanel.Active = button.Selected;

            if (!button.Selected)
                SetCursor(CursorType.DEFAULT);
            else
                SetCursor(CursorType.BUILDING);
        }

        private void OpenTechnologyTree(ButtonUI button)
        {
            Close();

            if (button.Selected)
            {
                currentUI = TechnologyTreeUI;
                currentUI.Open(null);
            }
        }

        private void OpenLaborPriority(ButtonUI button)
        {
            Close();

            if(button.Selected)
            {
                currentUI = LaborPriorityUI;
                currentUI.Open(null);
            }
        }

        public void OpenAreaUI(Area area)
        {
            Close();

            if (area is FarmArea)
            {
                currentUI = FarmingAreaUI;
                currentUI.Open(area);
            }
            else if (area is Stockpile)
            {
                currentUI = StockpileUI;
                currentUI.Open(area);
            }
        }

        public void OpenBuildingUI(BuildingCmp building)
        {
            Close();

            if (!building.Completed)
            {
                currentUI = BuildingUI;
            }
            else
            {
                if (building is CrafterBuildingCmp)
                    currentUI = CrafterUI;
                else if (building is HutCmp)
                    currentUI = HutUI;
                else if (building is StorageBuildingCmp)
                    currentUI = StorageUI;
            }

            currentUI.Open(building);
        }

        public void OpenPlantUI(PlantCmp plant)
        {
            Close();

            currentUI = PlantUI;
            currentUI.Open(plant);
        }

        public void OpenSettlerUI(SettlerCmp settler)
        {
            Close();

            currentUI = SettlerUI;
            currentUI.Open(settler);
        }

        public void Close()
        {
            if (currentUI != null)
            {
                currentUI.Close();
                currentUI = null;
            }
        }

        public void SetCursor(CursorType type)
        {
            switch(type)
            {
                case CursorType.DEFAULT:
                    cursor.Texture = defaultCursor;
                    break;
                case CursorType.BUILDING:
                    cursor.Texture = buildingCursor;
                    break;
            }
        }

        public void ShowPopUp(string text)
        {
            showPopUp = true;

            popUpText.Text = text;
            popUpBackground.Width = popUpText.Width + 5 * 2;
            popUpBackground.Height = popUpText.Height + 5 * 2;
        }

    }
}

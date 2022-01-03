using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paleon
{
    public class PlantUI : UI
    {
        private PanelUI panel;

        private PixelTextUI nameText;

        private PixelTextUI growthProgressText;

        private SaplingCmp plant;

        private ButtonUI harvestBtn;
        private ButtonUI chopBtn;

        public PlantUI()
        {
            panel = new PanelUI();
            panel.Color = UIRenderer.DefaultColor;

            panel.InnerWidth = 256;
            panel.InnerHeight = 365;
            panel.X = Engine.Width - panel.Width - 5;
            panel.Y = Engine.Height - panel.Height - 5;

            nameText = new PixelTextUI(RenderManager.PixelFont, "", Color.White);

            growthProgressText = new PixelTextUI(RenderManager.PixelFont, "", Color.White);

            AddActionButtons();

            UpdatePositions();
        }

        private void AddActionButtons()
        {
            // *** Harvest ***
            harvestBtn = new ButtonUI(TextureBank.ButtonTexture, TextureBank.UITexture.GetSubtexture(64, 0, 16, 16), 48, 48, 32, 32);
            harvestBtn.Name = "Harvest";
            harvestBtn.DefaultColor = UIRenderer.DefaultColor;
            harvestBtn.HoveredColor = UIRenderer.HoveredColor;
            harvestBtn.SelectedColor = UIRenderer.SelectedColor;
            harvestBtn.Selectable = true;

            harvestBtn.X = panel.InnerX;
            harvestBtn.Y = (panel.InnerY + panel.InnerHeight) - harvestBtn.Height;

            harvestBtn.AddOnPressedCallback(SetPlantLabor);

            // *** Chop button ***
            chopBtn = new ButtonUI(TextureBank.ButtonTexture, TextureBank.UITexture.GetSubtexture(16, 0, 16, 16), 48, 48, 32, 32);
            chopBtn.Name = "Chop";
            chopBtn.DefaultColor = UIRenderer.DefaultColor;
            chopBtn.HoveredColor = UIRenderer.HoveredColor;
            chopBtn.SelectedColor = UIRenderer.SelectedColor;
            chopBtn.Selectable = true;

            chopBtn.X = harvestBtn.X + harvestBtn.Width + 5;
            chopBtn.Y = harvestBtn.Y;

            chopBtn.AddOnPressedCallback(SetPlantLabor);
        }

        private void UpdatePositions()
        {
            nameText.X = panel.InnerX;
            nameText.Y = panel.InnerY;

            growthProgressText.X = panel.InnerX;
            growthProgressText.Y = nameText.Y + nameText.Height + 5;
        }

        public override void Update()
        {
            int x = MInput.Mouse.X;
            int y = MInput.Mouse.Y;
            if (panel.Intersects(x, y))
            {
                GameplayScene.MouseOnUI = true;

                growthProgressText.Text = (int)plant.GrowingProgress + " %";

                harvestBtn.Update();
                chopBtn.Update();
            }
        }

        public override void Render()
        {
            panel.Render();

            nameText.RenderShadow(2);

            growthProgressText.RenderShadow(2);

            harvestBtn.Render();
            chopBtn.Render();
        }

        public override void Open(object obj)
        {
            if (obj is SaplingCmp)
                plant = (SaplingCmp)obj;
            else
                throw new Exception("Incorrect input data!");

            nameText.Text = plant.Name;
            growthProgressText.Text = plant.GrowingProgress + " %";

            UpdatePlantLabors(plant);
        }

        private void UpdatePlantLabors(SaplingCmp plant)
        {
            harvestBtn.Selected = plant.AutoHarvest;
            chopBtn.Selected = plant.Chop;
        }

        private void SetPlantLabor(ButtonUI button)
        {
            if(button.Name == "Harvest")
                plant.AutoHarvest = button.Selected;
            else if(button.Name == "Chop")
                plant.Chop = button.Selected;

            UpdatePlantLabors(plant);
        }

        public override void Close()
        {
            plant = null;
        }
    }
}

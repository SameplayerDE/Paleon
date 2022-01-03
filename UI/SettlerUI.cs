using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paleon
{
    public class SettlerUI : UI
    {

        private PanelUI panel;

        private PixelTextUI nameText;

        private ImageUI satietyImage;
        private PixelTextUI satietyText;

        private ImageUI thirstImage;
        private PixelTextUI thirstText;

        private ImageUI temperatureImage;
        private PixelTextUI temperatureText;

        private ImageUI enduranceImage;
        private PixelTextUI enduranceText;

        private SettlerCmp settler;

        public SettlerUI()
        {
            panel = new PanelUI();
            panel.Color = UIRenderer.DefaultColor;

            panel.InnerWidth = 256;
            panel.InnerHeight = 365;
            panel.X = Engine.Width - panel.Width - 5;
            panel.Y = Engine.Height - panel.Height - 5;

            nameText = new PixelTextUI(RenderManager.PixelFont, "", Color.White);

            satietyImage = new ImageUI(TextureBank.UITexture.GetSubtexture(112, 48, 16, 16), 32, 32);
            satietyText = new PixelTextUI(RenderManager.PixelFont, "", Color.Orange);

            thirstImage = new ImageUI(TextureBank.UITexture.GetSubtexture(128, 48, 16, 16), 32, 32);
            thirstText = new PixelTextUI(RenderManager.PixelFont, "", Color.LightBlue);

            temperatureImage = new ImageUI(TextureBank.UITexture.GetSubtexture(160, 48, 16, 16), 32, 32);
            temperatureText = new PixelTextUI(RenderManager.PixelFont, "", Color.LightPink);

            enduranceImage = new ImageUI(TextureBank.UITexture.GetSubtexture(144, 48, 16, 16), 32, 32);
            enduranceText = new PixelTextUI(RenderManager.PixelFont, "", Color.Yellow);

            UpdatePositions();
        }

        private void UpdatePositions()
        {
            nameText.X = panel.InnerX;
            nameText.Y = panel.InnerY;

            satietyImage.X = panel.InnerX;
            satietyImage.Y = nameText.Y + nameText.Height + 5;
            satietyText.X = satietyImage.X + satietyImage.Width + 5;
            satietyText.Y = satietyImage.Y;

            thirstImage.X = panel.InnerX;
            thirstImage.Y = satietyImage.Y + satietyImage.Height + 5;
            thirstText.X = thirstImage.X + thirstImage.Width + 5;
            thirstText.Y = thirstImage.Y;

            temperatureImage.X = panel.InnerX;
            temperatureImage.Y = thirstImage.Y + thirstImage.Height + 5;
            temperatureText.X = temperatureImage.X + temperatureImage.Width + 5;
            temperatureText.Y = temperatureImage.Y;

            enduranceImage.X = panel.InnerX;
            enduranceImage.Y = temperatureImage.Y + temperatureImage.Height + 5;
            enduranceText.X = enduranceImage.X + enduranceImage.Width + 5;
            enduranceText.Y = enduranceImage.Y;
        }

        public override void Update()
        {
            UpdateStats();
        }

        public override void Render()
        {
            panel.Render();

            nameText.RenderShadow(2);

            satietyImage.Render();
            satietyText.RenderShadow(2);

            thirstImage.Render();
            thirstText.RenderShadow(2);

            temperatureImage.Render();
            temperatureText.RenderShadow(2);

            enduranceImage.Render();
            enduranceText.RenderShadow(2);
        }

        public override void Open(object obj)
        {
            if (obj is SettlerCmp)
                settler = (SettlerCmp)obj;
            else
                throw new Exception("Incorrect input data!");

            nameText.Text = settler.Name;

            UpdateStats();
        }

        private void UpdateStats()
        {
            if(settler != null)
            {
                Stats stats = settler.Stats;

                satietyText.Text = $"{(int)stats.CurrentHunger * 40} kcal";
                thirstText.Text = $"{(int)stats.CurrentThirst} %";
                temperatureText.Text = $"{stats.CurrentTemperature} °C";
                enduranceText.Text = $"{100} %";
            }
        }

        public override void Close()
        {
            settler = null;
        }

    }
}

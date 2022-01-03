using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paleon
{
    public class BuildingUI : UI
    {

        private PanelUI panel;

        private BuildingCmp building;

        private PixelTextUI nameText;
        private ButtonUI cancelButton;

        public BuildingUI()
        {
            panel = new PanelUI();
            panel.Color = UIRenderer.DefaultColor;

            panel.InnerWidth = 256;
            panel.InnerHeight = 32 * 10 + 5 * (10 - 1);
            panel.X = Engine.Width - panel.Width - 5;
            panel.Y = Engine.Height - panel.Height - 5;

            nameText = new PixelTextUI(RenderManager.PixelFont, "", Color.White);
            nameText.X = panel.InnerX;
            nameText.Y = panel.InnerY;

            cancelButton = new ButtonUI(TextureBank.ButtonTexture, TextureBank.UITexture.GetSubtexture(160, 32, 16, 16), 48, 48, 32, 32);
            cancelButton.DefaultColor = UIRenderer.DefaultColor;
            cancelButton.HoveredColor = UIRenderer.HoveredColor;
            cancelButton.SelectedColor = UIRenderer.SelectedColor;

            cancelButton.X = panel.InnerX;
            cancelButton.Y = (panel.InnerY + panel.InnerHeight) - cancelButton.Height;

            cancelButton.AddOnPressedCallback(CancelBuilding);
        }

        public override void Open(object obj)
        {
            if (obj is BuildingCmp)
            {
                building = (BuildingCmp)obj;
                nameText.Text = building.Name;
            }
            else
                throw new Exception("Incorrect input data!");
        }

        private void CancelBuilding(ButtonUI button)
        {
            building.CancelBuilding();

            GameplayScene.UIRenderer.Close();
        }
        
        public override void Update()
        {
            if(panel.Intersects(MInput.Mouse.X, MInput.Mouse.Y))
            {
                cancelButton.Update();
                if(cancelButton.Hovered)
                {
                    GameplayScene.UIRenderer.ShowPopUp("Cancel building");
                }

                GameplayScene.MouseOnUI = true;
            }
        }

        public override void Render()
        {
            panel.Render();

            nameText.RenderShadow(2);
            cancelButton.Render();
        }


        public override void Close()
        {
            building = null;
        }

    }
}

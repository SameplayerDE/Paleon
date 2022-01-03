using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paleon
{
    public class AreaUI<T> : UI where T : class
    {

        private PanelUI panel;
        private PixelTextUI nameText;

        private Area area;

        private List<ButtonUI> buttons;

        public AreaUI()
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

            buttons = new List<ButtonUI>();

            ButtonUI removeAreaBtn = new ButtonUI(TextureBank.ButtonTexture, TextureBank.UITexture.GetSubtexture(160, 32, 16, 16), 48, 48, 32, 32);
            removeAreaBtn.DefaultColor = UIRenderer.DefaultColor;
            removeAreaBtn.HoveredColor = UIRenderer.HoveredColor;
            removeAreaBtn.SelectedColor = UIRenderer.SelectedColor;

            removeAreaBtn.X = panel.InnerX;
            removeAreaBtn.Y = (panel.InnerY + panel.InnerHeight) - removeAreaBtn.Height;

            removeAreaBtn.AddOnPressedCallback(RemoveArea);
            buttons.Add(removeAreaBtn);
        }

        public override void Open(object obj)
        {
            if (obj is T)
            {
                area = ((Area)obj);
                nameText.Text = area.Name;
            }
            else
                throw new Exception("Incorrect input data!");
        }


        public override void Update()
        {
            if (panel.Intersects(MInput.Mouse.X, MInput.Mouse.Y))
            {
                GameplayScene.MouseOnUI = true;

                for (int i = 0; i < buttons.Count; ++i)
                {
                    ButtonUI button = buttons[i];
                    button.Update();

                    if (button.Hovered && !string.IsNullOrEmpty(button.Name))
                        GameplayScene.UIRenderer.ShowPopUp(button.Name);
                }
            }
        }

        public override void Render()
        {
            panel.Render();
            nameText.Render();

            for (int i = 0; i < buttons.Count; ++i)
            {
                ButtonUI button = buttons[i];
                button.Render();
            }
        }

        public override void Close()
        {
            for (int i = 0; i < buttons.Count; ++i)
                buttons[i].Selected = false;

            area = null;
        }

        private void RemoveArea(ButtonUI button)
        {
            area.Remove();
            area = null;

            GameplayScene.UIRenderer.Close();
        }

    }
}

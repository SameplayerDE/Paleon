using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paleon
{

    public enum MyAction
    {
        HAUL,
        STOCKPILE,
        BUILD,
        LINK,
        FARM,
        CHOP,
        GATHER,
        MINE,
        HUNT,
        CANCEL,
        WATER_AREA,
        NONE
    }

    public class ActionPanel
    {

        private Dictionary<ButtonUI, MyAction> buttonsAction;
        private List<ButtonUI> buttons;

        private Rectangle dest;

        private const int BUTTON_SIZE = 48;
        private const int ICON_SIZE = 32;

        private const int OFFSET = 5;

        private ButtonUI lastHoveredButton;
        private ButtonUI lastSelectedButton;


        public ActionPanel()
        {
            buttons = new List<ButtonUI>();
            buttonsAction = new Dictionary<ButtonUI, MyAction>();

            dest = new Rectangle();

            MyTexture uiTexture = TextureBank.UITexture;

            AddButton(MyAction.CHOP, uiTexture.GetSubtexture(16, 0, 16, 16));
            AddButton(MyAction.MINE, uiTexture.GetSubtexture(128, 0, 16, 16));
            AddButton(MyAction.GATHER, uiTexture.GetSubtexture(64, 0, 16, 16));
            AddButton(MyAction.HAUL, uiTexture.GetSubtexture(96, 0, 16, 16));
            AddButton(MyAction.HUNT, uiTexture.GetSubtexture(112, 0, 16, 16));
            AddButton(MyAction.FARM, uiTexture.GetSubtexture(144, 0, 16, 16));
            AddButton(MyAction.WATER_AREA, uiTexture.GetSubtexture(32, 0, 16, 16));
            AddButton(MyAction.STOCKPILE, uiTexture.GetSubtexture(80, 0, 16, 16)); 
            AddButton(MyAction.BUILD, uiTexture.GetSubtexture(160, 0, 16, 16));
            AddButton(MyAction.CANCEL, uiTexture.GetSubtexture(48, 0, 16, 16));

            UpdatePositions();

            SelectButton(null);
        }

        private void AddButton(MyAction myAction, MyTexture icon)
        {
            ButtonUI button = new ButtonUI(TextureBank.ButtonTexture, icon, 
                BUTTON_SIZE, BUTTON_SIZE,
                ICON_SIZE, ICON_SIZE);

            button.Name = myAction.ToString();
            button.DefaultColor = UIRenderer.DefaultColor;
            button.HoveredColor = UIRenderer.HoveredColor;
            button.SelectedColor = UIRenderer.SelectedColor;
            button.Selectable = true;
            button.AddOnPressedCallback(SelectButton);

            buttons.Add(button);
            buttonsAction.Add(button, myAction);
        }

        public void Update()
        {
            int x = MInput.Mouse.X;
            int y = MInput.Mouse.Y;

            if (lastHoveredButton != null)
            {
                lastHoveredButton.Hovered = false;
                lastHoveredButton = null;
            }

            if (MInput.Mouse.PressedRightButton)
                SelectButton(lastSelectedButton);

            if (Intersects(x, y))
            {
                GameplayScene.MouseOnUI = true;

                for (int i = 0; i < buttons.Count; i++)
                {
                    ButtonUI button = buttons[i];
                    button.Update();

                    if (button.Hovered)
                    {
                        lastHoveredButton = button;

                        GameplayScene.UIRenderer.ShowPopUp(lastHoveredButton.Name);
                    }

                    if (button.Selected)
                        lastSelectedButton = button;
                }
            }
        }

        public void Render()
        {
            for (int i = 0; i < buttons.Count; i++)
                buttons[i].Render();
        }

        private void SelectButton(ButtonUI button)
        {
            if (lastSelectedButton != null)
            {
                lastSelectedButton.Selected = false;
            }

            // If was pressed the same button
            if (lastSelectedButton == button)
            {
                lastSelectedButton = null;
                GameplayScene.WorldManager.SetMyAction(MyAction.NONE);
            }
            else
            {
                lastSelectedButton = button;
                GameplayScene.WorldManager.SetMyAction(buttonsAction[button]);
            }
        }

        private void UpdatePositions()
        {
            int totalWidth = buttons.Count * BUTTON_SIZE + (buttons.Count - 1) * OFFSET;
            int totalHeight = BUTTON_SIZE;

            int startX = Engine.HalfWidth - totalWidth / 2;
            int startY = Engine.Height - OFFSET - totalHeight;

            for (int i = 0; i < buttons.Count; i++)
            {
                buttons[i].X = startX + i * (BUTTON_SIZE + OFFSET);
                buttons[i].Y = startY;
            }

            dest.X = startX;
            dest.Y = startY;
            dest.Width = totalWidth;
            dest.Height = totalHeight;
        }

        public bool Intersects(int x, int y)
        {
            return dest.Contains(new Point(x, y));
        }

        public ButtonUI GetButton(MyAction myAction)
        {
            foreach(var button in buttonsAction)
            {
                if (button.Value == myAction)
                    return button.Key;
            }

            return null;
        }

    }
}

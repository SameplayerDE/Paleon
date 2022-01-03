using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paleon
{
    public class BuildingPanel
    {
        public bool Active { get; set; }

        private Dictionary<ButtonUI, BuildingCmp> buttonsBuildings;
        private List<ButtonUI> buttons;

        private Rectangle dest;

        private const int BUTTON_SIZE = 80;

        private const int OFFSET = 5;

        private ButtonUI lastHoveredButton;
        private ButtonUI lastSelectedButton;

        public BuildingPanel(bool active)
        {
            Active = active;

            buttonsBuildings = new Dictionary<ButtonUI, BuildingCmp>();
            buttons = new List<ButtonUI>();

            dest = new Rectangle();

            AddButton(new BonfireCmp());
            AddButton(new FirewoodWorkshopCmp());
            AddButton(new HutCmp());
            AddButton(new MillstoneCmp());
            AddButton(new MortarCmp());
            AddButton(new DryerCmp());
            AddButton(new CraftingWorkshopCmp());
            AddButton(new StorageHutCmp());
            AddButton(new KilnCmp());

            UpdatePositions();
        }

        public void AddButton(BuildingCmp building)
        {
            ButtonUI button = new ButtonUI(TextureBank.BigButtonTexture, building.Icon,
                BUTTON_SIZE, BUTTON_SIZE, 64, 64);

            button.Name = building.Name;
            button.DefaultColor = UIRenderer.DefaultColor;
            button.HoveredColor = UIRenderer.HoveredColor;
            button.SelectedColor = UIRenderer.SelectedColor;
            button.Selectable = true;
            button.AddOnPressedCallback(SelectButton);

            buttons.Add(button);
            buttonsBuildings.Add(button, building);
        }

        public void Update()
        {
            if (Active)
            {
                int x = MInput.Mouse.X;
                int y = MInput.Mouse.Y;

                if (lastHoveredButton != null)
                {
                    lastHoveredButton.Hovered = false;
                    lastHoveredButton = null;
                }

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
        }

        public void Render()
        {
            if (Active)
            {
                for (int i = 0; i < buttons.Count; i++)
                    buttons[i].Render();
            }
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
                GameplayScene.WorldManager.SetBuilding(null);
            }
            else
            {
                lastSelectedButton = button;
                GameplayScene.WorldManager.SetBuilding(buttonsBuildings[lastSelectedButton]);
            }
        }

        private void UpdatePositions()
        {
            int totalWidth = buttons.Count * BUTTON_SIZE + (buttons.Count - 1) * OFFSET;
            int totalHeight = BUTTON_SIZE;

            int startX = Engine.HalfWidth - totalWidth / 2;
            int startY = Engine.Height - OFFSET - totalHeight - 48 - OFFSET;

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

    }
}

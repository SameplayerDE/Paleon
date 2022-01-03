using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paleon
{
    public class LaborPriorityUI : UI
    {

        private PanelUI panel;

        private Dictionary<LaborType, MyTexture> laborsIcons;
        private List<ImageUI> laborsImages;

        private List<ElementUI> elements;

        private int start = 0;
        private int limit = 6;

        private int rows = 7;
        private int columns = 10;
        private int offset = 5;

        private const int ELEMENT_SIZE = 32;

        private const int OFFSET = 10;


        public LaborPriorityUI()
        {
            panel = new PanelUI();
            panel.InnerWidth = (columns * ELEMENT_SIZE) + (columns * offset) + offset;
            panel.InnerHeight = (rows * ELEMENT_SIZE);

            panel.X = Engine.Width / 2 - panel.Width / 2;
            panel.Y = Engine.Height / 2 - panel.Height / 2;

            laborsIcons = new Dictionary<LaborType, MyTexture>();
            laborsImages = new List<ImageUI>();

            elements = new List<ElementUI>();
        }

        public void AddSettler(SettlerCmp settler)
        {
            ElementUI element = new ElementUI(settler);
            elements.Add(element);

            // TODO: add labors to new added settlers
        }

        public void AddLabor(LaborType laborType, MyTexture icon)
        {
            laborsIcons.Add(laborType, icon);

            ImageUI laborImage = new ImageUI(icon, 32, 32);
            laborImage.Name = Utils.EnumToString(laborType); 

            laborsImages.Add(laborImage);

            for (int i = 0; i < elements.Count; i++)
            {
                elements[i].AddLabor(laborType);
            }
        }

        public override void Open(object obj)
        {
            UpdatePositions();
        }

        private void UpdatePositions()
        {
            // Update position of visible elements

            for (int i = 0; i < laborsImages.Count; i++)
            {
                ImageUI image = laborsImages[i];
                image.X = panel.InnerX + 32 + OFFSET + (i * (32 + OFFSET));
                image.Y = panel.InnerY;
            }

            for (int i = start; i < limit; i++)
            {
                if (i >= elements.Count)
                    break;

                ElementUI element = elements[i];
                element.X = panel.InnerX;
                element.Y = panel.InnerY + ELEMENT_SIZE + OFFSET + (i * (ELEMENT_SIZE + OFFSET));
            }
        }

        public override void Update()
        {
            int x = MInput.Mouse.X;
            int y = MInput.Mouse.Y;

            if (panel.Intersects(x, y))
            {
                GameplayScene.MouseOnUI = true;

                // Show labor image description
                for (int i = 0; i < laborsImages.Count; i++)
                {
                    if (laborsImages[i].Intersects(x, y))
                        GameplayScene.UIRenderer.ShowPopUp(laborsImages[i].Name);
                }

                for (int i = 0; i < elements.Count; i++)
                {
                    elements[i].Update(x, y);
                }
            }
        }

        public override void Render()
        {
            panel.Render();

            for (int i = 0; i < laborsImages.Count; i++)
            {
                laborsImages[i].Render();
            }

            for(int i = 0; i < elements.Count; i++)
            {
                elements[i].Render();
            }
        }

        public override void Close()
        {

        }

        private class ElementUI
        {
            public bool Active { get; set; } = true;

            private SettlerCmp settler;
            private ImageUI settlerImage;

            private Rectangle dest;

            private Dictionary<ButtonUI, LaborType> buttons;

            private int x;
            public int X
            {
                get { return x; }
                set
                {
                    if (x == value)
                        return;

                    x = value;

                    UpdatePositions();
                }
            }

            private int y;
            public int Y
            {
                get { return y; }
                set
                {
                    if (y == value)
                        return;

                    y = value;

                    UpdatePositions();
                }
            }

            private ButtonUI lastHoveredButton;

            public ElementUI(SettlerCmp settler)
            {
                this.settler = settler;
                settlerImage = new ImageUI(this.settler.Avatar, 32, 32);

                buttons = new Dictionary<ButtonUI, LaborType>();

                dest = new Rectangle();
            }

            public void AddLabor(LaborType laborType)
            {
                ButtonUI button = new ButtonUI(TextureBank.SmallButtonTexture, TextureBank.UITexture.GetSubtexture(48, 48, 16, 16),
                            32, 32, 32, 32);

                button.DefaultColor = UIRenderer.DefaultColor;
                button.HoveredColor = UIRenderer.HoveredColor;
                button.SelectedColor = UIRenderer.SelectedColor;
                button.Selectable = true;
                button.Selected = true;
                button.AddOnSelectionChangedCallback(PerimtLabor);

                buttons.Add(button, laborType);
            }

            private void UpdatePositions()
            {
                settlerImage.X = x;
                settlerImage.Y = y;

                int count = 0;

                foreach (var item in buttons)
                {
                    ButtonUI button = item.Key;
                    button.X = (settlerImage.X + settlerImage.Width + OFFSET) +  (button.Width + OFFSET) * count;
                    button.Y = settlerImage.Y;

                    ++count;
                }

                // Получаем размер rectangle, при пересечении которой начинается проверка на нажатие по чекбоксам
                ButtonUI firstButton = buttons.First().Key;
                ButtonUI lastButton = buttons.Last().Key;

                dest.X = firstButton.X;
                dest.Y = firstButton.Y;

                dest.Width = (lastButton.X + lastButton.Width) - firstButton.X;
                dest.Height = firstButton.Height;
            }

            public void Update(int mouseX, int mouseY)
            {
                if(lastHoveredButton != null)
                {
                    lastHoveredButton.Hovered = false;
                    lastHoveredButton = null;
                }

                if (Active && Intersects(mouseX, mouseY))
                {
                    foreach (var item in buttons)
                    {
                        ButtonUI button = item.Key;

                        button.Update();

                        if (button.Hovered)
                        {
                            lastHoveredButton = button;
                        }

                        button.VisibleIcon = button.Selected;
                    }
                }
            }

            private void PerimtLabor(ButtonUI button)
            {
                LaborType laborType = buttons[button];

                settler.PermitLabor(laborType, button.Selected);
            }

            public void Render()
            {
                if (Active)
                {
                    settlerImage.Render();

                    foreach (var item in buttons)
                    {
                        item.Key.Render();
                    }
                }
            }

            public bool Intersects(int x, int y)
            {
                return dest.Contains(new Point(x, y));
            }
        }
    }
}

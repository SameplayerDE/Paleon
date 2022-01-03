using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Paleon
{
    public class FarmAreaUI : UI
    {

        private PanelUI panel;

        private FarmArea farmingArea;

        private PixelTextUI nameText;

        private List<ElementUI> elements;
        private Dictionary<Item, ElementUI> elementsDict;

        private ElementUI lastHoveredElement;
        private ElementUI lastSelectedElement;

        private List<ButtonUI> buttons;

        public FarmAreaUI()
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

            elements = new List<ElementUI>();
            elementsDict = new Dictionary<Item, ElementUI>();

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

        public void AddItem(Item item)
        {
            ElementUI element = new ElementUI(item);
            element.AddOnPressedCallback(SelectElement);
            element.SelectedColor = UIRenderer.SelectedColor * 0.5f;
            element.HoveredColor = UIRenderer.HoveredColor * 0.5f;
            elements.Add(element);

            elementsDict.Add(item, element);

            UpdatePositions();
        }

        private void SelectElement(ElementUI element)
        {
            if (element == null)
            {
                if (lastSelectedElement != null)
                {
                    lastSelectedElement.Selected = false;
                    lastSelectedElement = null;
                }
            }
            else
            {
                if (lastSelectedElement == element)
                {
                    element.Selected = false;
                    lastSelectedElement = null;

                    // Прекращаем сажать растение
                    farmingArea.Plant = null;
                }
                else
                {
                    if (lastSelectedElement != null)
                        lastSelectedElement.Selected = false;

                    element.Selected = true;
                    lastSelectedElement = element;

                    // Сажаем растение, либо заменяем на другое
                    farmingArea.Plant = element.Item;
                }
            }
        }

        private void UpdatePositions()
        {
            for (int i = 0; i < elements.Count; i++)
            {
                ElementUI element = elements[i];
                element.X = panel.InnerX;
                element.Y = (nameText.Height + 5) + (panel.InnerY + 5) + (i * (ElementUI.HEIGHT + 5));
            }
        }

        public override void Open(object obj)
        {
            if (obj is FarmArea)
            {
                farmingArea = (FarmArea)obj;
                nameText.Text = farmingArea.Name;

                if (farmingArea.Plant != null)
                    SelectElement(elementsDict[farmingArea.Plant]);
                else
                    SelectElement(null);
            }
            else
                throw new Exception("Incorrect input data!");
        }

        private void RemoveArea(ButtonUI button)
        {
            farmingArea.Remove();
            farmingArea = null;

            GameplayScene.UIRenderer.Close();
        }

        public override void Update()
        {
            if (lastHoveredElement != null)
            {
                lastHoveredElement.Hovered = false;
                lastHoveredElement = null;
            }

            if (panel.Intersects(MInput.Mouse.X, MInput.Mouse.Y))
            {
                GameplayScene.MouseOnUI = true;

                for (int i = 0; i < elements.Count; i++)
                {
                    elements[i].Update();

                    if (elements[i].Hovered)
                        lastHoveredElement = elements[i];

                    if (elements[i].Selected)
                        lastSelectedElement = elements[i];
                }

                for(int i = 0; i < buttons.Count; ++i)
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

            for (int i = 0; i < elements.Count; i++)
                elements[i].Render();
        }


        public override void Close()
        {
            if (lastSelectedElement != null)
            {
                lastSelectedElement.Selected = false;
                lastSelectedElement = null;
            }

            for (int i = 0; i < buttons.Count; ++i)
            {
                buttons[i].Selected = false;
            }

            farmingArea = null;
        }

        private class ElementUI
        {
            public const int WIDTH = 256;
            public const int HEIGHT = 32;

            public Color HoveredColor { get; set; }
            public Color SelectedColor { get; set; }

            public Item Item { get; private set; }

            public ImageUI ItemImage { get; private set; }
            public PixelTextUI ItemNameText { get; private set; }

            public bool Hovered;
            public bool Selected;

            private Action<ElementUI> cbOnPressed;

            private ImageUI backgroundImage;


            private int x;
            public int X
            {
                get { return x; }
                set
                {
                    x = value;
                    backgroundImage.X = value;
                    ItemImage.X = value;
                    ItemNameText.X = ItemImage.X + ItemImage.Width + 5;
                }
            }

            private int y;
            public int Y
            {
                get { return y; }
                set
                {
                    y = value;
                    backgroundImage.Y = value;
                    ItemImage.Y = value;
                    ItemNameText.Y = value;
                }
            }

            public ElementUI(Item item)
            {
                Item = item;
                ItemImage = new ImageUI(item.Texture, 32, 32);
                ItemNameText = new PixelTextUI(RenderManager.PixelFont, item.Name, Color.White);

                backgroundImage = new ImageUI(TextureBank.ElementBackgroundTexture, 256, 32);
                backgroundImage.Color = Color.White * 0.5f;
            }

            public void Update()
            {
                int x = MInput.Mouse.X;
                int y = MInput.Mouse.Y;

                if (Intersects(x, y))
                {
                    Hovered = true;

                    if (MInput.Mouse.PressedLeftButton)
                    {
                        Selected = !Selected;
                        cbOnPressed?.Invoke(this);
                    }
                }
                else
                {
                    Hovered = false;
                }
            }

            public void Render()
            {
                if (Selected)
                {
                    backgroundImage.Color = SelectedColor;
                    backgroundImage.Render();
                }
                else if (Hovered)
                {
                    backgroundImage.Color = HoveredColor;
                    backgroundImage.Render();
                }

                if (Selected || Hovered)
                {
                    backgroundImage.Render();
                }

                ItemImage.Render();
                ItemNameText.RenderShadow(2);
            }

            public bool Intersects(int x, int y)
            {
                return backgroundImage.Intersects(x, y);
            }

            public void AddOnPressedCallback(Action<ElementUI> callback)
            {
                cbOnPressed += callback;
            }

            public void RemoveOnPressedCallback(Action<ElementUI> callback)
            {
                cbOnPressed -= callback;
            }
        }
    }
}

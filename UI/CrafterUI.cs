using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Paleon
{
    public class CrafterUI : UI
    {

        private PanelUI panel;

        private const int ROWS = 10;
        private const int OFFSET = 5;

        private List<ElementUI> elements;
        private ButtonUI addButton;
        private ButtonUI subButton;

        private CrafterBuildingCmp crafterBuilding;

        private ElementUI lastHoveredElement;
        private ElementUI lastSelectedElement;

        public CrafterUI()
        {
            panel = new PanelUI();
            panel.Color = UIRenderer.DefaultColor;

            panel.InnerWidth = ElementUI.WIDTH;
            panel.InnerHeight = ElementUI.HEIGHT * ROWS + OFFSET * (ROWS - 1);
            panel.X = Engine.Width - panel.Width - OFFSET;
            panel.Y = Engine.Height - panel.Height - OFFSET;


            MyTexture buttonBackground = TextureBank.UITexture.GetSubtexture(16, 48, 16, 16);

            addButton = new ButtonUI(buttonBackground, TextureBank.UITexture.GetSubtexture(40, 32, 8, 8), 32, 32, 16, 16);
            addButton.DefaultColor = UIRenderer.DefaultColor;
            addButton.SelectedColor = UIRenderer.SelectedColor;

            subButton = new ButtonUI(buttonBackground, TextureBank.UITexture.GetSubtexture(32, 32, 8, 8), 32, 32, 16, 16);
            subButton.DefaultColor = UIRenderer.DefaultColor;
            subButton.SelectedColor = UIRenderer.SelectedColor;

            for (int i = 0; i < 8; i++)
                AddElement();

            UpdatePositions();
        }

        private void AddElement()
        {
            if (elements == null)
                elements = new List<ElementUI>();

            ElementUI element = new ElementUI();
            element.AddOnPressedCallback(SelectElement);
            element.SelectedColor = UIRenderer.SelectedColor * 0.5f;
            element.HoveredColor = UIRenderer.HoveredColor * 0.5f;
            elements.Add(element);
        }

        private void UpdatePositions()
        {
            for (int i = 0; i < elements.Count; i++)
            {
                ElementUI element = elements[i];
                element.X = panel.InnerX;
                element.Y = (panel.InnerY + OFFSET) + (i * (ElementUI.HEIGHT + OFFSET));
            }

            subButton.X = panel.InnerX;
            subButton.Y = panel.InnerY + panel.InnerHeight - subButton.Height - OFFSET;
            subButton.AddOnPressedCallback(SubItem);

            addButton.X = subButton.X + subButton.Width + OFFSET;
            addButton.Y = panel.InnerY + panel.InnerHeight - addButton.Height - OFFSET;
            addButton.AddOnPressedCallback(AddItem);
        }

        public override void Update()
        {
            subButton.Hovered = false;
            addButton.Hovered = false;

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

                subButton.Update();
                addButton.Update();
            }
        }

        public override void Render()
        {
            panel.Render();

            for (int i = 0; i < elements.Count; i++)
            {
                elements[i].Render();

                subButton.Render();
                addButton.Render();
            }
        }

        private void SelectElement(ElementUI element)
        {
            if (lastSelectedElement != null)
            {
                lastSelectedElement.Selected = false;
            }

            // If was pressed the same button
            if (lastSelectedElement == element)
                lastSelectedElement = null;
            else
                lastSelectedElement = element;
        }

        public override void Open(object obj)
        {
            if (obj is CrafterBuildingCmp)
                crafterBuilding = (CrafterBuildingCmp)obj;
            else
                throw new Exception("Incorrect input data!");

            crafterBuilding.AddOnCountChangedCallback(UpdateElement);

            ResetElements();

            int count = 0;
            foreach (var entry in crafterBuilding.ItemsToCraft)
            {
                ElementUI element = elements[count];
                element.CraftingRecipe = entry.Key;
                element.Count = entry.Value;
                element.Active = true;
                
                count++;
            }
        }

        public override void Close()
        {
            if (crafterBuilding != null)
            {
                crafterBuilding.RemoveOnCountChangedCallback(UpdateElement);
                crafterBuilding = null;
                if (lastSelectedElement != null)
                {
                    lastSelectedElement.Selected = false;
                    lastSelectedElement = null;
                }
            }
        }

        private void UpdateElement(CraftingRecipe item, int count)
        {
            for(int i = 0; i < elements.Count; i++)
            {
                if (elements[i].CraftingRecipe == item)
                {
                    elements[i].Count = count;
                    break;
                }
            }
        }

        private void SubItem(ButtonUI button)
        {
            if (lastSelectedElement != null)
            {
                crafterBuilding.SubItemToCraft(lastSelectedElement.CraftingRecipe);
            }
        }

        private void AddItem(ButtonUI button)
        {
            if (lastSelectedElement != null)
            {
                crafterBuilding.AddItemToCraft(lastSelectedElement.CraftingRecipe);
            }
        }

        private void ResetElements()
        {
            for (int i = 0; i < elements.Count; i++)
                elements[i].Active = false;
        }

        private class ElementUI
        {
            public const int WIDTH = 256;
            public const int HEIGHT = 32;

            public bool Hovered;
            public bool Selected;
            public bool Active;

            public Color HoveredColor { get; set; }
            public Color SelectedColor { get; set; }

            private ImageUI itemImage;
            private PixelTextUI itemNameText;
            private PixelTextUI itemCountText;

            private Action<ElementUI> cbOnPressed;

            private ImageUI backgroundImage;

            private CraftingRecipe craftingRecipe;
            public CraftingRecipe CraftingRecipe
            {
                get { return craftingRecipe; }
                set
                {
                    if (craftingRecipe == value)
                        return;

                    craftingRecipe = value;
                    itemImage.Texture = craftingRecipe.Result.Texture;
                    itemNameText.Text = craftingRecipe.Result.Name;

                    UpdatePositions();
                }
            }

            private int count = -1;
            public int Count
            {
                get { return count; }
                set
                {
                    if (count == value)
                        return;

                    count = value;
                    itemCountText.Text = "[" + count + "]";

                    UpdatePositions();
                }
            }

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

            public ElementUI()
            {
                itemImage = new ImageUI(RenderManager.Pixel, 32, 32);
                itemNameText = new PixelTextUI(RenderManager.PixelFont, "Empty", Color.White);
                itemCountText = new PixelTextUI(RenderManager.PixelFont, "[-1]", Color.White);

                backgroundImage = new ImageUI(TextureBank.ElementBackgroundTexture, 256, 32);
                backgroundImage.Color = Color.White * 0.5f;

                Active = true;
            }

            private void UpdatePositions()
            {
                backgroundImage.Y = y;
                itemImage.Y = y;
                itemNameText.Y = y;
                itemCountText.Y = y;

                backgroundImage.X = x;
                itemImage.X = x;
                itemNameText.X = itemImage.X + itemImage.Width + 5;
                itemCountText.X = itemNameText.X + itemNameText.Width + 5;
            }

            public void Update()
            {
                if (Active)
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
            }

            public void Render()
            {
                if (Active)
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

                    itemImage.Render();
                    itemNameText.RenderShadow(2);
                    itemCountText.RenderShadow(2);
                }
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

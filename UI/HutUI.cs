using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paleon
{
    public class HutUI : UI
    {

        private PanelUI panel;

        private const int ROWS = 10;
        private const int OFFSET = 5;

        private List<ElementUI> elements;
        private Dictionary<SettlerCmp, ElementUI> settlerElements;

        private HutCmp hut;
        private List<SettlerCmp> settlers;

        private ElementUI lastHoveredElement;
        private ElementUI lastSelectedElement;

        public HutUI()
        {
            panel = new PanelUI();
            panel.Color = UIRenderer.DefaultColor;

            panel.InnerWidth = ElementUI.WIDTH;
            panel.InnerHeight = ElementUI.HEIGHT * ROWS + OFFSET * (ROWS - 1);
            panel.X = Engine.Width - panel.Width - OFFSET;
            panel.Y = Engine.Height - panel.Height - OFFSET;

            elements = new List<ElementUI>();
            settlerElements = new Dictionary<SettlerCmp, ElementUI>();
        }

        public void AddSettler(SettlerCmp settler)
        {
            ElementUI element = new ElementUI(settler);
            element.AddOnPressedCallback(SelectElement);
            element.SelectedColor = UIRenderer.SelectedColor * 0.5f;
            element.HoveredColor = UIRenderer.HoveredColor * 0.5f;
            elements.Add(element);
            settlerElements.Add(settler, element);

            UpdatePositions();
        }

        private void UpdatePositions()
        {
            for (int i = 0; i < elements.Count; i++)
            {
                ElementUI element = elements[i];
                element.X = panel.InnerX;
                element.Y = (panel.InnerY + OFFSET) + (i * (ElementUI.HEIGHT + OFFSET));
            }
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
            }
        }

        public override void Render()
        {
            panel.Render();

            for (int i = 0; i < elements.Count; i++)
            {
                elements[i].Render();
            }
        }

        public override void Open(object obj)
        {
            if (obj is HutCmp)
                hut = (HutCmp)obj;
            else
                throw new Exception("Incorrect input data!");

            settlers = GameplayScene.Settlers;

            UpdateElements(hut, settlers);
        }

        public override void Close()
        {
        }

        private void SelectElement(ElementUI element)
        {
            if(element.Settler.Hut != hut)
            {
                if(hut.Owner != null)
                {
                    hut.Owner.Hut = null;
                    hut.Owner = null;
                }

                element.Settler.Hut = hut;
                hut.Owner = element.Settler;
            }
            else
            {
                element.Settler.Hut.Owner = null;
                element.Settler.Hut = null;
            } 

            UpdateElements(hut, settlers);
        }

        private void UpdateElements(HutCmp hut, List<SettlerCmp> settlers)
        {
            for (int i = 0; i < settlers.Count; i++)
            {
                SettlerCmp settler = settlers[i];

                ElementUI element = settlerElements[settler];
                if (settler.Hut != null)
                {
                    element.HasHut = true;

                    element.Selected = settler.Hut == hut;
                }
                else
                {
                    element.HasHut = false;
                    element.Selected = false;
                }
            }
        }

        private class ElementUI
        {
            public const int WIDTH = 256;
            public const int HEIGHT = 32;

            public Color HoveredColor { get; set; }
            public Color SelectedColor { get; set; }

            public ImageUI AvatarImage { get; private set; }
            public PixelTextUI SettlerNameText { get; private set; }
            public SettlerCmp Settler { get; private set; }

            public bool Hovered;
            public bool Selected;

            private bool hasHut;
            public bool HasHut
            {
                get { return hasHut; }
                set
                {
                    hasHut = value;
                    hutImage.Visible = hasHut;
                }
            }

            private ImageUI hutImage;
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
                    AvatarImage.X = value;
                    SettlerNameText.X = AvatarImage.X + AvatarImage.Width + 5;
                    hutImage.X = SettlerNameText.X + SettlerNameText.Width + 5;
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
                    AvatarImage.Y = value;
                    SettlerNameText.Y = value;
                    hutImage.Y = value;
                }
            }

            public ElementUI(SettlerCmp settler)
            {
                Settler = settler;
                AvatarImage = new ImageUI(settler.Avatar, 32, 32);
                SettlerNameText = new PixelTextUI(RenderManager.PixelFont, settler.Name, Color.White);

                backgroundImage = new ImageUI(TextureBank.ElementBackgroundTexture, 256, 32);
                backgroundImage.Color = Color.White * 0.5f;

                hutImage = new ImageUI(TextureBank.UITexture.GetSubtexture(144, 16, 16, 16), 32, 32);
                hutImage.Visible = false;
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

                AvatarImage.Render();
                SettlerNameText.RenderShadow(2);
                hutImage.Render();
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

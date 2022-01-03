using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Paleon
{
    public class ButtonUI
    {
        public string Name;
        public MyTexture Back;
        public MyTexture Icon;
        public Vector2 Origin;
        public float Rotation;

        public Color DefaultColor = Color.White;
        public Color SelectedColor = Color.White;
        public Color HoveredColor = Color.White;

        public bool Selectable;

        public SpriteEffects Effects = SpriteEffects.None;

        private Rectangle dest;
        private Rectangle iconDest;

        private bool selected;
        public bool Selected
        {
            get { return selected; }
            set
            {
                if (selected == value)
                    return;

                selected = value;

                cbOnSelectionChanged?.Invoke(this);
            }
        }

        public bool Hovered;

        public bool Visible = true;
        public bool VisibleIcon = true;

        private Action<ButtonUI> cbOnPressed;
        private Action<ButtonUI> cbOnSelectionChanged;

        public int X
        {
            get { return dest.X; }
            set
            {
                dest.X = value;
                iconDest.X = (dest.X + dest.Width / 2) - (iconDest.Width / 2);
            }
        }

        public int Y
        {
            get { return dest.Y; }
            set
            {
                dest.Y = value;
                iconDest.Y = (dest.Y + dest.Height / 2) - (iconDest.Height / 2);
            }
        }

        public int Width
        {
            get { return dest.Width; }
            set { dest.Width = value; }
        }

        public int Height
        {
            get { return dest.Height; }
            set { dest.Height = value; }
        }

        public ButtonUI(MyTexture back, MyTexture icon, int width, int height, int iconWidth, int iconHeight)
        {
            Back = back;
            Icon = icon;
            dest = new Rectangle(0, 0, width, height);
            iconDest = new Rectangle(0, 0, iconWidth, iconHeight);

            X = 0;
            Y = 0;
        }

        public void Update()
        {
            int x = MInput.Mouse.X;
            int y = MInput.Mouse.Y;

            if(Intersects(x, y))
            {
                Hovered = true;

                if(MInput.Mouse.PressedLeftButton)
                {
                    Selected = !Selected;
                    cbOnPressed?.Invoke(this);
                }

                if (!Selectable && MInput.Mouse.ReleasedLeftButton)
                {
                    Selected = false;
                }
            }
            else
            {
                Hovered = false;
            }
        }

        public void Render()
        {
            if (Visible)
            {
                if (Selected)
                {
                    Back.Draw(dest, Origin, SelectedColor, Rotation, Effects);

                    if(VisibleIcon)
                        Icon?.Draw(iconDest, Origin, SelectedColor, Rotation, Effects);
                }
                else if (Hovered)
                {
                    Back.Draw(dest, Origin, HoveredColor, Rotation, Effects);

                    if (VisibleIcon)
                        Icon?.Draw(iconDest, Origin, HoveredColor, Rotation, Effects);
                }
                else
                {
                    Back.Draw(dest, Origin, DefaultColor, Rotation, Effects);

                    if (VisibleIcon)
                        Icon?.Draw(iconDest, Origin, DefaultColor, Rotation, Effects);
                }
            }
        }

        public bool Intersects(int x, int y)
        {
            return dest.Contains(new Point(x, y));
        }

        public void AddOnPressedCallback(Action<ButtonUI> callback)
        {
            cbOnPressed += callback;
        }

        public void RemoveOnPressedCallback(Action<ButtonUI> callback)
        {
            cbOnPressed -= callback;
        }

        public void AddOnSelectionChangedCallback(Action<ButtonUI> callback)
        {
            cbOnSelectionChanged += callback;
        }

        public void RemoveOnSelectionChangedCallback(Action<ButtonUI> callback)
        {
            cbOnSelectionChanged -= callback;
        }

    }
}

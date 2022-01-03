using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paleon
{
    public class ImageUI
    {
        public string Name;
        public MyTexture Texture;
        public Vector2 Origin;
        public float Rotation;
        public Color Color = Color.White;

        private Rectangle dest;

        public bool Pressed;
        public bool Hovered;

        public bool Visible = true;

        public SpriteEffects Effects = SpriteEffects.None;

        public int X
        {
            get { return dest.X; }
            set { dest.X = value; }
        }

        public int Y
        {
            get { return dest.Y; }
            set { dest.Y = value; }
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

        public ImageUI(MyTexture texture, int width, int height)
        {
            Texture = texture;
            dest = new Rectangle(0, 0, width, height);
            
            X = 0;
            Y = 0;
        }

        public ImageUI CenterOrigin()
        {
            Origin.X = Texture.Width / 2f;
            Origin.Y = Texture.Height / 2f;
            return this;
        }

        public void Render()
        {
            if (Visible)
            {
                Texture.Draw(dest, Origin, Color, Rotation, Effects);
            }
        }

        public bool Intersects(int x, int y)
        {
            return dest.Contains(new Point(x, y));
        }
    }
}

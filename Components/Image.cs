using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paleon
{

    public class Image : GraphicsComponent
    {
        public MyTexture Texture;

        private Rectangle dest;

        public virtual int Width
        {
            get { return dest.Width; }
            set { dest.Width = value; }
        }
        public virtual int Height
        {
            get { return dest.Height; }
            set { dest.Height = value; }
        }

        public Image(MyTexture texture)
            : base(false)
        {
            Texture = texture;
            dest.Width = texture.Width;
            dest.Height = texture.Height;
        }

        public Image(MyTexture texture, bool active)
            : base(active)
        {
            Texture = texture;
            dest.Width = texture.Width;
            dest.Height = texture.Height;
        }

        public Image(MyTexture texture, int width, int height)
            : base(false)
        {
            Texture = texture;

            dest.Width = width;
            dest.Height = height;
        }
        
        public Image(MyTexture texture, int width, int height, bool active)
            : base(active)
        {
            Texture = texture;
            dest.Width = width;
            dest.Height = height;
        }

        public Image SetOrigin(float x, float y)
        {
            Origin.X = x;
            Origin.Y = y;
            return this;
        }

        public Image CenterOrigin()
        {
            Origin.X = Texture.Width / 2f;
            Origin.Y = Texture.Height / 2f;
            return this;
        }

        public override void Render()
        {
            if (Texture != null)
            {
                dest.X = (int)RenderPosition.X;
                dest.Y = (int)RenderPosition.Y;
                Texture.Draw(dest, Origin, Color, Rotation, Effects);
            }
        }

        public bool Intersects(int x, int y)
        {
            return dest.Contains(new Point(x, y));
        }
    }

}

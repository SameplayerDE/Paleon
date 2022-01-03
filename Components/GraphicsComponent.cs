using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paleon
{
    public abstract class GraphicsComponent : Component
    {

        public Vector2 Position;
        public Vector2 Origin;
        public Vector2 Scale = Vector2.One;
        public float Rotation;
        public Color Color = Color.White;
        public SpriteEffects Effects = SpriteEffects.None;

        public GraphicsComponent(bool active) 
            : base(active, true)
        {

        }

        public float X
        {
            get { return Position.X; }
            set { Position.X = value; }
        }

        public float Y
        {
            get { return Position.Y; }
            set { Position.Y = value; }
        }

        private bool flipX = false;
        public bool FlipX
        {
            get { return (Effects & SpriteEffects.FlipHorizontally) == SpriteEffects.FlipHorizontally; }
            set 
            {
                if (flipX != value)
                {
                    flipX = value;
                    Effects = value ? (Effects | SpriteEffects.FlipHorizontally) : (Effects & ~SpriteEffects.FlipHorizontally);
                }
            }
        }

        private bool flipY = false;
        public bool FlipY
        {
            get { return (Effects & SpriteEffects.FlipVertically) == SpriteEffects.FlipVertically; }
            set 
            {
                if (flipY != value)
                {
                    flipY = value;
                    Effects = value ? (Effects | SpriteEffects.FlipVertically) : (Effects & ~SpriteEffects.FlipVertically);
                }
            }
        }

        public Vector2 RenderPosition
        {
            get { return (Entity == null ? Vector2.Zero : Entity.Position) + Position; }
            set { Position = value - (Entity == null ? Vector2.Zero : Entity.Position); }
        }

        public void RenderOutline(int offset = 1)
        {
            RenderOutline(Color.Black, offset);
        }

        public void RenderOutline(Color color, int offset = 1)
        {
            Vector2 pos = Position;
            Color was = Color;
            Color = color;

            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    if(i != 0 || j != 0)
                    {
                        Position = pos + new Vector2(i * offset, j * offset);
                        Render();
                    }
                }
            }

            Position = pos;
            Color = was;

            Render();
        }
    }
}

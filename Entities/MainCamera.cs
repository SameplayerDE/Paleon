using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

namespace Paleon
{
    public class MainCamera
    {
        public Vector2 Position { get; set; }

        public float Zoom;

        private Matrix projection;

        public Matrix Projection { get { return projection; } }

        public MainCamera()
        {
            projection = Matrix.CreateOrthographicOffCenter(0, Engine.Width, Engine.Height, 0, -1f, 1f);

            Position = Vector2.Zero;
            Zoom = 1;
        }

        public Rectangle GetViewport()
        {
            return new Rectangle(
                (int)(Position.X - Engine.HalfWidth / Zoom),
                (int)(Position.Y - Engine.HalfHeight / Zoom),
                (int)(Engine.Width / Zoom), (int)(Engine.Height / Zoom));
        }

        public Matrix Transformation
        {
            get
            {
                return Matrix.CreateTranslation(new Vector3(-(int)Position.X, -(int)Position.Y, 0f))
                    * Matrix.CreateScale(Zoom) * Matrix.CreateTranslation(new Vector3(Engine.Width / 2, Engine.Height / 2, 0));
            }
        }

    }
}

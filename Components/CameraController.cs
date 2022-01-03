using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paleon
{
    public class CameraController : Component
    {

        private Vector2 mouseLastPosition;
        private int speed = 500;

        private const int MAX_ZOOM = 4;

        public CameraController() : base(true, false)
        {

        }

        public override void Begin()
        {
            RenderManager.MainCamera.Zoom = 2;
        }

        public override void Update()
        {
            DragMovementUpdate();
            KeyboardMovementUpdate();
            ZoomUpdate();
        }

        private void DragMovementUpdate()
        {
            if (MInput.Mouse.PressedMiddleButton)
            {
                mouseLastPosition = new Vector2(MInput.Mouse.X / RenderManager.MainCamera.Zoom, MInput.Mouse.Y / RenderManager.MainCamera.Zoom);
            }

            if (MInput.Mouse.CheckMiddleButton)
            {
                Vector2 mouseNewPosition = new Vector2(MInput.Mouse.X / RenderManager.MainCamera.Zoom, MInput.Mouse.Y / RenderManager.MainCamera.Zoom);

                RenderManager.MainCamera.Position = RenderManager.MainCamera.Position - (mouseNewPosition - mouseLastPosition);
                mouseLastPosition = mouseNewPosition;
            }
        }

        private void KeyboardMovementUpdate()
        {
            Vector2 motion = Vector2.Zero;

            if (MInput.Keyboard.Check(Keys.A))
                motion.X = -1;
            else if (MInput.Keyboard.Check(Keys.D))
                motion.X = 1;

            if (MInput.Keyboard.Check(Keys.W))
                motion.Y = -1;
            else if (MInput.Keyboard.Check(Keys.S))
                motion.Y = 1;

            if (motion != Vector2.Zero)
            {
                motion.Normalize();
                RenderManager.MainCamera.Position += motion * speed * Engine.DeltaTime;
            }
        }

        private void ZoomUpdate()
        {
            int value = MInput.Mouse.WheelDelta;
            if (value < 0)
            {
                RenderManager.MainCamera.Zoom -= 1;
                if (RenderManager.MainCamera.Zoom < 1)
                    RenderManager.MainCamera.Zoom = 1;
            }
            else if (value > 0)
            {
                RenderManager.MainCamera.Zoom += 1;
                if (RenderManager.MainCamera.Zoom > MAX_ZOOM)
                    RenderManager.MainCamera.Zoom = MAX_ZOOM;
            }
        }

    }
}

using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paleon
{
    public class SliderUI
    {
        private Vector2 position;
        public Vector2 Position
        {
            get { return position; }
            set
            {
                position = value;

                back.X = (int)position.X;
                back.Y = (int)position.Y;

                front.X = (int)position.X;
                front.Y = (int)position.Y;
            }
        }

        public int Width { get => back.Width; }
        public int Height { get => back.Height; }

        public bool Active {get; set;} = true;

        private ImageUI back;
        private ImageUI front;

        public SliderUI(int width, int height, Color backColor, Color frontColor)
        {
            back = new ImageUI(RenderManager.Pixel, width, height);
            front = new ImageUI(RenderManager.Pixel, 0, height);

            back.Color = backColor;
            front.Color = frontColor;

            Position = new Vector2(0, 0);
        }

        public void Render()
        {
            if (Active)
            {
                back.Render();
                front.Render();
            }
        }

        public void SetValue(float min, float max, float current)
        {
            float total = max - min;
            float curr = max - current; 

            float percent = 1.0f - (curr / total);
            front.Width = (int)(percent * back.Width);
        }

    }
}

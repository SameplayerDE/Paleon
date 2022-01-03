using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paleon
{
    public class AreaSelector
    {

        public bool Active;

        private Color color;

        private Rectangle rect;

        private PixelTextUI text;

        private int x;
        private int y;

        private int width;
        private int height;

        public AreaSelector(Color color)
        {
            this.color = color;

            rect = new Rectangle();

            text = new PixelTextUI(RenderManager.PixelFont, "", Color.White);
        }

        public void SetRect(int x, int y, int width, int height, int cellSize)
        {
            if (this.x == x && this.y == y && this.width == width && this.height == height)
                return;

            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;

            rect.X = x * cellSize;
            rect.Y = y * cellSize;
            rect.Width = width * cellSize;
            rect.Height = height * cellSize;

            if (width <= 1 && height > 1)
                text.Text = "" + height;
            else if (height <= 1 && width > 1)
                text.Text = "" + width;
            else if (width > 1 && height > 1)
                text.Text = width + "x" + height;
            else
                text.Text = "";

            text.X = (rect.X + rect.Width / 2) - text.Width / 2;
            text.Y = (rect.Y + rect.Height / 2) - text.Height / 2;
        }

        public void Render()
        {
            if (Active)
            {
                if(width > 0 || height > 0)
                {
                    text.Render();
                }

                RenderManager.HollowRect(rect, color);
            }
        }


    }
}

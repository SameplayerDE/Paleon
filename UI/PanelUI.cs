using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paleon
{
    public class PanelUI
    {
        public bool Visible = true;

        private ImageUI topLeft;
        private ImageUI top;
        private ImageUI topRight;
        private ImageUI left;
        private ImageUI center;
        private ImageUI right;
        private ImageUI bottomLeft;
        private ImageUI bottom;
        private ImageUI bottomRight;

        private Color color;
        public Color Color
        {
            get { return color; }
            set {
                color = value;
                topLeft.Color = color;
                top.Color = color;
                topRight.Color = color;
                left.Color = color;
                center.Color = color;
                right.Color = color;
                bottomLeft.Color = color;
                bottom.Color = color;
                bottomRight.Color = color;
            }
        }

        public int InnerX
        {
            get { return center.X; }
        }

        public int InnerY
        {
            get { return center.Y; }
        }


        public int X
        {
            get { return topLeft.X; }
            set
            {
                UpdatePositionsX(value);
            }
        }

        public int Y
        {
            get { return topLeft.Y; }
            set
            {
                UpdatePositionsY(value);
            }
        }

        public int InnerWidth
        {
            get { return center.Width; }
            set
            {
                top.Width = value;
                center.Width = value;
                bottom.Width = value;

                UpdatePositionsX(topLeft.X);
            }
        }

        public int InnerHeight
        {
            get { return center.Height; }
            set
            {
                left.Height = value;
                center.Height = value;
                right.Height = value;

                UpdatePositionsY(topLeft.Y);
            }
        }

        public int Width
        {
            get { return left.Width + center.Width + right.Width; }
            set { }
        }

        public int Height
        {
            get { return top.Height + center.Height + bottom.Height; }
            set { }
        }

        private void UpdatePositionsX(int value)
        {
            topLeft.X = value;
            top.X = topLeft.X + topLeft.Width;
            topRight.X = top.X + top.Width;

            left.X = value;
            center.X = left.X + left.Width;
            right.X = center.X + center.Width;

            bottomLeft.X = value;
            bottom.X = bottomLeft.X + bottomLeft.Width;
            bottomRight.X = bottom.X + bottom.Width;
        }

        private void UpdatePositionsY(int value)
        {
            topLeft.Y = value;
            top.Y = value;
            topRight.Y = value;

            left.Y = topLeft.Y + topLeft.Height;
            center.Y = left.Y;
            right.Y = left.Y;

            bottomLeft.Y = left.Y + left.Height;
            bottom.Y = bottomLeft.Y;
            bottomRight.Y = bottomLeft.Y;
        }

        public PanelUI()
        {
            topLeft = new ImageUI(TextureBank.PanelTopLeft, 8, 8);
            top = new ImageUI(TextureBank.PanelTop, 8, 8);
            topRight = new ImageUI(TextureBank.PanelTopRight, 8, 8);

            left = new ImageUI(TextureBank.PanelLeft, 8, 8);
            center = new ImageUI(TextureBank.PanelMiddle, 8, 8);
            right = new ImageUI(TextureBank.PanelRight, 8, 8);

            bottomLeft = new ImageUI(TextureBank.PanelBottomLeft, 8, 8);
            bottom = new ImageUI(TextureBank.PanelBottom, 8, 8);
            bottomRight = new ImageUI(TextureBank.PanelBottomRight, 8, 8);

            X = 0;
            Y = 0;
        }

        public void Render()
        {
            if (Visible)
            {
                topLeft.Render();
                top.Render();
                topRight.Render();

                left.Render();
                center.Render();
                right.Render();

                bottomLeft.Render();
                bottom.Render();
                bottomRight.Render();
            }
        }

        public bool Intersects(int x, int y)
        {
            return x >= X && x <= X + Width && y >= Y && y <= Y + Height;
        }
    }
}

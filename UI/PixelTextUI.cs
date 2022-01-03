using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paleon
{
    public class PixelTextUI
    {
        private struct Char
        {
            public Vector2 Offset;
            public PixelFontCharacter CharData;
            public Rectangle Bounds; 
        }

        public bool Active = true;

        private List<Char> characters = new List<Char>();
        private PixelFontSize size;
        private string text;
        private PixelFont font;


        public PixelFont Font
        {
            get { return font; }
            set
            {
                if (value == font)
                    return;
                    
                font = value;
                Refresh();
            }
        }

        public float Size
        {
            get { return size.Size; }
            set
            {
                if (value == size.Size)
                    return;

                size = font.Get(value);
                Refresh();
            }
        }

        public string Text
        {
            get { return text; }
            set
            {
                if (text == value)
                    return;

                text = value;
                Refresh();
            }
        }

        public Vector2 Position = new Vector2();
        public Color Color = Color.White;

        public int X
        {
            get { return (int)Position.X; }
            set { Position.X = value; }
        }

        public int Y
        {
            get { return (int)Position.Y; }
            set { Position.Y = value; }
        }

        private int width;
        public int Width
        {
            get { return width; }
            set { }
        }

        private int height;
        public int Height {
            get { return height; }
            set { }
        }

        public PixelTextUI(PixelFont font, string text, Color color)
        {
            this.font = font;
            this.text = text;
            size = Font.Sizes[0];
            Color = color;

            Refresh();
        }

        public void Refresh()
        {
            characters.Clear();

            var widest = 0;
            var lines = 1;
            var offset = Vector2.Zero;

            for (int i = 0; i < text.Length; i++)
            {
                // new line
                if (text[i] == '\n')
                {
                    offset.X = 0;
                    offset.Y += size.LineHeight;
                    lines++;
                }

                // add char
                var fontChar = size.Get(text[i]);
                if (fontChar != null)
                {
                    characters.Add(new Char()
                    {
                        Offset = offset + new Vector2(fontChar.XOffset, fontChar.YOffset),
                        CharData = fontChar,
                        Bounds = fontChar.Texture.ClipRect,
                    });

                    offset.X += fontChar.XAdvance;

                    if (offset.X > widest)
                        widest = (int)offset.X;
                }
            }

            width = widest;
            height = lines * size.LineHeight;
        }

        public void Render()
        {
            if (Active)
            {
                for (var i = 0; i < characters.Count; i++)
                    characters[i].CharData.Texture.Draw(Position + characters[i].Offset, Vector2.Zero, Color);
            }
        }

        public void RenderShadow(int outline)
        {
            if (Active)
            {
                for (var k = 0; k < characters.Count; k++)
                {
                    characters[k].CharData.Texture.Draw(Position + characters[k].Offset + new Vector2(outline), Vector2.Zero, Color.Black);
                    characters[k].CharData.Texture.Draw(Position + characters[k].Offset, Vector2.Zero, Color);
                }
            }
        }

        public bool Intersects(int x, int y)
        {
            return x >= X && x <= X + Width && y >= Y && y <= Y + Height;
        }
    }
}

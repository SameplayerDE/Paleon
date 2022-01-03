using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Paleon
{
    public static class RenderManager
    {
        public static SpriteBatch SpriteBatch { get; private set; }

        public static MainCamera MainCamera { get; private set; }

        public static PixelFont PixelFont { get; private set; }

        public static MyTexture Pixel { get; private set; }

        private static Rectangle rect;

        private static GraphicsDevice gd;

        private static bool wasInitialized = false;

        public static void Initialize(GraphicsDevice graphicsDevice)
        {
            gd = graphicsDevice;
            SpriteBatch = new SpriteBatch(graphicsDevice);
            MainCamera = new MainCamera();
            PixelFont = new PixelFont("DefaultFont");
            PixelFont.AddFontSize(Engine.ContentDirectory + "/Fonts/default.fnt");
            Pixel = new MyTexture(1, 1, Color.White);

            wasInitialized = true;
        }

        public static void Begin(SamplerState samperState, BlendState blendState)
        {
            if (!wasInitialized)
                throw new Exception("Render Manager was not initialized!"); 

            gd.SamplerStates[0] = samperState;
            gd.BlendState = blendState;
        }

        public static void Rect(float x, float y, float width, float height, Color color)
        {
            rect.X = (int)x;
            rect.Y = (int)y;
            rect.Width = (int)width;
            rect.Height = (int)height;
            SpriteBatch.Draw(Pixel.Texture, rect, Pixel.ClipRect, color);
        }

        public static void HollowRect(float x, float y, float width, float height, Color color)
        {
            rect.X = (int)x;
            rect.Y = (int)y;
            rect.Width = (int)width;
            rect.Height = (int)height;

            SpriteBatch.Draw(Pixel.Texture, rect, Pixel.ClipRect, color * 0.2f);

            rect.Height = 1;

            SpriteBatch.Draw(Pixel.Texture, rect, Pixel.ClipRect, color * 0.8f);

            rect.Y += (int)height - 1;

            SpriteBatch.Draw(Pixel.Texture, rect, Pixel.ClipRect, color);

            rect.Y -= (int)height - 1;
            rect.Width = 1;
            rect.Height = (int)height;

            SpriteBatch.Draw(Pixel.Texture, rect, Pixel.ClipRect, color * 0.8f);

            rect.X += (int)width - 1;

            SpriteBatch.Draw(Pixel.Texture, rect, Pixel.ClipRect, color);
        }

        public static void HollowRect(Vector2 position, float width, float height, Color color)
        {
            HollowRect(position.X, position.Y, width, height, color);
        }

        public static void HollowRect(Rectangle rect, Color color)
        {
            HollowRect(rect.X, rect.Y, rect.Width, rect.Height, color);
        }

        public static void Circle(Vector2 position, float radius, Color color, int resolution)
        {
            Vector2 last = Vector2.UnitX * radius;
            Vector2 lastP = last.Perpendicular();
            for (int i = 1; i <= resolution; i++)
            {
                Vector2 at = Utils.AngleToVector(i * MathHelper.PiOver2 / resolution, radius);
                Vector2 atP = at.Perpendicular();

                Line(position + last, position + at, color);
                Line(position - last, position - at, color);
                Line(position + lastP, position + atP, color);
                Line(position - lastP, position - atP, color);

                last = at;
                lastP = atP;
            }
        }

        public static void Circle(float x, float y, float radius, Color color, int resolution)
        {
            Circle(new Vector2(x, y), radius, color, resolution);
        }

        public static void Circle(Vector2 position, float radius, Color color, float thickness, int resolution)
        {
            Vector2 last = Vector2.UnitX * radius;
            Vector2 lastP = last.Perpendicular();
            for (int i = 1; i <= resolution; i++)
            {
                Vector2 at = Utils.AngleToVector(i * MathHelper.PiOver2 / resolution, radius);
                Vector2 atP = at.Perpendicular();

                Line(position + last, position + at, color, thickness);
                Line(position - last, position - at, color, thickness);
                Line(position + lastP, position + atP, color, thickness);
                Line(position - lastP, position - atP, color, thickness);

                last = at;
                lastP = atP;
            }
        }

        public static void Circle(float x, float y, float radius, Color color, float thickness, int resolution)
        {
            Circle(new Vector2(x, y), radius, color, thickness, resolution);
        }

        public static void Line(Vector2 start, Vector2 end, Color color)
        {
            LineAngle(start, Utils.Angle(start, end), Vector2.Distance(start, end), color);
        }

        public static void Line(Vector2 start, Vector2 end, Color color, float thickness)
        {
            LineAngle(start, Utils.Angle(start, end), Vector2.Distance(start, end), color, thickness);
        }

        public static void Line(float x1, float y1, float x2, float y2, Color color)
        {
            Line(new Vector2(x1, y1), new Vector2(x2, y2), color);
        }

        public static void LineAngle(Vector2 start, float angle, float length, Color color)
        {
            SpriteBatch.Draw(Pixel.Texture, start, Pixel.ClipRect, color, angle, Vector2.Zero, new Vector2(length, 1), SpriteEffects.None, 0);
        }

        public static void LineAngle(Vector2 start, float angle, float length, Color color, float thickness)
        {
            SpriteBatch.Draw(Pixel.Texture, start, Pixel.ClipRect, color, angle, new Vector2(0, .5f), new Vector2(length, thickness), SpriteEffects.None, 0);
        }

        public static void LineAngle(float startX, float startY, float angle, float length, Color color)
        {
            LineAngle(new Vector2(startX, startY), angle, length, color);
        }


    }
}

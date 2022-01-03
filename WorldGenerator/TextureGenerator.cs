using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Paleon
{
    public static class TextureGenerator
    {

        // Height Map Colors
        private static Color DeepColor = new Color(15f / 255f, 30f / 255f, 80f / 255f, 1f);
        private static Color ShallowColor = new Color(15f / 255f, 40f / 255f, 90f / 255f, 1f);
        private static Color RiverColor = new Color(30f / 255f, 120f / 255f, 200f / 255f, 1f);
        private static Color SandColor = new Color(198f / 255f, 190f / 255f, 31f / 255f, 1f);
        private static Color GrassColor = new Color(50f / 255f, 220f / 255f, 20f / 255f, 1f);
        private static Color ForestColor = new Color(16f / 255f, 160f / 255f, 0f, 1f);
        private static Color RockColor = new Color(0.5f, 0.5f, 0.5f, 1f);
        private static Color SnowColor = new Color(1f, 1f, 1f, 1f);

        private static Color IceWater = new Color(210f / 255f, 255f / 255f, 252f / 255f, 1f);
        private static Color ColdWater = new Color(119f / 255f, 156f / 255f, 213f / 255f, 1f);
        private static Color RiverWater = new Color(65f / 255f, 110f / 255f, 179f / 255f, 1f);

        // Height Map Colors
        private static Color Coldest = new Color(255, 0, 255, 255);
        private static Color Colder = new Color(0, 0, 255);
        private static Color Cold = new Color(0, 255, 255, 255);
        private static Color Warm = new Color(0, 255, 0, 255);
        private static Color Warmer = new Color(255, 255, 0, 255);
        private static Color Warmest = new Color(255, 0, 0, 255);

        //Moisture map
        private static Color Dryest = new Color(255f / 255f, 139f / 255f, 17f / 255f, 1f);
        private static Color Dryer = new Color(245f / 255f, 245f / 255f, 23f / 255f, 1f);
        private static Color Dry = new Color(80f / 255f, 255f / 255f, 0f / 255f, 1f);
        private static Color Wet = new Color(85f / 255f, 255f / 255f, 255f / 255f, 1f);
        private static Color Wetter = new Color(20f / 255f, 70f / 255f, 255f / 255f, 1f);
        private static Color Wettest = new Color(0f / 255f, 0f / 255f, 100f / 255f, 1f);

        //biome map
        private static Color Ice = Color.White;
        private static Color Desert = new Color(238, 218, 130, 255);
        private static Color Savanna = new Color(177, 209, 110, 255);
        private static Color TropicalRainforest = new Color(66, 123, 25, 255);
        private static Color Tundra = new Color(96, 131, 112, 255);
        private static Color TemperateRainforest = new Color(29, 73, 40, 255);
        private static Color Grassland = new Color(164, 225, 99, 255);
        private static Color SeasonalForest = new Color(73, 100, 35, 255);
        private static Color BorealForest = new Color(95, 115, 62, 255);
        private static Color Chaparral = new Color(139, 175, 90, 255);

        public static Texture2D GetHeightMapTexture(GraphicsDevice device, int width, int height, PixelTile[,] tiles)
        {
            var texture = new Texture2D(device, width, height);
            var pixels = new Color[width * height];

            for (var x = 0; x < width; x++)
            {
                for (var y = 0; y < height; y++)
                {
                    switch (tiles[x, y].HeightType)
                    {
                        case HeightType.DeepWater:
                            pixels[x + y * width] = new Color(0f, 0f, 0f, 1f);
                            break;
                        case HeightType.ShallowWater:
                            pixels[x + y * width] = new Color(0f, 0f, 0f, 1f);
                            break;
                        case HeightType.Sand:
                            pixels[x + y * width] = new Color(0.3f, 0.3f, 0.3f, 1f);
                            break;
                        case HeightType.Grass:
                            pixels[x + y * width] = new Color(0.45f, 0.45f, 0.45f, 1f);
                            break;
                        case HeightType.Forest:
                            pixels[x + y * width] = new Color(0.6f, 0.6f, 0.6f, 1f);
                            break;
                        case HeightType.Rock:
                            pixels[x + y * width] = new Color(0.75f, 0.75f, 0.75f, 1f);
                            break;
                        case HeightType.Snow:
                            pixels[x + y * width] = new Color(1f, 1f, 1f, 1f);
                            break;
                        case HeightType.River:
                            pixels[x + y * width] = new Color(0f, 0f, 0f, 1f);
                            break;
                    }

                    //				pixels[x + y * width] = Color.Lerp(Color.black, Color.white, tiles[x,y].HeightValue);
                    //
                    //				//darken the color if a edge tile
                    //				if ((int)tiles[x,y].HeightType > 2 && tiles[x,y].Bitmask != 15)
                    //					pixels[x + y * width] = Color.Lerp(pixels[x + y * width], Color.black, 0.4f);
                    //
                    //				if (tiles[x,y].Color != Color.black)
                    //					pixels[x + y * width] = tiles[x,y].Color;
                    //				else if ((int)tiles[x,y].HeightType > 2)
                    //					pixels[x + y * width] = Color.white;
                    //				else
                    //					pixels[x + y * width] = Color.black;
                }
            }

            texture.SetData(pixels);
            return texture;
        }

        public static Texture2D GetHeatMapTexture(GraphicsDevice device, int width, int height, PixelTile[,] tiles)
        {
            var texture = new Texture2D(device, width, height);
            var pixels = new Color[width * height];

            for (var x = 0; x < width; x++)
            {
                for (var y = 0; y < height; y++)
                {
                    switch (tiles[x, y].HeatType)
                    {
                        case HeatType.Coldest:
                            pixels[x + y * width] = Coldest;
                            break;
                        case HeatType.Colder:
                            pixels[x + y * width] = Colder;
                            break;
                        case HeatType.Cold:
                            pixels[x + y * width] = Cold;
                            break;
                        case HeatType.Warm:
                            pixels[x + y * width] = Warm;
                            break;
                        case HeatType.Warmer:
                            pixels[x + y * width] = Warmer;
                            break;
                        case HeatType.Warmest:
                            pixels[x + y * width] = Warmest;
                            break;
                    }

                    if (tiles[x, y].HeightType == HeightType.DeepWater)
                        pixels[x + y * width] = Coldest;

                    if (tiles[x, y].HeightType == HeightType.ShallowWater || tiles[x, y].HeightType == HeightType.River)
                        pixels[x + y * width] = Colder;
                }
            }

            texture.SetData(pixels);
            return texture;
        }

        public static Texture2D GetMoistureMapTexture(GraphicsDevice device, int width, int height, PixelTile[,] tiles)
        {
            var texture = new Texture2D(device, width, height);
            var pixels = new Color[width * height];

            for (var x = 0; x < width; x++)
            {
                for (var y = 0; y < height; y++)
                {
                    PixelTile t = tiles[x, y];

                    if (t.MoistureType == MoistureType.Dryest)
                        pixels[x + y * width] = Dryest;
                    else if (t.MoistureType == MoistureType.Dryer)
                        pixels[x + y * width] = Dryer;
                    else if (t.MoistureType == MoistureType.Dry)
                        pixels[x + y * width] = Dry;
                    else if (t.MoistureType == MoistureType.Wet)
                        pixels[x + y * width] = Wet;
                    else if (t.MoistureType == MoistureType.Wetter)
                        pixels[x + y * width] = Wetter;
                    else
                        pixels[x + y * width] = Wettest;
                }
            }

            texture.SetData(pixels);
            return texture;
        }

        public static Texture2D GetBiomeMapTexture(GraphicsDevice device, int width, int height, PixelTile[,] tiles, float coldest, float colder, float cold)
        {
            var texture = new Texture2D(device, width, height);
            var pixels = new Color[width * height];

            for (var x = 0; x < width; x++)
            {
                for (var y = 0; y < height; y++)
                {
                    BiomeType value = tiles[x, y].BiomeType;

                    switch (value)
                    {
                        case BiomeType.Ice:
                            pixels[x + y * width] = new Color(199, 223, 235, 255);
                            break;
                        case BiomeType.Tundra:
                            pixels[x + y * width] = new Color(241, 89, 65, 255);
                            break;
                        case BiomeType.BorealForest:
                            pixels[x + y * width] = new Color(254, 205, 103, 255);
                            break;
                        case BiomeType.TemperateForest:
                            pixels[x + y * width] = new Color(252, 188, 124, 255);
                            break;
                        case BiomeType.TemperateGrassland:
                            pixels[x + y * width] = new Color(245, 132, 102, 255);
                            break;
                        case BiomeType.Chaparral:
                            pixels[x + y * width] = new Color(18, 146, 185, 255);
                            break;
                        case BiomeType.Desert:
                            pixels[x + y * width] = new Color(169, 154, 123, 255);
                            break;
                        case BiomeType.Savanna:
                            pixels[x + y * width] = new Color(255, 229, 202, 255);
                            break;
                        case BiomeType.TropicalDryforest:
                            pixels[x + y * width] = new Color(189, 242, 64, 255);
                            break;
                        case BiomeType.TropicalRainforest:
                            pixels[x + y * width] = new Color(120, 164, 79, 255);
                            break;
                    }

                    // Water tiles
                    if (tiles[x, y].HeightType == HeightType.DeepWater)
                    {
                        pixels[x + y * width] = DeepColor;
                    }
                    else if (tiles[x, y].HeightType == HeightType.ShallowWater)
                    {
                        pixels[x + y * width] = ShallowColor;
                    }

                    // draw rivers
                    if (tiles[x, y].HeightType == HeightType.River)
                    {
                        float heatValue = tiles[x, y].HeatValue;

                        /*if (tiles[x,y].HeatType == HeatType.Coldest)
                            pixels[x + y * width] = Color.Lerp (IceWater, ColdWater, (heatValue) / (coldest));
                        else if (tiles[x,y].HeatType == HeatType.Colder)
                            pixels[x + y * width] = Color.Lerp (ColdWater, RiverWater, (heatValue - coldest) / (colder - coldest));
                        else if (tiles[x,y].HeatType == HeatType.Cold)
                            pixels[x + y * width] = Color.Lerp (RiverWater, ShallowColor, (heatValue - colder) / (cold - colder));
                        else
                            pixels[x + y * width] = ShallowColor;*/
                        pixels[x + y * width] = ShallowColor;

                    }
                }
            }

            texture.SetData(pixels);
            return texture;
        }
    }
}
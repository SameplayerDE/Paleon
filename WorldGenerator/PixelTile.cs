using System.Collections.Generic;
using Microsoft.Xna.Framework;


public enum HeightType
{
	DeepWater = 1,
	ShallowWater = 2,
	Shore = 3,
	Sand = 4,
	Grass = 5,
	Forest = 6,
	Rock = 7,
	Snow = 8,
	River = 9
}

public enum HeatType
{
	Coldest = 0,
	Colder = 1,
	Cold = 2,
	Warm = 3,
	Warmer = 4,
	Warmest = 5
}

public enum MoistureType
{
	Wettest = 5,
	Wetter = 4,
	Wet = 3,
	Dry = 2,
	Dryer = 1,
	Dryest = 0
}

public enum BiomeType
{
	Desert,
	Savanna,
	TropicalRainforest,
	TemperateGrassland,
	Chaparral,
	TemperateForest,
	TropicalDryforest,
	BorealForest,
	Tundra,
	Ice
}

namespace Paleon
{
    public class PixelTile
    {
        public HeightType HeightType;
        public HeatType HeatType;
        public MoistureType MoistureType;
        public BiomeType BiomeType;

        public float Cloud1Value { get; set; }
        public float Cloud2Value { get; set; }
        public float HeightValue { get; set; }
        public float HeatValue { get; set; }
        public float MoistureValue { get; set; }
        public int X, Y;

        public PixelTile Left;
        public PixelTile Right;
        public PixelTile Top;
        public PixelTile Bottom;

        public bool Collidable;

        public Color Color = Color.Black;

        public List<River> Rivers = new List<River>();

        public int RiverSize { get; set; }

        public PixelTile()
        {
        }

        public int GetRiverNeighborCount(River river)
        {
            int count = 0;
            if (Left != null && Left.Rivers.Count > 0 && Left.Rivers.Contains(river))
                count++;
            if (Right != null && Right.Rivers.Count > 0 && Right.Rivers.Contains(river))
                count++;
            if (Top != null && Top.Rivers.Count > 0 && Top.Rivers.Contains(river))
                count++;
            if (Bottom != null && Bottom.Rivers.Count > 0 && Bottom.Rivers.Contains(river))
                count++;
            return count;
        }

        public Direction GetLowestNeighbor(PixelGenerator generator)
        {
            float left = generator.GetHeightValue(Left);
            float right = generator.GetHeightValue(Right);
            float bottom = generator.GetHeightValue(Bottom);
            float top = generator.GetHeightValue(Top);

            if (left < right && left < top && left < bottom)
                return Direction.LEFT;
            else if (right < left && right < top && right < bottom)
                return Direction.RIGHT;
            else if (top < left && top < right && top < bottom)
                return Direction.UP;
            else if (bottom < top && bottom < right && bottom < left)
                return Direction.DOWN;
            else
                return Direction.DOWN;
        }

        public void SetRiverPath(River river)
        {
            if (!Collidable)
                return;

            if (!Rivers.Contains(river))
            {
                Rivers.Add(river);
            }
        }

        private void SetRiverTile(River river)
        {
            SetRiverPath(river);
            HeightType = HeightType.River;
            HeightValue = 0;
            Collidable = false;
        }

        // This function got messy.  Sorry.
        public void DigRiver(River river, int size)
        {
            SetRiverTile(river);
            RiverSize = size;

            if (size == 1)
            {
                if (Bottom != null)
                {
                    Bottom.SetRiverTile(river);
                    if (Bottom.Right != null) Bottom.Right.SetRiverTile(river);
                }
                if (Right != null) Right.SetRiverTile(river);
            }

            if (size == 2)
            {
                if (Bottom != null)
                {
                    Bottom.SetRiverTile(river);
                    if (Bottom.Right != null) Bottom.Right.SetRiverTile(river);
                }
                if (Right != null)
                {
                    Right.SetRiverTile(river);
                }
                if (Top != null)
                {
                    Top.SetRiverTile(river);
                    if (Top.Left != null) Top.Left.SetRiverTile(river);
                    if (Top.Right != null) Top.Right.SetRiverTile(river);
                }
                if (Left != null)
                {
                    Left.SetRiverTile(river);
                    if (Left.Bottom != null) Left.Bottom.SetRiverTile(river);
                }
            }

            if (size == 3)
            {
                if (Bottom != null)
                {
                    Bottom.SetRiverTile(river);
                    if (Bottom.Right != null) Bottom.Right.SetRiverTile(river);
                    if (Bottom.Bottom != null)
                    {
                        Bottom.Bottom.SetRiverTile(river);
                        if (Bottom.Bottom.Right != null) Bottom.Bottom.Right.SetRiverTile(river);
                    }
                }
                if (Right != null)
                {
                    Right.SetRiverTile(river);
                    if (Right.Right != null)
                    {
                        Right.Right.SetRiverTile(river);
                        if (Right.Right.Bottom != null) Right.Right.Bottom.SetRiverTile(river);
                    }
                }
                if (Top != null)
                {
                    Top.SetRiverTile(river);
                    if (Top.Left != null) Top.Left.SetRiverTile(river);
                    if (Top.Right != null) Top.Right.SetRiverTile(river);
                }
                if (Left != null)
                {
                    Left.SetRiverTile(river);
                    if (Left.Bottom != null) Left.Bottom.SetRiverTile(river);
                }
            }

            if (size == 4)
            {

                if (Bottom != null)
                {
                    Bottom.SetRiverTile(river);
                    if (Bottom.Right != null) Bottom.Right.SetRiverTile(river);
                    if (Bottom.Bottom != null)
                    {
                        Bottom.Bottom.SetRiverTile(river);
                        if (Bottom.Bottom.Right != null) Bottom.Bottom.Right.SetRiverTile(river);
                    }
                }
                if (Right != null)
                {
                    Right.SetRiverTile(river);
                    if (Right.Right != null)
                    {
                        Right.Right.SetRiverTile(river);
                        if (Right.Right.Bottom != null) Right.Right.Bottom.SetRiverTile(river);
                    }
                }
                if (Top != null)
                {
                    Top.SetRiverTile(river);
                    if (Top.Right != null)
                    {
                        Top.Right.SetRiverTile(river);
                        if (Top.Right.Right != null) Top.Right.Right.SetRiverTile(river);
                    }
                    if (Top.Top != null)
                    {
                        Top.Top.SetRiverTile(river);
                        if (Top.Top.Right != null) Top.Top.Right.SetRiverTile(river);
                    }
                }
                if (Left != null)
                {
                    Left.SetRiverTile(river);
                    if (Left.Bottom != null)
                    {
                        Left.Bottom.SetRiverTile(river);
                        if (Left.Bottom.Bottom != null) Left.Bottom.Bottom.SetRiverTile(river);
                    }

                    if (Left.Left != null)
                    {
                        Left.Left.SetRiverTile(river);
                        if (Left.Left.Bottom != null) Left.Left.Bottom.SetRiverTile(river);
                        if (Left.Left.Top != null) Left.Left.Top.SetRiverTile(river);
                    }

                    if (Left.Top != null)
                    {
                        Left.Top.SetRiverTile(river);
                        if (Left.Top.Top != null) Left.Top.Top.SetRiverTile(river);
                    }
                }
            }
        }
    }
}
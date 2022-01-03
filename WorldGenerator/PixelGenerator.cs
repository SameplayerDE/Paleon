using AccidentalNoise;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Paleon
{
    public abstract class PixelGenerator
    {

        protected int Seed { get; set; }

        protected int Width;
        protected int Height;

        protected int TerrainOctaves = 6;
        protected double TerrainFrequency = 1.25;
        protected float DeepWater = 0.2f;
        protected float ShallowWater = 0.4f;
        protected float Sand = 0.5f;
        protected float Grass = 0.7f;
        protected float Forest = 0.8f;
        protected float Rock = 0.9f;

        protected int HeatOctaves = 4;
        protected double HeatFrequency = 3.0;
        protected float ColdestValue = 0.05f;
        protected float ColderValue = 0.18f;
        protected float ColdValue = 0.4f;
        protected float WarmValue = 0.6f;
        protected float WarmerValue = 0.8f;

        protected int MoistureOctaves = 4;
        protected double MoistureFrequency = 3.0;
        protected float DryerValue = 0.27f;
        protected float DryValue = 0.4f;
        protected float WetValue = 0.6f;
        protected float WetterValue = 0.8f;
        protected float WettestValue = 0.9f;

        protected int RiverCount = 5;
        protected float MinRiverHeight = 0.6f;
        protected int MaxRiverAttempts = 1000;
        protected int MinRiverTurns = 18;
        protected int MinRiverLength = 20;
        protected int MaxRiverIntersections = 2;

        protected MapData HeightData;
        protected MapData HeatData;
        protected MapData MoistureData;

        protected PixelTile[,] Tiles;

        protected List<PixelTileGroup> Waters = new List<PixelTileGroup>();
        protected List<PixelTileGroup> Lands = new List<PixelTileGroup>();

        protected List<River> Rivers = new List<River>();
        protected List<RiverGroup> RiverGroups = new List<RiverGroup>();

        protected BiomeType[,] BiomeTable = new BiomeType[6, 6] {   
		//COLDEST        //COLDER          //COLD                  //HOT                          //HOTTER                       //HOTTEST
		{ BiomeType.Ice, BiomeType.Tundra, BiomeType.TemperateGrassland,    BiomeType.Desert,              BiomeType.Desert,              BiomeType.Desert },              //DRYEST
		{ BiomeType.Ice, BiomeType.Tundra, BiomeType.TemperateGrassland,    BiomeType.Desert,              BiomeType.Desert,              BiomeType.Desert },              //DRYER
		{ BiomeType.Ice, BiomeType.Tundra, BiomeType.Chaparral,     BiomeType.Chaparral,            BiomeType.Savanna,             BiomeType.Savanna },             //DRY
		{ BiomeType.Ice, BiomeType.Tundra, BiomeType.BorealForest, BiomeType.Chaparral,            BiomeType.Savanna,             BiomeType.Savanna },             //WET
		{ BiomeType.Ice, BiomeType.Tundra, BiomeType.BorealForest, BiomeType.TemperateForest,      BiomeType.TropicalRainforest,  BiomeType.TropicalRainforest },  //WETTER
		{ BiomeType.Ice, BiomeType.Tundra, BiomeType.BorealForest, BiomeType.TropicalDryforest, BiomeType.TropicalRainforest,  BiomeType.TropicalRainforest }   //WETTEST
	};

        public PixelGenerator(int width, int height)
        {
            Width = width;
            Height = height;

            Seed = MyRandom.Range(0, int.MaxValue);
        }

        public abstract void Initialize();
        protected abstract void GetData();

        protected abstract PixelTile GetTop(PixelTile tile);
        protected abstract PixelTile GetBottom(PixelTile tile);
        protected abstract PixelTile GetLeft(PixelTile tile);
        protected abstract PixelTile GetRight(PixelTile tile);


        public virtual void Generate()
        {
            GetData();
            LoadTiles();
            UpdateNeighbors();

            GenerateRivers();
            BuildRiverGroups();
            DigRiverGroups();
            AdjustMoistureMap();

            GenerateBiomeMap();
        }

        public PixelTile GetPixelTile(int x, int y)
        {
            if (x < 0 || y < 0 || x > Tiles.GetLength(0) || y > Tiles.GetLength(1))
                return null;

            return Tiles[x, y];
        }

        public BiomeType GetBiomeType(PixelTile tile)
        {
            return BiomeTable[(int)tile.MoistureType, (int)tile.HeatType];
        }

        private void GenerateBiomeMap()
        {
            for (var x = 0; x < Width; x++)
            {
                for (var y = 0; y < Height; y++)
                {

                    if (!Tiles[x, y].Collidable) continue;

                    PixelTile t = Tiles[x, y];
                    t.BiomeType = GetBiomeType(t);
                }
            }
        }

        private void AddMoisture(PixelTile t, int radius)
        {
            int startx = MyMathHelper.Mod(t.X - radius, Width);
            int endx = MyMathHelper.Mod(t.X + radius, Width);
            Vector2 center = new Vector2(t.X, t.Y);
            int curr = radius;

            while (curr > 0)
            {

                int x1 = MyMathHelper.Mod(t.X - curr, Width);
                int x2 = MyMathHelper.Mod(t.X + curr, Width);
                int y = t.Y;

                AddMoisture(Tiles[x1, y], 0.025f / (center - new Vector2(x1, y)).Length());

                for (int i = 0; i < curr; i++)
                {
                    AddMoisture(Tiles[x1, MyMathHelper.Mod(y + i + 1, Height)], 0.025f / (center - new Vector2(x1, MyMathHelper.Mod(y + i + 1, Height))).Length());
                    AddMoisture(Tiles[x1, MyMathHelper.Mod(y - (i + 1), Height)], 0.025f / (center - new Vector2(x1, MyMathHelper.Mod(y - (i + 1), Height))).Length());

                    AddMoisture(Tiles[x2, MyMathHelper.Mod(y + i + 1, Height)], 0.025f / (center - new Vector2(x2, MyMathHelper.Mod(y + i + 1, Height))).Length());
                    AddMoisture(Tiles[x2, MyMathHelper.Mod(y - (i + 1), Height)], 0.025f / (center - new Vector2(x2, MyMathHelper.Mod(y - (i + 1), Height))).Length());
                }
                curr--;
            }
        }

        private void AddMoisture(PixelTile t, float amount)
        {
            MoistureData.Data[t.X, t.Y] += amount;
            t.MoistureValue += amount;
            if (t.MoistureValue > 1)
                t.MoistureValue = 1;

            //set moisture type
            if (t.MoistureValue < DryerValue) t.MoistureType = MoistureType.Dryest;
            else if (t.MoistureValue < DryValue) t.MoistureType = MoistureType.Dryer;
            else if (t.MoistureValue < WetValue) t.MoistureType = MoistureType.Dry;
            else if (t.MoistureValue < WetterValue) t.MoistureType = MoistureType.Wet;
            else if (t.MoistureValue < WettestValue) t.MoistureType = MoistureType.Wetter;
            else t.MoistureType = MoistureType.Wettest;
        }

        private void AdjustMoistureMap()
        {
            for (var x = 0; x < Width; x++)
            {
                for (var y = 0; y < Height; y++)
                {

                    PixelTile t = Tiles[x, y];
                    if (t.HeightType == HeightType.River)
                    {
                        AddMoisture(t, (int)60);
                    }
                }
            }
        }

        private void DigRiverGroups()
        {
            for (int i = 0; i < RiverGroups.Count; i++)
            {

                RiverGroup group = RiverGroups[i];
                River longest = null;

                //Find longest river in this group
                for (int j = 0; j < group.Rivers.Count; j++)
                {
                    River river = group.Rivers[j];
                    if (longest == null)
                        longest = river;
                    else if (longest.Tiles.Count < river.Tiles.Count)
                        longest = river;
                }

                if (longest != null)
                {
                    //Dig out longest path first
                    DigRiver(longest);

                    for (int j = 0; j < group.Rivers.Count; j++)
                    {
                        River river = group.Rivers[j];
                        if (river != longest)
                        {
                            DigRiver(river, longest);
                        }
                    }
                }
            }
        }

        private void BuildRiverGroups()
        {
            //loop each tile, checking if it belongs to multiple rivers
            for (var x = 0; x < Width; x++)
            {
                for (var y = 0; y < Height; y++)
                {
                    PixelTile t = Tiles[x, y];

                    if (t.Rivers.Count > 1)
                    {
                        // multiple rivers == intersection
                        RiverGroup group = null;

                        // Does a rivergroup already exist for this group?
                        for (int n = 0; n < t.Rivers.Count; n++)
                        {
                            River tileriver = t.Rivers[n];
                            for (int i = 0; i < RiverGroups.Count; i++)
                            {
                                for (int j = 0; j < RiverGroups[i].Rivers.Count; j++)
                                {
                                    River river = RiverGroups[i].Rivers[j];
                                    if (river.ID == tileriver.ID)
                                    {
                                        group = RiverGroups[i];
                                    }
                                    if (group != null) break;
                                }
                                if (group != null) break;
                            }
                            if (group != null) break;
                        }

                        // existing group found -- add to it
                        if (group != null)
                        {
                            for (int n = 0; n < t.Rivers.Count; n++)
                            {
                                if (!group.Rivers.Contains(t.Rivers[n]))
                                    group.Rivers.Add(t.Rivers[n]);
                            }
                        }
                        else   //No existing group found - create a new one
                        {
                            group = new RiverGroup();
                            for (int n = 0; n < t.Rivers.Count; n++)
                            {
                                group.Rivers.Add(t.Rivers[n]);
                            }
                            RiverGroups.Add(group);
                        }
                    }
                }
            }
        }

        public float GetHeightValue(PixelTile tile)
        {
            if (tile == null)
                return int.MaxValue;
            else
                return tile.HeightValue;
        }

        private void GenerateRivers()
        {
            int attempts = 0;
            int rivercount = RiverCount;
            Rivers = new List<River>();

            // Generate some rivers
            while (rivercount > 0 && attempts < MaxRiverAttempts)
            {

                // Get a random tile
                int x = MyRandom.Range(0, Width);
                int y = MyRandom.Range(0, Height);
                PixelTile tile = Tiles[x, y];

                // validate the tile
                if (!tile.Collidable) continue;
                if (tile.Rivers.Count > 0) continue;

                if (tile.HeightValue > MinRiverHeight)
                {
                    // Tile is good to start river from
                    River river = new River(rivercount);

                    // Figure out the direction this river will try to flow
                    river.CurrentDirection = tile.GetLowestNeighbor(this);

                    // Recursively find a path to water
                    FindPathToWater(tile, river.CurrentDirection, ref river);

                    // Validate the generated river 
                    if (river.TurnCount < MinRiverTurns || river.Tiles.Count < MinRiverLength || river.Intersections > MaxRiverIntersections)
                    {
                        //Validation failed - remove this river
                        for (int i = 0; i < river.Tiles.Count; i++)
                        {
                            PixelTile t = river.Tiles[i];
                            t.Rivers.Remove(river);
                        }
                    }
                    else if (river.Tiles.Count >= MinRiverLength)
                    {
                        //Validation passed - Add river to list
                        Rivers.Add(river);
                        tile.Rivers.Add(river);
                        rivercount--;
                    }
                }
                attempts++;
            }
        }

        // Dig river based on a parent river vein
        private void DigRiver(River river, River parent)
        {
            int intersectionID = 0;
            int intersectionSize = 0;

            // determine point of intersection
            for (int i = 0; i < river.Tiles.Count; i++)
            {
                PixelTile t1 = river.Tiles[i];
                for (int j = 0; j < parent.Tiles.Count; j++)
                {
                    PixelTile t2 = parent.Tiles[j];
                    if (t1 == t2)
                    {
                        intersectionID = i;
                        intersectionSize = t2.RiverSize;
                    }
                }
            }

            int counter = 0;
            int intersectionCount = river.Tiles.Count - intersectionID;
            int size = MyRandom.Range(intersectionSize, 5);
            river.Length = river.Tiles.Count;

            // randomize size change
            int two = river.Length / 2;
            int three = two / 2;
            int four = three / 2;
            int five = four / 2;

            int twomin = two / 3;
            int threemin = three / 3;
            int fourmin = four / 3;
            int fivemin = five / 3;

            // randomize length of each size
            int count1 = MyRandom.Range(fivemin, five);
            if (size < 4)
            {
                count1 = 0;
            }
            int count2 = count1 + MyRandom.Range(fourmin, four);
            if (size < 3)
            {
                count2 = 0;
                count1 = 0;
            }
            int count3 = count2 + MyRandom.Range(threemin, three);
            if (size < 2)
            {
                count3 = 0;
                count2 = 0;
                count1 = 0;
            }
            int count4 = count3 + MyRandom.Range(twomin, two);

            // Make sure we are not digging past the river path
            if (count4 > river.Length)
            {
                int extra = count4 - river.Length;
                while (extra > 0)
                {
                    if (count1 > 0) { count1--; count2--; count3--; count4--; extra--; }
                    else if (count2 > 0) { count2--; count3--; count4--; extra--; }
                    else if (count3 > 0) { count3--; count4--; extra--; }
                    else if (count4 > 0) { count4--; extra--; }
                }
            }

            // adjust size of river at intersection point
            if (intersectionSize == 1)
            {
                count4 = intersectionCount;
                count1 = 0;
                count2 = 0;
                count3 = 0;
            }
            else if (intersectionSize == 2)
            {
                count3 = intersectionCount;
                count1 = 0;
                count2 = 0;
            }
            else if (intersectionSize == 3)
            {
                count2 = intersectionCount;
                count1 = 0;
            }
            else if (intersectionSize == 4)
            {
                count1 = intersectionCount;
            }
            else
            {
                count1 = 0;
                count2 = 0;
                count3 = 0;
                count4 = 0;
            }

            // dig out the river
            for (int i = river.Tiles.Count - 1; i >= 0; i--)
            {

                PixelTile t = river.Tiles[i];

                if (counter < count1)
                {
                    t.DigRiver(river, 4);
                }
                else if (counter < count2)
                {
                    t.DigRiver(river, 3);
                }
                else if (counter < count3)
                {
                    t.DigRiver(river, 2);
                }
                else if (counter < count4)
                {
                    t.DigRiver(river, 1);
                }
                else
                {
                    t.DigRiver(river, 0);
                }
                counter++;
            }
        }

        // Dig river
        private void DigRiver(River river)
        {
            int counter = 0;

            // How wide are we digging this river?
            int size = MyRandom.Range(1, 5);
            river.Length = river.Tiles.Count;

            // randomize size change
            int two = river.Length / 2;
            int three = two / 2;
            int four = three / 2;
            int five = four / 2;

            int twomin = two / 3;
            int threemin = three / 3;
            int fourmin = four / 3;
            int fivemin = five / 3;

            // randomize lenght of each size
            int count1 = MyRandom.Range(fivemin, five);
            if (size < 4)
            {
                count1 = 0;
            }
            int count2 = count1 + MyRandom.Range(fourmin, four);
            if (size < 3)
            {
                count2 = 0;
                count1 = 0;
            }
            int count3 = count2 + MyRandom.Range(threemin, three);
            if (size < 2)
            {
                count3 = 0;
                count2 = 0;
                count1 = 0;
            }
            int count4 = count3 + MyRandom.Range(twomin, two);

            // Make sure we are not digging past the river path
            if (count4 > river.Length)
            {
                int extra = count4 - river.Length;
                while (extra > 0)
                {
                    if (count1 > 0) { count1--; count2--; count3--; count4--; extra--; }
                    else if (count2 > 0) { count2--; count3--; count4--; extra--; }
                    else if (count3 > 0) { count3--; count4--; extra--; }
                    else if (count4 > 0) { count4--; extra--; }
                }
            }

            // Dig it out
            for (int i = river.Tiles.Count - 1; i >= 0; i--)
            {
                PixelTile t = river.Tiles[i];

                if (counter < count1)
                {
                    t.DigRiver(river, 4);
                }
                else if (counter < count2)
                {
                    t.DigRiver(river, 3);
                }
                else if (counter < count3)
                {
                    t.DigRiver(river, 2);
                }
                else if (counter < count4)
                {
                    t.DigRiver(river, 1);
                }
                else
                {
                    t.DigRiver(river, 0);
                }
                counter++;
            }
        }

        private void FindPathToWater(PixelTile tile, Direction direction, ref River river)
        {
            if (tile.Rivers.Contains(river))
                return;

            // check if there is already a river on this tile
            if (tile.Rivers.Count > 0)
                river.Intersections++;

            river.AddTile(tile);

            // get neighbors
            PixelTile left = GetLeft(tile);
            PixelTile right = GetRight(tile);
            PixelTile top = GetTop(tile);
            PixelTile bottom = GetBottom(tile);

            float leftValue = int.MaxValue;
            float rightValue = int.MaxValue;
            float topValue = int.MaxValue;
            float bottomValue = int.MaxValue;

            // query height values of neighbors
            if (left != null && left.GetRiverNeighborCount(river) < 2 && !river.Tiles.Contains(left))
                leftValue = left.HeightValue;
            if (right != null && right.GetRiverNeighborCount(river) < 2 && !river.Tiles.Contains(right))
                rightValue = right.HeightValue;
            if (top != null && top.GetRiverNeighborCount(river) < 2 && !river.Tiles.Contains(top))
                topValue = top.HeightValue;
            if (bottom != null && bottom.GetRiverNeighborCount(river) < 2 && !river.Tiles.Contains(bottom))
                bottomValue = bottom.HeightValue;

            // if neighbor is existing river that is not this one, flow into it
            if (bottom != null && bottom.Rivers.Count == 0 && !bottom.Collidable)
                bottomValue = 0;
            if (top != null && top.Rivers.Count == 0 && !top.Collidable)
                topValue = 0;
            if (left != null && left.Rivers.Count == 0 && !left.Collidable)
                leftValue = 0;
            if (right != null && right.Rivers.Count == 0 && !right.Collidable)
                rightValue = 0;

            // override flow direction if a tile is significantly lower
            if (direction == Direction.LEFT)
                if (Math.Abs(rightValue - leftValue) < 0.1f)
                    rightValue = int.MaxValue;
            if (direction == Direction.RIGHT)
                if (Math.Abs(rightValue - leftValue) < 0.1f)
                    leftValue = int.MaxValue;
            if (direction == Direction.UP)
                if (Math.Abs(topValue - bottomValue) < 0.1f)
                    bottomValue = int.MaxValue;
            if (direction == Direction.DOWN)
                if (Math.Abs(topValue - bottomValue) < 0.1f)
                    topValue = int.MaxValue;

            // find mininum
            float min = Math.Min(Math.Min(Math.Min(leftValue, rightValue), topValue), bottomValue);

            // if no minimum found - exit
            if (min == int.MaxValue)
                return;

            //Move to next neighbor
            if (min == leftValue)
            {
                if (left != null && left.Collidable)
                {
                    if (river.CurrentDirection != Direction.LEFT)
                    {
                        river.TurnCount++;
                        river.CurrentDirection = Direction.LEFT;
                    }
                    FindPathToWater(left, direction, ref river);
                }
            }
            else if (min == rightValue)
            {
                if (right != null && right.Collidable)
                {
                    if (river.CurrentDirection != Direction.RIGHT)
                    {
                        river.TurnCount++;
                        river.CurrentDirection = Direction.RIGHT;
                    }
                    FindPathToWater(right, direction, ref river);
                }
            }
            else if (min == bottomValue)
            {
                if (bottom != null && bottom.Collidable)
                {
                    if (river.CurrentDirection != Direction.DOWN)
                    {
                        river.TurnCount++;
                        river.CurrentDirection = Direction.DOWN;
                    }
                    FindPathToWater(bottom, direction, ref river);
                }
            }
            else if (min == topValue)
            {
                if (top != null && top.Collidable)
                {
                    if (river.CurrentDirection != Direction.UP)
                    {
                        river.TurnCount++;
                        river.CurrentDirection = Direction.UP;
                    }
                    FindPathToWater(top, direction, ref river);
                }
            }
        }

        // Build a Tile array from our data
        private void LoadTiles()
        {
            Tiles = new PixelTile[Width, Height];

            for (var x = 0; x < Width; x++)
            {
                for (var y = 0; y < Height; y++)
                {
                    PixelTile t = new PixelTile();
                    t.X = x;
                    t.Y = y;

                    //set heightmap value
                    float heightValue = HeightData.Data[x, y];
                    heightValue = (heightValue - HeightData.Min) / (HeightData.Max - HeightData.Min);
                    t.HeightValue = heightValue;


                    if (heightValue < DeepWater)
                    {
                        t.HeightType = HeightType.DeepWater;
                        t.Collidable = false;
                    }
                    else if (heightValue < ShallowWater)
                    {
                        t.HeightType = HeightType.ShallowWater;
                        t.Collidable = false;
                    }
                    else if (heightValue < Sand)
                    {
                        t.HeightType = HeightType.Sand;
                        t.Collidable = true;
                    }
                    else if (heightValue < Grass)
                    {
                        t.HeightType = HeightType.Grass;
                        t.Collidable = true;
                    }
                    else if (heightValue < Forest)
                    {
                        t.HeightType = HeightType.Forest;
                        t.Collidable = true;
                    }
                    else if (heightValue < Rock)
                    {
                        t.HeightType = HeightType.Rock;
                        t.Collidable = true;
                    }
                    else
                    {
                        t.HeightType = HeightType.Snow;
                        t.Collidable = true;
                    }


                    //adjust moisture based on height
                    if (t.HeightType == HeightType.DeepWater)
                    {
                        MoistureData.Data[t.X, t.Y] += 8f * t.HeightValue;
                    }
                    else if (t.HeightType == HeightType.ShallowWater)
                    {
                        MoistureData.Data[t.X, t.Y] += 3f * t.HeightValue;
                    }
                    else if (t.HeightType == HeightType.Shore)
                    {
                        MoistureData.Data[t.X, t.Y] += 1f * t.HeightValue;
                    }
                    else if (t.HeightType == HeightType.Sand)
                    {
                        MoistureData.Data[t.X, t.Y] += 0.2f * t.HeightValue;
                    }

                    //Moisture Map Analyze	
                    float moistureValue = MoistureData.Data[x, y];
                    moistureValue = (moistureValue - MoistureData.Min) / (MoistureData.Max - MoistureData.Min);
                    t.MoistureValue = moistureValue;

                    //set moisture type
                    if (moistureValue < DryerValue) t.MoistureType = MoistureType.Dryest;
                    else if (moistureValue < DryValue) t.MoistureType = MoistureType.Dryer;
                    else if (moistureValue < WetValue) t.MoistureType = MoistureType.Dry;
                    else if (moistureValue < WetterValue) t.MoistureType = MoistureType.Wet;
                    else if (moistureValue < WettestValue) t.MoistureType = MoistureType.Wetter;
                    else t.MoistureType = MoistureType.Wettest;


                    // Adjust Heat Map based on Height - Higher == colder
                    if (t.HeightType == HeightType.Forest)
                    {
                        HeatData.Data[t.X, t.Y] -= 0.1f * t.HeightValue;
                    }
                    else if (t.HeightType == HeightType.Rock)
                    {
                        HeatData.Data[t.X, t.Y] -= 0.25f * t.HeightValue;
                    }
                    else if (t.HeightType == HeightType.Snow)
                    {
                        HeatData.Data[t.X, t.Y] -= 0.4f * t.HeightValue;
                    }
                    else
                    {
                        HeatData.Data[t.X, t.Y] += 0.01f * t.HeightValue;
                    }

                    // Set heat value
                    float heatValue = HeatData.Data[x, y];
                    heatValue = (heatValue - HeatData.Min) / (HeatData.Max - HeatData.Min);
                    t.HeatValue = heatValue;

                    // set heat type
                    if (heatValue < ColdestValue) t.HeatType = HeatType.Coldest;
                    else if (heatValue < ColderValue) t.HeatType = HeatType.Colder;
                    else if (heatValue < ColdValue) t.HeatType = HeatType.Cold;
                    else if (heatValue < WarmValue) t.HeatType = HeatType.Warm;
                    else if (heatValue < WarmerValue) t.HeatType = HeatType.Warmer;
                    else t.HeatType = HeatType.Warmest;

                    Tiles[x, y] = t;
                }
            }
        }

        private void UpdateNeighbors()
        {
            for (var x = 0; x < Width; x++)
            {
                for (var y = 0; y < Height; y++)
                {
                    PixelTile t = Tiles[x, y];

                    t.Top = GetTop(t);
                    t.Bottom = GetBottom(t);
                    t.Left = GetLeft(t);
                    t.Right = GetRight(t);
                }
            }
        }

    }
}
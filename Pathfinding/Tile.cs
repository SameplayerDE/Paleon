using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Paleon
{
    public enum BlockType
    {
        STONE,
        CLAY,
        NONE
    }

    public enum GroundType
    {
        GROUND,
        NONE
    }

    public enum GroundTopType
    {
        GRASS,
        FARMLAND,
        WATER,
        NONE
    }

    public class Tile
    {

        public int X { get; private set; }
        public int Y { get; private set; }

        private bool isWalkable = true;
        public bool IsWalkable
        {
            get { return isWalkable; }
            set
            {
                if (isWalkable == value)
                    return;

                isWalkable = value;

                Chunk.UpdateTile(this);

                foreach (SettlerCmp settler in GameplayScene.Settlers)
                {
                    settler.Pathfinder?.RebuildPath();
                }
            }
        }

        public List<Tile> AllNeighbours;
        public List<Tile> Neighbours;

        internal TileMap GroundTileMap;
        internal TileMap GroundTopTileMap;
        internal TileMap BlockTileMap;
        internal TileMap ItemTileMap;
        internal TileMap MarkTileMap;

        public Room Room;
        public MChunk Chunk { get; internal set; }

        public Area Area { get; set; }

        public Block Block { get; private set; }

        public void RemoveBlock()
        {
            Block = null;
            BlockType = BlockType.NONE; 
            IsWalkable = true;
        }

        public void SetBlock(Block block)
        {
            Block = block;
            BlockType = Block.BlockType;
            IsWalkable = Block.IsWalkable;
        }


        // TODO: запретить доступ к этой коллекции напрямую
        internal List<ItemContainer> ItemContainers { get; set; }

        private BlockType blockType = BlockType.NONE;
        public BlockType BlockType
        {
            get { return blockType; }
            private set
            {
                if (blockType == value)
                    return;

                blockType = value;

                UpdateBlock(this);

                foreach (Tile tile in AllNeighbours)
                    UpdateBlock(tile);
            }
        }

        private GroundType groundType = GroundType.NONE;
        public GroundType GroundType
        {
            get { return groundType; }
            set
            {
                groundType = value;
                switch (groundType)
                {
                    case GroundType.GROUND:
                        {
                            GroundTileMap.SetCell(X, Y, 7);
                            IsWalkable = true;
                        }
                        break;
                }
            }
        }

        private GroundTopType groundTopType = GroundTopType.NONE;
        public GroundTopType GroundTopType
        {
            get { return groundTopType; }
            set
            {
                if (groundTopType == value)
                    return;

                groundTopType = value;

                UpdateGroundTop(this);

                foreach (Tile tile in AllNeighbours)
                    UpdateGroundTop(tile);

                if (groundTopType == GroundTopType.WATER)
                    IsWalkable = false;
                else
                    IsWalkable = true;
            }
        }

        public void UpdateGroundTop(Tile tile)
        {
            Tile leftTopTile = GameplayScene.GetTile(tile.X - 1, tile.Y - 1);
            Tile topTile = GameplayScene.GetTile(tile.X, tile.Y - 1);
            Tile rightTopTile = GameplayScene.GetTile(tile.X + 1, tile.Y - 1);

            Tile leftTile = GameplayScene.GetTile(tile.X - 1, tile.Y);
            Tile rightTile = GameplayScene.GetTile(tile.X + 1, tile.Y);

            Tile leftBottomTile = GameplayScene.GetTile(tile.X - 1, tile.Y + 1);
            Tile bottomTile = GameplayScene.GetTile(tile.X, tile.Y + 1);
            Tile rightBottomTile = GameplayScene.GetTile(tile.X + 1, tile.Y + 1);

            int leftTop = (leftTopTile != null && leftTopTile.GroundTopType == tile.GroundTopType) ? 1 : 0;
            int top = (topTile != null && topTile.GroundTopType == tile.GroundTopType) ? 2 : 0;
            int rightTop = (rightTopTile != null && rightTopTile.GroundTopType == tile.GroundTopType) ? 4 : 0;

            int left = (leftTile != null && leftTile.GroundTopType == tile.GroundTopType) ? 8 : 0;
            int right = (rightTile != null && rightTile.GroundTopType == tile.GroundTopType) ? 16 : 0;

            int leftBottom = (leftBottomTile != null && leftBottomTile.GroundTopType == tile.GroundTopType) ? 32 : 0;
            int bottom = (bottomTile != null && bottomTile.GroundTopType == tile.GroundTopType) ? 64 : 0;
            int rightBottom = (rightBottomTile != null && rightBottomTile.GroundTopType == tile.GroundTopType) ? 128 : 0;

            int bitmask = leftTop + top + rightTop + left + right + leftBottom + bottom + rightBottom;

            if (tile.GroundTopType == GroundTopType.WATER)
            {
                int random = MyRandom.Range(20);

                if (bitmask == 255 && random == 1)
                {
                    GroundTopTileMap.SetCell(tile.X, tile.Y, 148);
                }
                else if (bitmask == 255 && random == 2)
                {
                    GroundTopTileMap.SetCell(tile.X, tile.Y, 149);
                }
                else
                {
                    GroundTopTileMap.SetCell(tile.X, tile.Y, BitmaskGenerator.GetTileNumber(bitmask));
                }
            }
            else if (tile.GroundTopType == GroundTopType.FARMLAND)
            {
                GroundTopTileMap.SetCell(tile.X, tile.Y, BitmaskGenerator.GetTileNumber(bitmask) + 48);
            }
            else if (tile.GroundTopType == GroundTopType.GRASS)
            {
                int random = MyRandom.Range(50);

                if (bitmask == 255 && (random >= 1 && random <= 5))
                {
                    GroundTopTileMap.SetCell(tile.X, tile.Y, 144);
                }
                else if (bitmask == 255 && random == 6)
                {
                    GroundTopTileMap.SetCell(tile.X, tile.Y, 145);
                }
                else if (bitmask == 255 && random == 7)
                {
                    GroundTopTileMap.SetCell(tile.X, tile.Y, 146);
                }
                else if (bitmask == 255 && random == 8)
                {
                    GroundTopTileMap.SetCell(tile.X, tile.Y, 147);
                }
                else
                {
                    GroundTopTileMap.SetCell(tile.X, tile.Y, BitmaskGenerator.GetTileNumber(bitmask) + 96);
                }
            }
        }

        public void UpdateBlock(Tile tile)
        {
            if (tile.BlockType != BlockType.NONE)
            {
                Tile topTile = GameplayScene.GetTile(tile.X, tile.Y - 1);
                Tile leftTile = GameplayScene.GetTile(tile.X - 1, tile.Y);
                Tile rightTile = GameplayScene.GetTile(tile.X + 1, tile.Y);
                Tile bottomTile = GameplayScene.GetTile(tile.X, tile.Y + 1);

                int top = (topTile != null && topTile.BlockType != BlockType.NONE) ? 1 : 0;
                int left = (leftTile != null && leftTile.BlockType != BlockType.NONE) ? 2 : 0;
                int right = (rightTile != null && rightTile.BlockType != BlockType.NONE) ? 4 : 0;
                int bottom = (bottomTile != null && bottomTile.BlockType != BlockType.NONE) ? 8 : 0;

                int bitmask = top + left + right + bottom;

                if (tile.blockType == BlockType.STONE)
                    bitmask += 16;
                if (tile.BlockType == BlockType.CLAY)
                    bitmask += 32;

                BlockTileMap.SetCell(tile.X, tile.Y, bitmask);
            }
            else
            {
                BlockTileMap.SetCell(tile.X, tile.Y, 0);
            }
        }

        private Entity entity;
        public Entity Entity 
        { 
            get { return entity; }
            set 
            {
                if (entity != null)
                    throw new Exception("Old entity not deleted!");

                entity = value;
            } 
        }

        public void RemoveEntity()
        {
            entity = null;
            // Reset walkability (check if there was unwalkable tile)
            IsWalkable = true;
        }

        public Tile(int x, int y, TileMap groundTileMap, TileMap groundTopTileMap, TileMap blockTileMap, TileMap itemTileMap, TileMap markTileMap)
        {
            X = x;
            Y = y;

            GroundTileMap = groundTileMap;
            GroundTopTileMap = groundTopTileMap;
            BlockTileMap = blockTileMap;
            ItemTileMap = itemTileMap;
            MarkTileMap = markTileMap;

            AllNeighbours = new List<Tile>();
            Neighbours = new List<Tile>();

            ItemContainers = new List<ItemContainer>();
        }

        public void SetMark(int tileId)
        {
            MarkTileMap.SetCell(X, Y, tileId);
        }

        public ItemContainer CreateItemContainer(Item item)
        {
            if (BlockType != BlockType.NONE)
                throw new Exception("Remove block before tile setting!");

            ItemContainer itemCntr = new ItemContainer(this);
            ItemContainers.Add(itemCntr);
            itemCntr.Item = item;

            return itemCntr;
        }

        public ItemContainer CreateItemContainer(SettlerCmp owner)
        {
            ItemContainer itemCntr = new ItemContainer(this);
            ItemContainers.Add(itemCntr);
            itemCntr.Owner = owner;

            return itemCntr;
        }
    }
}

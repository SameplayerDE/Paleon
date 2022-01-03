using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paleon
{
    public class Block
    {

        public BlockType BlockType { get; private set; }
        public bool IsWalkable { get; private set; }

        public Item Resource { get; private set; }

        public MineLabor MineLabor { get; set; }

        public int MaxHardness { get; private set; }
        public int CurrentHardness { get; set; }

        public Block(BlockType blockType, bool isWalkable, Item resource, int maxHardness)
        {
            BlockType = blockType;
            IsWalkable = isWalkable;
            Resource = resource;
            MaxHardness = maxHardness;

            CurrentHardness = maxHardness;
        }

    }
}

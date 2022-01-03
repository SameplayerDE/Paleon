using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paleon
{
    public class ItemDatabase
    {
        public static Item STONE { get; private set; }
        public static Item LOG { get; private set; }
        public static Item FLINT_AXE { get; private set; }
        public static Item FLINT_PICKAXE { get; private set; }
        public static Item WOODEN_HARPOON { get; private set; }
        public static Item OAT_SEEDS { get; private set; }
        public static Item RASPBERRY { get; private set; }
        public static Item RAW_FISH { get; private set; }
        public static Item COOKED_FISH { get; private set; }
        public static Item RAW_MEAT { get; private set; }
        public static Item COOKED_MEAT { get; private set; }
        public static Item FLINT { get; private set; }
        public static Item STICK { get; private set; }
        public static Item WOODEN_SPEAR { get; private set; }
        public static Item SHARP_FLINT { get; private set; }
        public static Item CORPSE { get; private set; }
        public static Item HAY { get; private set; }
        public static Item WHEAT_FLOUR { get; private set; }
        public static Item RAW_SKIN { get; private set; }
        public static Item DRY_SKIN { get; private set; }
        public static Item CLAM { get; private set; }
        public static Item PEAR { get; private set; }
        public static Item ASH { get; private set; }
        public static Item FAT { get; private set; }
        public static Item BONE { get; private set; }
        public static Item FLATBREAD { get; private set; }
        public static Item CLAY { get; private set; }
        public static Item ACORN { get; private set; }
        public static Item FIREWOOD { get; private set; }
        public static Item BARLEY_SEEDS { get; private set; }
        public static Item FLINT_SICKLE { get; private set; }
        public static Item CURRANT { get; private set; }
        public static Item FUR_CLOTHING { get; private set; }
        public static Item JERKY { get; private set; }
        public static Item STOCKFISH { get; private set; }
        public static Item BURNT_CLAY_JUG { get; private set; }

        public static void Initialize()
        {
            Tileset itemsTileset = new Tileset(ResourceManager.GetTexture("tileset"), 16, 16);

            STONE = new Item(20, itemsTileset[20], "Stone", 0, 0, ItemType.NONE);
            LOG = new Item(15, itemsTileset[15], "Log", 0, 0, ItemType.NONE);
            FLINT = new Item(9, itemsTileset[9], "Flint", 0, 0, ItemType.NONE);
            STICK = new Item(8, itemsTileset[8], "Stick", 0, 0, ItemType.NONE);

            FLINT_AXE = new Item(44, itemsTileset[44], "Flint axe", 0, 0, ItemType.NONE);

            FLINT_PICKAXE = new Item(45, itemsTileset[45], "Flint pickaxe", 0, 0, ItemType.PICKAXE);

            WOODEN_HARPOON = new Item(14, itemsTileset[14], "Wooden harpoon", 0, 0, ItemType.HARPOON);

            WOODEN_SPEAR = new Item(12, itemsTileset[12], "Wooden spear", 0, 5, ItemType.SPEAR);

            SHARP_FLINT = new Item(13, itemsTileset[13], "Sharp flint", 0, 3, ItemType.KNIFE);

            OAT_SEEDS = new Item(30, itemsTileset[30], "Oat seeds", 0, 0, ItemType.NONE);

            RASPBERRY = new Item(11, itemsTileset[11], "Raspberry", 10, 0, ItemType.FOOD);

            RAW_FISH = new Item(28, itemsTileset[28], "Raw fish", 25, 0, ItemType.FOOD);

            COOKED_FISH = new Item(27, itemsTileset[27], "Cooked fish", 15, 0, ItemType.FOOD);

            RAW_MEAT = new Item(43, itemsTileset[43], "Raw meat", 30, 0, ItemType.FOOD);

            COOKED_MEAT = new Item(26, itemsTileset[26], "Cooked meat", 20, 0, ItemType.FOOD);

            HAY = new Item(10, itemsTileset[10], "Hay", 0, 0, ItemType.NONE);

            WHEAT_FLOUR = new Item(40, itemsTileset[40], "Oat flour", 0, 0, ItemType.NONE);

            RAW_SKIN = new Item(42, itemsTileset[42], "Raw skin", 0, 0, ItemType.NONE);
            
            DRY_SKIN = new Item(46, itemsTileset[46], "Dry skin", 0, 0, ItemType.NONE);

            CLAM = new Item(39, itemsTileset[39], "Clam", 10, 0, ItemType.NONE);

            PEAR = new Item(29, itemsTileset[29], "Pear", 8, 0, ItemType.FOOD);

            ASH = new Item(47, itemsTileset[47], "Ash", 0, 0, ItemType.NONE);

            FAT = new Item(37, itemsTileset[37], "Fat", 0, 0, ItemType.NONE);

            BONE = new Item(41, itemsTileset[41], "Bone", 0, 0, ItemType.NONE);

            FLATBREAD = new Item(21, itemsTileset[21], "Flatbread", 15, 0, ItemType.FOOD);

            CLAY = new Item(19, itemsTileset[19], "Clay", 0, 0, ItemType.NONE);

            ACORN = new Item(53, itemsTileset[53], "Acorn", 0, 0, ItemType.NONE);
            JERKY = new Item(54, itemsTileset[54], "Jerky", 25, 0, ItemType.FOOD);
            STOCKFISH = new Item(55, itemsTileset[55], "Stockfish", 20, 0, ItemType.FOOD);

            FIREWOOD = new Item(31, itemsTileset[31], "Firewood", 0, 0, ItemType.NONE);

            BARLEY_SEEDS = new Item(22, itemsTileset[22], "Barley seeds", 0, 0, ItemType.NONE);

            FLINT_SICKLE = new Item(50, itemsTileset[50], "Flint sickle", 0, 4, ItemType.SICKLE);

            CURRANT = new Item(52, itemsTileset[52], "Currant", 5, 0, ItemType.FOOD);

            FUR_CLOTHING = new Item(34, itemsTileset[34], "Fur clothing", 0, 0, ItemType.CLOTHING);
            BURNT_CLAY_JUG = new Item(1, itemsTileset[1], "Burnt clay jug", 0, 0, ItemType.CONTAINER);
        }
    }
}

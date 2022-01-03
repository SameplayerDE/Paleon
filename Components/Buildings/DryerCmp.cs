using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paleon
{
    public class DryerCmp : LongCrafterBuildingCmp
    {

        public DryerCmp() : base("dryer")
        {
            ItemsToCraft.Add(new CraftingRecipe(ItemDatabase.DRY_SKIN, ItemDatabase.RAW_SKIN), 0);
            ItemsToCraft.Add(new CraftingRecipe(ItemDatabase.JERKY, ItemDatabase.RAW_MEAT), 0);
            ItemsToCraft.Add(new CraftingRecipe(ItemDatabase.STOCKFISH, ItemDatabase.RAW_FISH), 0);

            AddBuildingItem(ItemDatabase.STICK, 1);
        }

    }
}

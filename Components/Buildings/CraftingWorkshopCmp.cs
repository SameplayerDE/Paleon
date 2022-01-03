using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paleon
{
    public class CraftingWorkshopCmp : QuickCrafterBuildingCmp
    {

        public CraftingWorkshopCmp() : base("crafting_workshop")
        {
            ItemsToCraft.Add(new CraftingRecipe(ItemDatabase.SHARP_FLINT, ItemDatabase.FLINT), 0);
            ItemsToCraft.Add(new CraftingRecipe(ItemDatabase.WOODEN_SPEAR, ItemDatabase.STICK), 0);
            ItemsToCraft.Add(new CraftingRecipe(ItemDatabase.FLINT_AXE, ItemDatabase.FLINT), 0);
            ItemsToCraft.Add(new CraftingRecipe(ItemDatabase.FLINT_PICKAXE, ItemDatabase.FLINT), 0);

            AddBuildingItem(ItemDatabase.FLINT, 2);
        }
    }
}

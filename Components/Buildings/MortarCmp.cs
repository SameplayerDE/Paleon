using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paleon
{
    public class MortarCmp : QuickCrafterBuildingCmp
    {

        public MortarCmp() : base("mortar")
        {
            
            ItemsToCraft.Add(new CraftingRecipe(ItemDatabase.WHEAT_FLOUR, ItemDatabase.OAT_SEEDS), 0);

            AddBuildingItem(ItemDatabase.STONE, 1);
        }
    }
}

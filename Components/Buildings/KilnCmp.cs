using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paleon
{
    public class KilnCmp : QuickCrafterBuildingCmp
    {

        public KilnCmp() : base("kiln")
        {
            ItemsToCraft.Add(new CraftingRecipe(ItemDatabase.BURNT_CLAY_JUG, ItemDatabase.CLAY), 0);

            AddBuildingItem(ItemDatabase.STONE, 4);
        }

    }
}

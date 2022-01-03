using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paleon
{
    public class FirewoodWorkshopCmp : QuickCrafterBuildingCmp
    {

        public FirewoodWorkshopCmp() : base("firewood_workshop")
        {
            ItemsToCraft.Add(new CraftingRecipe(ItemDatabase.FIREWOOD, ItemDatabase.LOG), 0);

            AddBuildingItem(ItemDatabase.LOG, 1);
        }

    }
}

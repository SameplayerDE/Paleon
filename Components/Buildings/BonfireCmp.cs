namespace Paleon
{
    public class BonfireCmp : QuickCrafterBuildingCmp
    {

        public BonfireCmp() : base("bonfire")
        {
            ItemsToCraft.Add(new CraftingRecipe(ItemDatabase.COOKED_FISH, ItemDatabase.RAW_FISH), 0);
            ItemsToCraft.Add(new CraftingRecipe(ItemDatabase.COOKED_MEAT, ItemDatabase.RAW_MEAT), 0);
            ItemsToCraft.Add(new CraftingRecipe(ItemDatabase.ASH, ItemDatabase.STICK), 0);
            ItemsToCraft.Add(new CraftingRecipe(ItemDatabase.FLATBREAD, ItemDatabase.WHEAT_FLOUR), 0);

            AddBuildingItem(ItemDatabase.FLINT, 1);
        }

    }
}

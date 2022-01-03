using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paleon
{
    public class CrafterBuildingCmp : BuildingCmp
    {

        public Dictionary<CraftingRecipe, int> ItemsToCraft { get; private set; }

        private Action<CraftingRecipe, int> cbOnCountChanged;

        public CrafterBuildingCmp(string name) : base(name)
        {
            ItemsToCraft = new Dictionary<CraftingRecipe, int>();
        }

        public virtual void Process()
        {

        }

        public void AddItemToCraft(CraftingRecipe item)
        {
            if (ItemsToCraft[item] < 10)
                ItemsToCraft[item] += 1;

            cbOnCountChanged?.Invoke(item, ItemsToCraft[item]);
        }

        public void SubItemToCraft(CraftingRecipe item)
        {
            if (ItemsToCraft[item] > 0)
                ItemsToCraft[item] -= 1;

            cbOnCountChanged?.Invoke(item, ItemsToCraft[item]);
        }

        public void AddOnCountChangedCallback(Action<CraftingRecipe, int> callback)
        {
            cbOnCountChanged += callback;
        }

        public void RemoveOnCountChangedCallback(Action<CraftingRecipe, int> callback)
        {
            cbOnCountChanged -= callback;
        }

    }
}

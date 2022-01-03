using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paleon
{
    public class QuickCraftLabor : Labor
    {

        private CrafterBuildingCmp crafterCmp;

        private Tile targetTile; // Тайл строения
        private bool adjacent;
        private ItemContainer stockpileItemCntr; // Складской тайл с игредиентом для крафта

        private CraftingRecipe craftingRecipe;

        public QuickCraftLabor(CrafterBuildingCmp crafterCmp) : base(LaborType.Crafter)
        {
            this.crafterCmp = crafterCmp;
        }

        public override bool Check(SettlerCmp settler)
        {
            targetTile = null;

            // Check if building is available
            for(int i = 0; i < crafterCmp.OccupiedTiles.Count; i++)
            {
                if (crafterCmp.OccupiedTiles[i].IsTarget)
                {
                    targetTile = crafterCmp.OccupiedTiles[i].Tile;

                    if (crafterCmp.OccupiedTiles[i].IsWalkable)
                        adjacent = false;
                    else
                        adjacent = true;

                    if (settler.Pathfinder.IsPathAvailable(targetTile, adjacent))
                        break;
                }
            }

            if (targetTile == null)
                return false;

            // Check if building has items to craft and if stockpile has ingredients
            foreach (KeyValuePair<CraftingRecipe, int> entry in crafterCmp.ItemsToCraft)
            {
                if (entry.Value > 0)
                {
                    craftingRecipe = entry.Key;
                    stockpileItemCntr = GameplayScene.WorldManager.StorageManager.GetItemContainerWith(settler.Pathfinder.CurrentTile.Room.Id,
                        entry.Key.Ingredients[0]);

                    if (stockpileItemCntr != null)
                        break;
                }
            }

            if (stockpileItemCntr == null)
                return false;

            return true;
        }

        public override void Begin(SettlerCmp settler)
        {
            tasks.Add(new GotoTakeTask(stockpileItemCntr, false));
            tasks.Add(new GotoCraftTask(crafterCmp, targetTile, craftingRecipe, adjacent));

            base.Begin(settler);
        }

    }
}

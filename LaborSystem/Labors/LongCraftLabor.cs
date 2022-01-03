using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paleon
{
    public class LongCraftLabor : Labor
    {
        private LongCrafterBuildingCmp longCrafterCmp;

        private Tile targetTile; // Тайл строения
        private bool adjacent;
        private ItemContainer stockpileItemCntr; // Складской тайл с игредиентом для крафта

        private CraftingRecipe craftingRecipe;

        public GotoTakeTask TakeTask { get; private set; }
        public GotoPutTask PutTask { get; private set; }

        public LongCraftLabor(LongCrafterBuildingCmp longCrafter) : base(LaborType.Crafter)
        {
            longCrafterCmp = longCrafter;
        }

        public override bool Check(SettlerCmp settler)
        {
            targetTile = null;

            // Check if building is available
            for (int i = 0; i < longCrafterCmp.OccupiedTiles.Count; i++)
            {
                if (longCrafterCmp.OccupiedTiles[i].IsTarget)
                {
                    targetTile = longCrafterCmp.OccupiedTiles[i].Tile;

                    if (longCrafterCmp.OccupiedTiles[i].IsWalkable)
                        adjacent = false;
                    else
                        adjacent = true;

                    if (settler.Pathfinder.IsPathAvailable(targetTile, adjacent))
                        break;
                }
            }

            if (targetTile == null)
                return false;

            // Check if crafter has items to craft and if stockpile has ingredients
            foreach (KeyValuePair<CraftingRecipe, int> entry in longCrafterCmp.ItemsToCraft)
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
            longCrafterCmp.CurrentCraftingRecipe = craftingRecipe;
            tasks.Add(TakeTask = new GotoTakeTask(stockpileItemCntr, false));
            tasks.Add(PutTask = new GotoPutTask(targetTile, true, null));

            base.Begin(settler);
        }
    }
}

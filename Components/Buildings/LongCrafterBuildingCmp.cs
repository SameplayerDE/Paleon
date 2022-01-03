using System.Collections.Generic;

namespace Paleon
{
    public class LongCrafterBuildingCmp : CrafterBuildingCmp
    {

        private Timer timer;
        private bool craftingStarted = false;

        private ItemContainer itemCntr;

        public CraftingRecipe CurrentCraftingRecipe { get; set; }

        public LongCrafterBuildingCmp(string json) : base(json)
        {
            timer = new Timer();
        }

        public override void Update()
        {
            base.Update();

            if(craftingStarted)
            {
                if(timer.GetTime() > 10)
                {
                    itemCntr.Item = CurrentCraftingRecipe.Result;
                    ItemsToCraft[CurrentCraftingRecipe] -= 1;
                    CurrentCraftingRecipe = null;
                    craftingStarted = false;

                    HaulLabor haulLabor = new HaulLabor(itemCntr, true);
                    haulLabor.AddOnLaborCompletedCallback(OnHaulCompleted);
                    GameplayScene.WorldManager.LaborManager.Add(haulLabor);
                }
            }
        }

        public override void Complete()
        {
            base.Complete();

            LongCraftLabor craftLabor = new LongCraftLabor(this);
            craftLabor.AddOnLaborCompletedCallback(OnDeliveringCompleted);
            GameplayScene.WorldManager.LaborManager.Add(craftLabor);
        }

        private void OnDeliveringCompleted(Labor labor)
        {
            labor.RemoveOnLaborCompletedCallback(OnDeliveringCompleted);

            itemCntr = (labor as LongCraftLabor).PutTask.ItemCntr;

            craftingStarted = true;
            timer.Reset();
        }

        private void OnHaulCompleted(Labor labor)
        {
            labor.RemoveOnLaborCompletedCallback(OnHaulCompleted);

            LongCraftLabor craftLabor = new LongCraftLabor(this);
            craftLabor.AddOnLaborCompletedCallback(OnDeliveringCompleted);
            GameplayScene.WorldManager.LaborManager.Add(craftLabor);
        }
    }
}

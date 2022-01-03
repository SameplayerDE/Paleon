using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paleon
{
    public class QuickCrafterBuildingCmp : CrafterBuildingCmp
    {

        private QuickCraftLabor craftLabor;

        public QuickCrafterBuildingCmp(string json) : base(json)
        {

        }

        public override void Complete()
        {
            base.Complete();

            craftLabor = new QuickCraftLabor(this);
            craftLabor.AddOnLaborCompletedCallback(OnLaborCompleted);
            GameplayScene.WorldManager.LaborManager.Add(craftLabor);
        }

        private void OnLaborCompleted(Labor labor)
        {
            labor.RemoveOnLaborCompletedCallback(OnLaborCompleted);

            craftLabor = new QuickCraftLabor(this);
            craftLabor.AddOnLaborCompletedCallback(OnLaborCompleted);
            GameplayScene.WorldManager.LaborManager.Add(craftLabor);
        }

    }
}

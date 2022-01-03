using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paleon
{
    public class GotoCraftTask : Task
    {

        private CrafterBuildingCmp crafter;

        private Tile targetTile;

        private bool adjacent;

        private CraftingRecipe craftingRecipe;

        private Timer timer;

        private Item processItem;

        public GotoCraftTask(CrafterBuildingCmp crafter, Tile tile, CraftingRecipe craftingRecipe, bool adjacent)
        {
            this.crafter = crafter;

            targetTile = tile;

            this.adjacent = adjacent;

            this.craftingRecipe = craftingRecipe;
        }

        public override bool Check(SettlerCmp settler)
        {
            return settler.Pathfinder.IsPathAvailable(targetTile, adjacent);
        }

        public override void Begin(SettlerCmp settler)
        {
            
        }

        public override void BeforeUpdate(SettlerCmp settler)
        {
            settler.Pathfinder.SetPath(targetTile, adjacent);
            processItem = settler.Inventory.Cargo;

            timer = new Timer();
        }

        public override TaskState Update(SettlerCmp settler)
        {
            switch (settler.Pathfinder.MovementState)
            {
                case MovementState.Success:
                    {
                        float currentTime = timer.GetTime();

                        //settler.Slider.SetValue(currentTime, 2);

                        if (currentTime > 2)
                        {
                            // Удаляем предмет с инвентаря поселенца
                            settler.Inventory.Cargo = null;
                            // Скрафченный предмет кладем на земелю
                            Tile tile = settler.Pathfinder.CurrentTile;
                            ItemContainer ic = tile.CreateItemContainer(craftingRecipe.Result);
                            // Создаем работу по переносу созданного предмета
                            GameplayScene.WorldManager.LaborManager.Add(new HaulLabor(ic, false));


                            //settler.Slider.Reset();
                            //settler.Slider.Visible = false;

                            crafter.SubItemToCraft(craftingRecipe);

                            State = TaskState.Success;
                        }
                        else
                        {
                            //settler.Slider.Visible = true;

                            crafter.Process();

                            State = TaskState.Running;
                        }

                        settler.PlayIdleAnimation();
                    }
                    break;
                case MovementState.Fail:
                    {
                        settler.PlayIdleAnimation();
                        State = TaskState.Fail;
                    }
                    break;
                case MovementState.Running:
                    {
                        settler.PlayWalkAnimation();
                        State = TaskState.Running;
                    }
                    break;
            }

            return State;
        }

        public override void Cancel(SettlerCmp settler)
        {
            settler.Pathfinder.ResetPath();

            //settler.Slider.Visible = false;
            //settler.Slider.Reset();

            // Выкидывем предмет из груза
        }

    }
}

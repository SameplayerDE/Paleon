using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paleon
{
    public class GotoHarvestTask : Task
    {
       
        private Tile targetTile;

        private Timer timer;

        public GotoHarvestTask(Tile tile)
        {
            targetTile = tile;
        }

        public override bool Check(SettlerCmp settler)
        {
            if (settler.Pathfinder.IsPathAvailable(targetTile, true))
            {
                SaplingCmp plant = targetTile.Entity.Get<SaplingCmp>();
                if (plant.Fruit != null && plant.FruitCount > 0)
                    return true;
            }

            return false;
        }

        public override void Begin(SettlerCmp settler)
        {
        }

        public override void BeforeUpdate(SettlerCmp settler)
        {
            settler.Pathfinder.SetPath(targetTile, true);

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
                            SaplingCmp plant = targetTile.Entity.Get<SaplingCmp>();

                            plant.DoHarvest(settler);

                            // Создаем работу по переносу фруктов
                            List<ItemContainer> ics = settler.Pathfinder.CurrentTile.ItemContainers;
                            for (int i = 0; i < ics.Count; ++i)
                            {
                                GameplayScene.WorldManager.LaborManager.Add(new HaulLabor(ics[i], false));
                            }

                            //settler.Slider.Reset();
                            //settler.Slider.Visible = false;

                            cbOnCompleted?.Invoke(this);

                            settler.PlayIdleAnimation();
                            State = TaskState.Success;
                        }
                        else
                        {
                            settler.PlayIdleAnimation();
                            //settler.Slider.Visible = true;
                            State = TaskState.Running;
                        }
                    }
                    break;
                case MovementState.Fail:
                    {
                        settler.PlayWalkAnimation();
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

            cbOnCompleted?.Invoke(this);

            //settler.Slider.Visible = false;
            //settler.Slider.Reset();
        }
    }
}

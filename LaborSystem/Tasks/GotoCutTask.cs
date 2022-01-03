
using System.Collections.Generic;

namespace Paleon
{
    public class GotoCutTask : Task
    {

        private Tile targetTile;

        private Timer timer;

        public GotoCutTask(Tile tile)
        {
            targetTile = tile;
        }

        public override bool Check(SettlerCmp settler)
        {
            return settler.Pathfinder.IsPathAvailable(targetTile, true);
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

                        //settler.Slider.SetValue(currentTime, 5);

                        if (currentTime > 5)
                        {
                            SaplingCmp sapling = targetTile.Entity.Get<SaplingCmp>();
                            sapling.DoChop(settler);

                            // Создаем работу по переносу ресурсов
                            List<ItemContainer> ics = targetTile.ItemContainers;
                            for(int i = 0; i < ics.Count; ++i)
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

            //settler.Slider.Visible = false;
            //settler.Slider.Reset();
        }
    }
}

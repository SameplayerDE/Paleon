using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paleon
{
    public class GotoHuntTask : Task
    {

        private AnimalCmp animal;

        private Tile targetTile;

        private Timer damagerTimer;

        public GotoHuntTask(AnimalCmp animal)
        {
            this.animal = animal;
            damagerTimer = new Timer();
        }

        public override bool Check(SettlerCmp settler)
        {
            return settler.Pathfinder.IsPathAvailable(animal.Pathfinder.CurrentTile, true);
        }

        public override void Begin(SettlerCmp settler)
        {
            targetTile = animal.Pathfinder.CurrentTile;
        }

        public override void BeforeUpdate(SettlerCmp settler)
        {
            settler.Pathfinder.SetPath(animal.Pathfinder.CurrentTile, true);
        }

        public override TaskState Update(SettlerCmp settler)
        {
            switch (settler.Pathfinder.MovementState)
            {
                case MovementState.Success:
                    {
                        if (targetTile == animal.Pathfinder.CurrentTile)
                        {
                            if (damagerTimer.GetTime() > 0.5f)
                            {
                                //settler.HandItem.DealDamage(animal);
                                //damagerTimer.Reset();

                                //if (animal.Stats.CurrentHealth == 0)
                                //{
                                //    settler.PlayIdleAnimation();
                                //    State = TaskState.Success;
                                //}
                            }
                            else
                            {
                                settler.PlayIdleAnimation();
                                State = TaskState.Running;
                            }
                        }
                        else
                        {
                            settler.PlayWalkAnimation();

                            damagerTimer.Reset();
                            RebuildPath(settler);
                            State = TaskState.Running;
                        }
                    }
                    break;
                case MovementState.Fail:
                    {
                        State = TaskState.Fail;
                    }
                    break;
                case MovementState.Completion:
                    {
                        settler.PlayWalkAnimation();

                        State = TaskState.Running;
                    }
                    break;
                case MovementState.Running:
                    {
                        if (targetTile != animal.Pathfinder.CurrentTile)
                        {
                            settler.Pathfinder.ResetPath();
                            damagerTimer.Reset();
                        }

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
        }

        private void RebuildPath(SettlerCmp settler)
        {
            BeforeUpdate(settler);
            targetTile = animal.Pathfinder.CurrentTile;
        }

    }
}

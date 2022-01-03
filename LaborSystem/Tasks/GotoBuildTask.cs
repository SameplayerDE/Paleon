using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paleon
{
    public class GotoBuildTask : Task
    {
        private Tile targetTile;

        private bool adjacent;

        private BuildingCmp building;

        private Timer timer;

        public GotoBuildTask(Tile targetTile, bool adjacent, BuildingCmp building)
        {
            this.targetTile = targetTile;

            this.adjacent = adjacent;

            this.building = building;
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

                        if(currentTime > 2)
                        {
                            building.Complete();

                            //settler.Slider.Reset();
                            //settler.Slider.Visible = false;

                            State = TaskState.Success;
                        }
                        else
                        {
                            //settler.Slider.Visible = true;
                            // TODO: building animation
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
        }
    }
}

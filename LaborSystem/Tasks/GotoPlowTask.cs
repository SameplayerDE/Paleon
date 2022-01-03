using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paleon
{
    public class GotoPlowTask : Task
    {
        private FarmArea farmland;

        private Tile targetTile;

        private Timer timer;
        private int processTime = 4;

        public GotoPlowTask(FarmArea farmland)
        {
            this.farmland = farmland;
        }

        public GotoPlowTask(Tile tile)
        {
            targetTile = tile;
        }

        public override bool Check(SettlerCmp settler)
        {
            if (targetTile != null)
            {
                if (settler.Pathfinder.IsPathAvailable(targetTile, false))
                    return true;
            }
            else
            {
                for (int i = 0; i < farmland.Tiles.Count; i++)
                {
                    Tile tile = farmland.Tiles[i];
                    if (settler.Pathfinder.IsPathAvailable(tile, false) && tile.GroundTopType != GroundTopType.FARMLAND)
                    {
                        targetTile = farmland.Tiles[i];
                        return true;
                    }
                }
            }

            return false;
        }

        public override void Begin(SettlerCmp settler)
        {
        }

        public override void BeforeUpdate(SettlerCmp settler)
        {
            settler.Pathfinder.SetPath(targetTile, false);

            timer = new Timer();
        }

        public override TaskState Update(SettlerCmp settler)
        {
            switch (settler.Pathfinder.MovementState)
            {
                case MovementState.Success:
                    {
                        float currentTime = timer.GetTime();

                        //settler.Slider.SetValue(currentTime, processTime);

                        if (currentTime > processTime)
                        {
                            targetTile.GroundTopType = GroundTopType.FARMLAND;

                            //settler.Slider.Reset();
                            //settler.Slider.Visible = false;
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

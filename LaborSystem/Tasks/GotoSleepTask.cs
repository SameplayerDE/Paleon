using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paleon
{
    public class GotoSleepTask : Task
    {
        private Tile targetTile;
        private bool adjacent;


        public GotoSleepTask()
        {

        }

        public override bool Check(SettlerCmp settler)
        {
            targetTile = null;

            if (settler.Hut != null)
            {
                for(int i = 0; i < settler.Hut.OccupiedTiles.Count; i++)
                {
                    if(settler.Hut.OccupiedTiles[i].IsTarget)
                    {
                        targetTile = settler.Hut.OccupiedTiles[i].Tile;
                        adjacent = !settler.Hut.OccupiedTiles[i].IsWalkable;
                        if (settler.Pathfinder.IsPathAvailable(targetTile, adjacent))
                            break;
                    }
                }
            }

            return true;
        }

        public override void Begin(SettlerCmp settler)
        {
            
        }

        public override void BeforeUpdate(SettlerCmp settler)
        {
            if (targetTile != null)
                settler.Pathfinder.SetPath(targetTile, adjacent);

        }

        public override TaskState Update(SettlerCmp settler)
        {
            switch (settler.Pathfinder.MovementState)
            {
                case MovementState.Success:
                    {
                        if(!GameplayScene.WorldManager.WorldTimer.IsNight)
                        {
                            settler.Visible = true;
                            settler.PlayIdleAnimation();
                            State = TaskState.Success;
                        }
                        else
                        {
                            settler.Visible = settler.Hut == null;
                            settler.PlaySleepAnimation();
                            State = TaskState.Running;
                        }
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
            settler.Visible = true;
            settler.Pathfinder.ResetPath();
        }
    }
}

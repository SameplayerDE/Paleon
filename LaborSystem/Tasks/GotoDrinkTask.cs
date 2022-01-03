using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paleon
{
    public class GotoDrinkTask : Task
    {

        private List<WaterSource> waterSources;

        private Tile targetTile;

        private Timer timer;

        public GotoDrinkTask(List<WaterSource> waterSources) 
        {
            this.waterSources = waterSources;

            timer = new Timer();
        }

        public override bool Check(SettlerCmp settler)
        {
            for (int i = 0; i < waterSources.Count; i++)
            {
                targetTile = waterSources[i].GetCoastTile(settler);

                if (targetTile != null)
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
        }


        public override TaskState Update(SettlerCmp settler)
        {
            switch (settler.Pathfinder.MovementState)
            {
                case MovementState.Success:
                    {
                        settler.PlayEatAnimation();
                        settler.Stats.ThirstModificator = 8.0f;

                        if ((int)settler.Stats.CurrentThirst == settler.Stats.MaxThirst)
                        {
                            settler.Stats.ThirstModificator = -0.14f;

                            settler.PlayIdleAnimation();
                            State = TaskState.Success;
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
        }


    }
}

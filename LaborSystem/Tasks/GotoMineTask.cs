using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paleon
{
    public class GotoMineTask : Task
    {
        private Tile targetTile;

        private Timer timer;

        public GotoMineTask(Tile tile)
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

                        settler.Slider.SetValue(0, targetTile.Block.MaxHardness, targetTile.Block.CurrentHardness);

                        if(timer.GetTime() > 1)
                        {
                            targetTile.Block.CurrentHardness -= 1;

                            timer.Reset();

                            if (targetTile.Block.CurrentHardness == 0)
                            {
                                Block block = targetTile.Block;
                                targetTile.RemoveBlock();

                                // Превращаем блок в предметы
                                ItemContainer ic = targetTile.CreateItemContainer(block.Resource);

                                // Создаем работу по переносу ресурсов
                                GameplayScene.WorldManager.LaborManager.Add(new HaulLabor(ic, false));

                                settler.Slider.Active = false;

                                // Оповещаем всем об оконачнии таски
                                cbOnCompleted?.Invoke(this);

                                State = TaskState.Success;
                                return State;
                            }
                        }

                        settler.PlayIdleAnimation();
                        settler.Slider.Active = true;
                        State = TaskState.Running;
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

            settler.Slider.Active = false;

            cbOnCompleted?.Invoke(this);
        }
    }
}

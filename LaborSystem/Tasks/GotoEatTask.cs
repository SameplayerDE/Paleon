namespace Paleon
{
    public class GotoEatTask : Task
    {
        private Timer timer;

        private int processTime = 6;

        private ItemContainer targetItemContainer;

        public GotoEatTask()
        {
        }

        public override bool Check(SettlerCmp settler)
        {
            targetItemContainer = GameplayScene.WorldManager.StorageManager.GetItemContainerWithFood(settler.Pathfinder.CurrentTile.Room.Id);

            return targetItemContainer != null;
        }

        public override void Begin(SettlerCmp settler)
        {
            targetItemContainer.Owner = settler;
        }

        public override void BeforeUpdate(SettlerCmp settler)
        {
            timer = new Timer();

            settler.Pathfinder.SetPath(targetItemContainer.Tile, false);
        }

        public override TaskState Update(SettlerCmp settler)
        {
            switch (settler.Pathfinder.MovementState)
            {
                case MovementState.Success:
                    {
                        if (targetItemContainer != null)
                        {
                            // Добавляем предмет в поселенца и удаляем контейнер
                            settler.Inventory.Cargo = targetItemContainer.Item;
                            targetItemContainer.Item = null;
                            targetItemContainer = null;
                        }

                        float currentTime = timer.GetTime();

                        if (currentTime > processTime)
                        {
                            settler.Inventory.Cargo.Eat(settler);
                            settler.Inventory.Cargo = null;

                            settler.PlayIdleAnimation();

                            // Если поселенец не наелся, то он идет искать другую еду
                            if (settler.Stats.CurrentHunger < 90 && Check(settler))
                            {
                                Begin(settler);
                                BeforeUpdate(settler);
                            }
                            else
                            {
                                State = TaskState.Success;
                            }
                        }
                        else
                        {
                            settler.PlayEatAnimation();
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
            settler.Pathfinder.ResetPath();
        }
    }
}

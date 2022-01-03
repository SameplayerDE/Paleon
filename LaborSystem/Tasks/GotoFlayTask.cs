using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paleon
{
    public class GotoFlayTask : Task
    {
        private AnimalCmp animal;

        private Tile targetTile;

        private Alarm alarm;

        private int alarmLoopCount = 0;

        public GotoFlayTask(AnimalCmp animal)
        {
            this.animal = animal;
        }

        public override bool Check(SettlerCmp settler)
        {
            return settler.Pathfinder.IsPathAvailable(animal.Pathfinder.CurrentTile, false);
        }

        public override void Begin(SettlerCmp settler)
        {
            targetTile = animal.Pathfinder.CurrentTile;
        }

        public override void BeforeUpdate(SettlerCmp settler)
        {
            settler.Pathfinder.SetPath(animal.Pathfinder.CurrentTile, true);

            alarm = Alarm.Create(Alarm.AlarmMode.Looping, OnLoopCompleted, 4, false);
        }

        private void OnLoopCompleted()
        {
            if (alarmLoopCount == 0)
            {
                SpawnItem(ItemDatabase.RAW_SKIN, 1, 3, animal.Pathfinder.CurrentTile);
            }
            else if (alarmLoopCount == 1)
            {
                SpawnItem(ItemDatabase.RAW_MEAT, 1, 3, animal.Pathfinder.CurrentTile);
            }
            else if (alarmLoopCount == 2)
            {
                SpawnItem(ItemDatabase.FAT, 1, 3, animal.Pathfinder.CurrentTile);
            }
            else if (alarmLoopCount == 3)
            {
                SpawnItem(ItemDatabase.BONE, 1, 3, animal.Pathfinder.CurrentTile);
            }

            alarmLoopCount++;
        }

        private void SpawnItem(Item item, int min, int max, Tile parentTile)
        {
            List<Tile> spawnableTiles = new List<Tile>();
            for(int i = 0; i < parentTile.Neighbours.Count; i++)
            {
                if(parentTile.Neighbours[i].IsWalkable)
                {
                    spawnableTiles.Add(parentTile.Neighbours[i]);
                }
            }

            Tile randomTile = spawnableTiles[MyRandom.Range(0, spawnableTiles.Count)];

            int itemsCount = MyRandom.Range(min, max) + 1;
            for (int i = 0; i < itemsCount; i++)
                randomTile.CreateItemContainer(item);   
        }

        public override TaskState Update(SettlerCmp settler)
        {
            switch (settler.Pathfinder.MovementState)
            {
                case MovementState.Success:
                    {
                        if (!alarm.Active)
                        {
                            alarm.Start();

                            //settler.Slider.Visible = true;
                            State = TaskState.Running;
                        }
                        else
                        {
                            alarm.Update();

                            settler.PlayIdleAnimation();
                            //settler.Slider.SetValue(4 - alarm.TimeLeft, 4);

                            if(alarmLoopCount == 4)
                            {
                                animal.Entity.RemoveSelf();

                                //settler.Slider.Reset();
                                //settler.Slider.Visible = false;

                                settler.PlayIdleAnimation();
                                State = TaskState.Success;
                            }
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

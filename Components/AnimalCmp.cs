using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paleon
{
    public class AnimalCmp : Component
    {

        public PathfinderCmp Pathfinder { get; private set; }
        private Sprite sprite;

        private int idleTime = 3;
        private Timer timer;

        public bool IsDead { get; private set; } = false;

        public bool SelectedToHunt = false;

        public Stats Stats { get; private set; }

        public AnimalCmp(PathfinderCmp pathfinder, Sprite sprite) : base(true, false)
        {
            this.Pathfinder = pathfinder;
            this.sprite = sprite;
        }

        public override void Begin()
        {
            timer = new Timer();

            Stats = new Stats(100, 100);
        }

        public override void Update()
        {
            if (!IsDead)
            {
                switch (Pathfinder.MovementState)
                {
                    case MovementState.Success:
                        if (GameplayScene.WorldManager.WorldTimer.IsNight)
                        {
                            sprite.Play("Sleep");
                        }
                        else if (timer.GetTime() >= idleTime)
                        {
                            int x = Pathfinder.CurrentTile.X;
                            int y = Pathfinder.CurrentTile.Y;
                            int randomX = MathHelper.Clamp(MyRandom.Range(x - 10, x + 10), 0, GameplayScene.TILEMAP_SIZE - 1);
                            int randomY = MathHelper.Clamp(MyRandom.Range(y - 10, y + 10), 0, GameplayScene.TILEMAP_SIZE - 1);
                            Tile newTile = GameplayScene.GetTile(randomX, randomY);
                            if (newTile.IsWalkable)
                            {
                                Pathfinder.SetPath(newTile, false);
                                timer.Reset();
                            }

                            sprite.Play("Idle");
                        }
                        else
                        {
                            sprite.Play("Idle");
                        }
                        break;
                    case MovementState.Running:
                        sprite.Play("Walk");
                        break;
                }

                switch (Pathfinder.Direction)
                {
                    case Direction.LEFT:
                        sprite.FlipX = false;
                        break;
                    case Direction.RIGHT:
                        sprite.FlipX = true;
                        break;
                }
            }
        }

        public bool Intersects(int x, int y)
        {
            return sprite.Intersects(x, y);
        }

    }
}

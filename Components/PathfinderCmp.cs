using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paleon
{
    public class PathfinderCmp : Component
    {
        public Tile CurrentTile { get; private set; }
        public MovementState MovementState { get; private set; } = MovementState.Success;
        public Direction Direction { get; private set; } = Direction.LEFT;

        public float MovementSpeed { get; set; }

        private Tile nextTile;
        private float movementPerc;
        private TilePath path;

        private bool rebuildPath;

        private Tile[,] tiles;

        public PathfinderCmp(Tile[,] tiles) : base(true, false)
        {
            this.tiles = tiles;
            MovementSpeed = 1;
        }

        public override void Begin()
        {
            CurrentTile = nextTile = GameplayScene.GetTile((int)(Entity.X / GameplayScene.TILE_SIZE),
              (int)(Entity.Y / GameplayScene.TILE_SIZE));
        }

        public override void Update()
        {
            switch (MovementState)
            {
                case MovementState.Running:
                    {
                        if (nextTile.Equals(CurrentTile))
                        {
                            if (path.Count == 0)
                            {
                                path = null;

                                MovementState = MovementState.Success;
                            }
                            else
                            {
                                if (rebuildPath)
                                {
                                    rebuildPath = false;
                                    if (!RebuildPath(path))
                                        MovementState = MovementState.Fail;
                                }
                                else
                                {
                                    nextTile = path[path.Count - 1];
                                    path.RemoveAt(path.Count - 1);

                                    if (CurrentTile.X > nextTile.X)
                                        Direction = Direction.LEFT;
                                    else if (CurrentTile.X < nextTile.X)
                                        Direction = Direction.RIGHT;

                                    MovementState = MovementState.Running;
                                }
                            }
                        }

                        UpdateMovement();
                    }
                    break;
                case MovementState.Completion:
                    {
                        if (CurrentTile != nextTile)
                            UpdateMovement();
                        else
                            MovementState = MovementState.Success;
                    }
                    break;
            }
        }

        private void UpdateMovement()
        {
            float distToTravel = MathUtils.Distance(CurrentTile.X, CurrentTile.Y, nextTile.X, nextTile.Y);

            float distThisFrame = MovementSpeed * Engine.GameDeltaTime;

            float percThisFrame = distThisFrame / distToTravel;

            movementPerc += percThisFrame;
            if (movementPerc >= 1)
            {
                CurrentTile = nextTile;
                movementPerc = 0;
            }

            Entity.X = MathUtils.Lerp(CurrentTile.X, nextTile.X, movementPerc) * GameplayScene.TILE_SIZE;
            Entity.Y = MathUtils.Lerp(CurrentTile.Y, nextTile.Y, movementPerc) * GameplayScene.TILE_SIZE;
        }

        public void ResetPath()
        {
            path = null;
            MovementState = MovementState.Completion;
        }

        private bool RebuildPath(TilePath path)
        {
            // Если путь к тайлу существует
            if (CurrentTile.Room.Id == path[path.Count - 1].Room.Id)
            {
                // Проверяем нет ли препятствий на пути
                foreach (Tile tile in path.Tiles)
                {
                    // Если есть, то перестраиваем путь к тайлу
                    if (tile.IsWalkable == false)
                    {
                        SetPath(path.TargetTile, path.Adjacent);
                        return true;
                    }
                }

                return true;
            }

            return false;
        }

        public void RebuildPath()
        {
            if (MovementState == MovementState.Running)
                rebuildPath = true;
        }

        public void SetPath(Tile targetTile, bool adjacent)
        {
            if (CurrentTile == targetTile)
            {
                MovementState = MovementState.Success;
                return;
            }

            if (!IsPathAvailable(targetTile, adjacent))
            {
                path = null;
                MovementState = MovementState.Fail;
                return;
            }

            path = PathAStar.CreatePath(tiles, GameplayScene.PathTileGraph, CurrentTile, targetTile, adjacent);
            nextTile = CurrentTile;
            MovementState = MovementState.Running;
        }

        public bool IsPathAvailable(Tile targetTile, bool adjacent)
        {
            if (adjacent)
            {
                foreach (Tile n in targetTile.Neighbours)
                    if (n.IsWalkable)
                        return true;

                return false;
            }

            return CurrentTile.Room.Id == targetTile.Room.Id;
        }

        public void DebugPathRender()
        {
            if (path != null)
            {
                for (int i = 0; i < path.Count; i++)
                {
                    if (i + 1 < path.Count)
                    {
                        Tile first = path[i];
                        Tile next = path[i + 1];

                        RenderManager.Line(new Vector2(first.X * GameplayScene.TILE_SIZE + GameplayScene.TILE_SIZE / 2, first.Y * GameplayScene.TILE_SIZE + GameplayScene.TILE_SIZE / 2),
                            new Vector2(next.X * GameplayScene.TILE_SIZE + GameplayScene.TILE_SIZE / 2, next.Y * GameplayScene.TILE_SIZE + GameplayScene.TILE_SIZE / 2), Color.DarkBlue);
                    }
                }
            }
        }

    }
}

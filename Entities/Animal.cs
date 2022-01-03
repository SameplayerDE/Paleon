using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paleon
{
    public class Animal : Entity
    {

        public Animal(Scene scene, Tile[,] tiles) : base(scene)
        {
            Sprite sprite = new Sprite(24, 24);
            MyTexture texture = ResourceManager.GetTexture("zebu");
            sprite.Add("Walk", new Animation(texture, 1, 0, 24, 24, 0, 0, 1));
            sprite.Add("Idle", new Animation(texture, 2, 0, 24, 24, 0, 0, 2));
            sprite.Add("Sleep", new Animation(texture, 2, 0, 24, 24, 0, 24, 2));
            sprite.Add("Dead", new Animation(texture, 1, 0, 24, 24, 0, 48, 1));
            sprite.X = -4;
            sprite.Y = GameplayScene.TILE_SIZE - sprite.Height;
            Add(sprite);
            PathfinderCmp pathfinder = new PathfinderCmp(tiles);
            Add(pathfinder);
            Add(new AnimalCmp(pathfinder, sprite));
            Add(new SelectableCmp((int)sprite.X, (int)sprite.Y + 4, 24, 20));
        }

    }
}

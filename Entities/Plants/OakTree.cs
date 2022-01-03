using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paleon
{
    public class OakTree : Entity
    {
        public OakTree(Tile tile, Scene scene) : base(scene)
        {
            List<Stage> stages = new List<Stage>();
            stages.Add(new Stage(0, 20, Tuple.Create(ItemDatabase.STICK, 1)));
            stages.Add(new Stage(1, 40, Tuple.Create(ItemDatabase.STICK, 2)));
            stages.Add(new Stage(2, 60, Tuple.Create(ItemDatabase.STICK, 3), Tuple.Create(ItemDatabase.LOG, 1)));
            stages.Add(new Stage(3, -1, Tuple.Create(ItemDatabase.STICK, 3), Tuple.Create(ItemDatabase.LOG, 3)));

            SaplingCmp plant = new SaplingCmp(
                "Oak", 
                tile, 
                24, 30, 
                ResourceManager.GetTexture("oak_tree"), 
                Color.White,
               ItemDatabase.ACORN,
               false,
               true, 
               false,
               stages);

            Add(plant);
            Add(new SelectableCmp((GameplayScene.TILE_SIZE - 24) / 2, GameplayScene.TILE_SIZE - 30, 24, 30));
        }
    }
}

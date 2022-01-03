using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paleon
{
    public class Oat : Entity
    {

        public Oat(Tile tile, Scene scene) : base(scene)
        {
            List<Stage> stages = new List<Stage>();
            stages.Add(new Stage(0, 10));
            stages.Add(new Stage(1, 20));
            stages.Add(new Stage(2, 30));

            stages.Add(new Stage(3, 40));
            stages.Add(new Stage(4, 50));
            stages.Add(new Stage(5, 60));

            stages.Add(new Stage(6, 70));
            stages.Add(new Stage(7, 100));
            stages.Add(new Stage(8, -1));


            SaplingCmp crop = new SaplingCmp(
                "Oat", 
                tile, 
                16, 22, 
                ResourceManager.GetTexture("oat"), 
                Color.White,
                ItemDatabase.OAT_SEEDS,
                false,
                false,
                true,
                stages);

            Add(crop);

            Add(new SelectableCmp(GameplayScene.TILE_SIZE - 16, GameplayScene.TILE_SIZE - 22, 16, 22));
        }

    }
}

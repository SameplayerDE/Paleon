using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paleon
{
    public class Cattail : Entity
    {

        public Cattail(Tile tile, Scene scene) : base(scene)
        {
            List<Stage> stages = new List<Stage>();
            stages.Add(new Stage(0, 20));
            stages.Add(new Stage(1, 40));
            stages.Add(new Stage(2, 50));
            stages.Add(new Stage(3, 60));
            stages.Add(new Stage(4, -1));

            SaplingCmp crop = new SaplingCmp(
                "Cattail",
                tile,
                16, 18,
                ResourceManager.GetTexture("cattail"),
                Color.White,
                ItemDatabase.STICK,
                false,
                false,
                true,
                stages);

            Add(crop);

            Add(new SelectableCmp(GameplayScene.TILE_SIZE - 16, GameplayScene.TILE_SIZE - 18, 16, 18));
        }
    }
}

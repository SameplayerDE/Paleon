using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paleon
{
    public class RaspberryBush : Entity
    {
        public RaspberryBush(Tile tile, Scene scene) : base(scene)
        {
            List<Stage> stages = new List<Stage>();
            stages.Add(new Stage(0, 20));
            stages.Add(new Stage(1, 40));
            stages.Add(new Stage(2, 60));
            stages.Add(new Stage(3, -1));

            SaplingCmp plant = new SaplingCmp(
                "Raspberry",
                tile,
                16, 18,
                ResourceManager.GetTexture("berry_bush"),
                Color.White,
                ItemDatabase.RASPBERRY,
                false,
                true,
                true,
               stages);

            Add(plant);
            Add(new SelectableCmp((GameplayScene.TILE_SIZE - 16) / 2, GameplayScene.TILE_SIZE - 18, 16, 18));
        }
    }
}

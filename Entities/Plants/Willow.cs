using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paleon
{
    public class Willow : Entity
    {

        public Willow(Scene scene, Tile tile) : base(scene)
        {
            List<Stage> stages = new List<Stage>();
            stages.Add(new Stage(0, 20));
            stages.Add(new Stage(1, 40));
            stages.Add(new Stage(2, 60));
            stages.Add(new Stage(3, -1));

            SaplingCmp plant = new SaplingCmp(
                "Willow", 
                tile, 
                16, 18, 
                ResourceManager.GetTexture("willow"), 
                Color.White,
                ItemDatabase.STICK,
                false,
                false, 
                true, 
                stages);

            Add(plant);
            Add(new SelectableCmp((GameplayScene.TILE_SIZE - 16) / 2, GameplayScene.TILE_SIZE - 18, 16, 18));
        } 

    }
}

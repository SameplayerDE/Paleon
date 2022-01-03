using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paleon
{
    public class CurrantBush : Entity
    {
        public CurrantBush(Tile tile, Scene scene) : base(scene)
        {
            //FruitSaplingCmp sapling = new FruitSaplingCmp(tile, 16, 18, ResourceManager.GetTexture("currant_bush"), Color.White, 4,
            //    false, ItemDatabase.CURRANT, ResourceManager.GetTexture("currant_bush").GetSubtexture(64, 0, 16, 18), Season.MiddleSummer,
            //    Season.LateSummer, 3, true);
            //sapling.AddStage(new Stage(3));
            //sapling.AddStage(new Stage(3));
            //sapling.AddStage(new Stage(3));
            //sapling.AddStage(new Stage(-1));
            //Add(sapling);

            //Add(new SelectableCmp(GameplayScene.TILE_SIZE - 16, GameplayScene.TILE_SIZE - 18, 16, 18));
        }
    }
}

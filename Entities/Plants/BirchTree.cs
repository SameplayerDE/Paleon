using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paleon
{
    public class BirchTree : Entity
    {

        public BirchTree(Tile tile, Scene scene) : base(scene)
        {
            //Add(new VegetationCmp(VegetationType.TREE, tile, 24, 30, 1, 0, ResourceManager.GetTexture("birch"), null, null, false, false, 720));
        }

    }
}

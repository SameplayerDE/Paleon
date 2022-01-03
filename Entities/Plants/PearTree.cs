using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paleon
{
    public class PearTree : Entity
    {
        public PearTree(Tile tile, Scene scene) : base(scene)
        {
            //Add(new VegetationCmp(VegetationType.FRUIT_TREE, tile, 24, 30, 2, 1, ResourceManager.GetTexture("pear_tree"), ItemDatabase.PEAR, null, false, false, 720));
        }
    }
}

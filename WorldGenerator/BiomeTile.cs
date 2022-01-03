using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paleon
{
    public class BiomeTile
    {

        public int X, Y;
        public MyTexture Texture;
        public bool HasRiver = false;
        public MyTexture RiverTexture;
        public HeightType HeightType;

        public BiomeTile(int x, int y)
        {
            X = x;
            Y = y;
        }

    }
}

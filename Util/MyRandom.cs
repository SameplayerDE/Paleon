using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paleon
{
    public static class MyRandom
    {

        private static Random random = new Random();

        public static int Range(int min, int max)
        {
            return random.Next(min, max);
        }

        public static int Range(int maxValue)
        {
            return random.Next(maxValue);
        }

        public static int FromSet(params int[] values)
        {
            int num = Range(values.Length);
            return values[num];
        }

    }
}

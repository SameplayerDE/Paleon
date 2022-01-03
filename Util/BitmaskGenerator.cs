using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paleon
{
    public static class BitmaskGenerator
    {

        private static int[] bitmasks = new int[256];


        static BitmaskGenerator()
        {
            Generate(1, 2,                                   new int[] { 1, 4, 32, 128 });
            Generate(2, 8,                                   new int[] { 1, 4, 32, 128 });
            Generate(3, 2 + 8,                               new int[] { 4, 32, 128 });
            Generate(4, 1 + 2 + 8,                           new int[] { 4, 32, 128 });
            Generate(5, 16,                                  new int[] { 1, 4, 32, 128 });
            Generate(6, 2 + 16,                              new int[] { 1, 32, 128 });
            Generate(7, 2 + 4 + 16,                          new int[] { 1, 32, 128 });
            Generate(8, 8 + 16,                              new int[] { 1, 4, 32, 128 });
            Generate(9, 2 + 8 + 16,                          new int[] { 32, 128 });
            Generate(10, 1 + 2 + 8 + 16,                     new int[] { 32, 128 });
            Generate(11, 2 + 4 + 8 + 16,                     new int[] { 32, 128 });
            Generate(12, 1 + 2 + 4 + 8 + 16,                 new int[] { 32, 128 });
            Generate(13, 64,                                 new int[] { 1, 4, 32, 128 });
            Generate(14, 2 + 64,                             new int[] { 1, 4, 32, 128 });
            Generate(15, 8 + 64,                             new int[] { 1, 4, 128 });
            Generate(16, 2 + 8 + 64,                         new int[] { 4, 128 });
            Generate(17, 1 + 2 + 8 + 64,                     new int[] { 4, 128 });
            Generate(18, 16 + 64,                            new int[] { 1, 4, 32 });
            Generate(19, 2 + 16 + 64,                        new int[] { 1, 32 });
            Generate(20, 2 + 4 + 16 + 64,                    new int[] { 1, 32 });
            Generate(21, 8 + 16 + 64,                        new int[] { 1, 4 });
            Generate(22, 2 + 8 + 16 + 64);
            Generate(23, 1 + 2 + 8 + 16 + 64);
            Generate(24, 2 + 4 + 16 + 8 + 64);
            Generate(25, 1 + 2 + 4 + 8 + 16 + 64);
            Generate(26, 8 + 32 + 64,                        new int[] { 1, 4, 128 });
            Generate(27, 2 + 8 + 32 + 64,                    new int[] { 4, 128 });
            Generate(28, 1 + 2 + 8 + 32 + 64,                new int[] { 4, 128 });
            Generate(29, 8 + 32 + 64 + 16,                   new int[] { 1, 4 });
            Generate(30, 2 + 8 + 16 + 32 + 64);
            Generate(31, 1 + 2 + 8 + 16 + 32 + 64);
            Generate(32, 2  + 4 + 8 + 16 + 32 + 64);
            Generate(33, 1 + 2 + 4 + 8 + 16 + 32 + 64);
            Generate(34, 16 + 64 + 128,                      new int[] { 1, 4, 32 });
            Generate(35, 2 + 16 + 64 + 128,                  new int[] { 1, 32 });
            Generate(36, 2 + 4 + 16 + 64 + 128,              new int[] { 1, 32 });
            Generate(37, 8 + 16 + 64 + 128,                  new int[] { 1, 4 });
            Generate(38, 2 + 8 + 16 + 64 + 128);
            Generate(39, 1 + 2 + 8 + 16 + 64 + 128);
            Generate(40, 8 + 2 + 4 + 16 + 64 + 128);
            Generate(41, 1 + 2 + 4 + 8 + 16 + 64 + 128);
            Generate(42, 8 + 16 + 32 + 64 + 128,             new int[] { 1, 4 });
            Generate(43, 2 + 8 + 16 + 32 + 64 + 128);
            Generate(44, 1 + 2 + 8 + 16 + 32 + 64 + 128);
            Generate(45, 2 + 4 + 8 + 16 + 32 + 64 + 128);
            Generate(46, 1 + 2 + 4 + 8 + 16 + 32 + 64 + 128);
            Generate(47, 0,                                  new int[] { 1, 4, 32, 128 });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tileId">Id тайла который будет привязан к сгенерированным битмаскам</param>
        /// <param name="offset">Число которое будет прибавляться к результатам комбинаций</param>
        /// <param name="arr">Числа для комбинирования</param>
        private static void Generate(int tileId, int offset, int[] arr)
        {
            if (arr.Length == 0)
            {
                bitmasks[offset] = tileId;
                return;
            }
            else if(arr.Length == 1)
            {
                bitmasks[offset + arr[0]] = tileId;
                return;
            }

            // Высчитываем количество комбинаций 2 ^ arr.Length
            int count = (int)Math.Pow(2, arr.Length);
            for (int i = 0; i < count; i++)
            {
                // Переводим число в бинарное
                string binary = Convert.ToString(i, 2).PadLeft(arr.Length, '0');

                int resultsNumber = 0;

                for (int c = 0; c < binary.Length; c++)
                {
                    // Если в двоичном числе встречается 1, то прибавляем к результату комбинации
                    if (binary[c] == '1')
                    {
                        resultsNumber += arr[c];
                    }
                }

                bitmasks[resultsNumber + offset] = tileId;
            }
        }

        private static void Generate(int tileId, int offset)
        {
            bitmasks[offset] = tileId;
        }

        // Возвращает Id тайла для данного bitmask
        public static int GetTileNumber(int bitmask)
        {
            return bitmasks[bitmask];
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paleon
{
    public class Timer
    {

        private float time;

        public Timer()
        {

        }

        public float GetTime()
        {
            time += Engine.GameDeltaTime;
            return time;
        }

        public void Reset()
        {
            time = 0;
        }

    }
}

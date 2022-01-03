using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paleon
{
    public class SleepLabor : Labor
    {

        public SleepLabor() : base(LaborType.NONE)
        {
            tasks.Add(new GotoSleepTask());
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paleon
{
    public class EatLabor : Labor
    {

        public EatLabor() : base(LaborType.NONE)
        {
            tasks.Add(new GotoEatTask());
        }

    }
}

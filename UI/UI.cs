using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paleon
{
    public abstract class UI
    {

        public abstract void Update();

        public abstract void Render();

        public abstract void Open(object obj);

        public abstract void Close();

    }
}

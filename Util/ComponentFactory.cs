using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paleon
{
    public static class ComponentFactory
    {

        public static T Create<T>() where T : Component, new()
        {
            return new T();
        }

    }
}

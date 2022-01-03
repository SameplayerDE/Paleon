using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paleon
{
    public class StorageHutCmp : StorageBuildingCmp
    {

        public StorageHutCmp() : base("storage_hut", 50)
        {
            AddBuildingItem(ItemDatabase.STICK, 6);
        }

    }
}

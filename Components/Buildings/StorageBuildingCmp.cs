using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paleon
{
    public class StorageBuildingCmp : BuildingCmp
    {

        public int MaxWeight { get; private set; }

        public int OccupiedCount
        {
            get { return CurrentCount + CountToAdd; }
        }

        public int FreeCount
        {
            get { return MaxWeight - OccupiedCount; }
        }

        public int CurrentCount { get; private set; }
        public int CountToAdd;
        public int CountToRemove;

        public Dictionary<Item, int> Slots { get; private set; }

        public StorageBuildingCmp(string json, int maxSpace) : base(json)
        {
            MaxWeight = maxSpace;

            Slots = new Dictionary<Item, int>();
        }

        public void AddItem(Item item, int count)
        {
            if (!Slots.ContainsKey(item))
                Slots.Add(item, count);
            else
                Slots[item] += count;

            CurrentCount += count;

            if (CountToAdd > 0)
                CountToAdd -= count;
        }
    }
}

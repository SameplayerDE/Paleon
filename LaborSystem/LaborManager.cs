using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paleon
{
    public class LaborManager
    {

        private Dictionary<LaborType, List<Labor>> laborsByType;

        private List<Labor> labors;
        private List<Labor> laborsToRemove;

        public LaborManager()
        {
            laborsByType = new Dictionary<LaborType, List<Labor>>();
            foreach (var laborType in Enum.GetValues(typeof(LaborType)))
            {
                laborsByType.Add((LaborType)laborType, new List<Labor>());
            }

            labors = new List<Labor>();
            laborsToRemove = new List<Labor>();
        }

        public void Update(List<SettlerCmp> settlers)
        {
            UpdateSettlersLabors(settlers);
            CheckCompletedLabors();
        }

        private void UpdateSettlersLabors(List<SettlerCmp> settlers)
        {
            for (int i = 0; i < settlers.Count; i++)
            {
                SettlerCmp settler = settlers[i];

                if (settler.Labor == null)
                {
                    Labor labor = settler.LookForLabor(laborsByType);
                    if (labor != null)
                    {
                        settler.Labor = labor;
                        settler.Labor.Begin(settler);
                    }
                }
                else
                {
                    settler.Labor.Update();
                }
            }
        }

        private void CheckCompletedLabors()
        {
            // Looking for completed labors
            for (int i = 0; i < labors.Count; i++)
            {
                Labor labor = labors[i];
                if (labor.IsCompleted)
                    laborsToRemove.Add(labor);
            }

            // Remove all completed labors
            if (laborsToRemove.Count > 0)
            {
                for (int i = 0; i < laborsToRemove.Count; i++)
                {
                    Labor labor = laborsToRemove[i];
                    labors.Remove(labor);
                    laborsByType[labor.LaborType].Remove(labor);
                }

                laborsToRemove.Clear();
            }
        }

        public void Add(Labor labor)
        {
            if (labors.Contains(labor))
                throw new Exception("LaborManager has this labor!");

            laborsByType[labor.LaborType].Add(labor);

            labors.Add(labor);
        }

    }
}

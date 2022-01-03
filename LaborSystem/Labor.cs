using System;
using System.Collections.Generic;

namespace Paleon
{
    public enum LaborType
    {
        Miner,
        Woodcutter,
        Porter,
        Builder,
        Crafter,
        Cook,
        Farmer,
        Hunter,
        Fisher,
        NONE
    }

    public abstract class Labor
    {
        public LaborType LaborType { get; private set; }

        public bool IsCompleted { get; set; }

        public bool Repeat { get; set; }

        public SettlerCmp Owner { get; protected set; }

        protected List<Task> tasks { get; private set; }
        protected Task currentTask { get; private set; }

        private int taskIndex;

        private Action<Labor> cbOnLaborCompleted;

        public Labor(LaborType laborType)
        {
            LaborType = laborType;
            tasks = new List<Task>();
        }

        public virtual bool Check(SettlerCmp settler)
        {
            for (int i = 0; i < tasks.Count; i++)
            {
                if (!tasks[i].Check(settler))
                    return false;
            }

            return true;
        }

        public virtual void Begin(SettlerCmp settler)
        {
            Owner = settler;

            for (int i = 0; i < tasks.Count; i++)
            {
                tasks[i].Begin(settler);
            }

            currentTask = tasks[0];
            currentTask.BeforeUpdate(Owner);
        }

        public virtual void Update()
        {
            switch (currentTask.State)
            {
                case TaskState.Running:
                    currentTask.Update(Owner);
                    break;
                case TaskState.Success:
                    if (taskIndex < tasks.Count - 1)
                    {
                        taskIndex += 1;
                        currentTask = tasks[taskIndex];
                        currentTask.BeforeUpdate(Owner);
                    }
                    else
                    {
                        Complete();
                    }
                    break;
                case TaskState.Fail:
                    Cancel();
                    break;
            }
        }

        public virtual void Cancel()
        {
            if (Owner != null)
            {
                for (int i = 0; i < tasks.Count; i++)
                {
                    if(tasks[i].State != TaskState.Success)
                        tasks[i].Cancel(Owner);
                }

                Owner.PlayIdleAnimation();

                Complete();
            }

            cbOnLaborCompleted?.Invoke(this);

            if (Repeat)
                IsCompleted = false;
            else
                IsCompleted = true;
        }

        protected virtual void Complete()
        {
            Owner.Labor = null;
            Owner = null;
            taskIndex = 0;

            if (Repeat)
                for (int i = 0; i < tasks.Count; i++)
                    tasks[i].State = TaskState.Running;

            if (Repeat)
                IsCompleted = false;
            else
                IsCompleted = true;

            cbOnLaborCompleted?.Invoke(this);
        }

        public void AddOnLaborCompletedCallback(Action<Labor> callback)
        {
            cbOnLaborCompleted += callback;
        }

        public void RemoveOnLaborCompletedCallback(Action<Labor> callback)
        {
            cbOnLaborCompleted -= callback;
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paleon
{
    public enum TaskState
    {
        Success,
        Running,
        Fail
    }

    public abstract class Task
    {

        protected Action<Task> cbOnCompleted;

        public TaskState State { get; set; } = TaskState.Running;

        public abstract bool Check(SettlerCmp settler);

        public abstract void Begin(SettlerCmp settler);

        public abstract void BeforeUpdate(SettlerCmp settler);

        public abstract TaskState Update(SettlerCmp settler);

        public abstract void Cancel(SettlerCmp settler);

        public void AddOnCompletedCallback(Action<Task> callback)
        {
            cbOnCompleted += callback;
        }

        public void RemoveOnCompletedCallback(Action<Task> callback)
        {
            cbOnCompleted -= callback;
        }

    }
}

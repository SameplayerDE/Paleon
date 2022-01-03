using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paleon
{
    public class Alarm : Component
    {
        public enum AlarmMode { Oneshot, Looping };

        public Action OnComplete;

        public AlarmMode Mode { get; private set; }
        public float Duration { get; private set; }
        public float TimeLeft { get; private set; }

        #region Static

        private static Stack<Alarm> cached = new Stack<Alarm>();

        public static Alarm Create(AlarmMode mode, Action onComplete, float duration = 1f, bool start = false)
        {
            Alarm alarm;
            if (cached.Count == 0)
                alarm = new Alarm();
            else
                alarm = cached.Pop();

            alarm.Init(mode, onComplete, duration, start);
            return alarm;
        }

        #endregion

        private Alarm() : base(false, false)
        {

        }

        private void Init(AlarmMode mode, Action onComplete, float duration = 1f, bool start = false)
        {
#if DEBUG
            if (duration <= 0)
                throw new Exception("Alarm duration cannot be less than zero");
#endif
            Mode = mode;
            Duration = duration;
            OnComplete = onComplete;

            Active = false;
            TimeLeft = 0;

            if (start)
                Start();
        }

        public override void Update()
        {
            TimeLeft -= Engine.GameDeltaTime;
            if (TimeLeft <= 0)
            {
                TimeLeft = 0;
                OnComplete?.Invoke();

                if (Mode == AlarmMode.Looping)
                    Start();
                else if (Mode == AlarmMode.Oneshot)
                    Stop();
                    
            }
        }

        public void Start()
        {
            Active = true;
            TimeLeft = Duration;
        }

        public void Start(float duration)
        {
#if DEBUG
            if (duration <= 0)
                throw new Exception("Alarm duration cannot be <= 0");
#endif

            Duration = duration;
            Start();
        }

        public void Stop()
        {
            Active = false;
        }
    }
}

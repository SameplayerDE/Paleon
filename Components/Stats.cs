using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paleon
{
    public class Stats
    {

        public int MaxHunger { get; private set; }
        public int MaxThirst { get; private set; }

        public float HungerModificator { get; set; }
        public float ThirstModificator { get; set; }
        public float TemperatureModificator { get; set; }

        private float currentHunger;
        public float CurrentHunger 
        { 
            get { return currentHunger; }
            set
            {
                float newHunger = MathHelper.Clamp(value, 0, MaxHunger);
                if (newHunger != currentHunger)
                {
                    currentHunger = newHunger;
                }
            }
        }

        private float currentThirst;
        public float CurrentThirst
        {
            get { return currentThirst; }
            set
            {
                float newThirst = MathHelper.Clamp(value, 0, MaxThirst);
                if (newThirst != currentThirst)
                {
                    currentThirst = newThirst;
                    if (currentThirst == 0)
                    {
                    }
                }
            }
        }

        public int MaxTemperature { get; private set; }
        public int MinTemperature { get; private set; }
        public float CurrentTemperature { get; private set; }

        private Timer timer;

        public Stats(int maxSatiety, int maxThirst)
        {
            MaxHunger = maxSatiety;
            CurrentHunger = MaxHunger;

            MaxThirst = maxThirst;
            CurrentThirst = MaxThirst;

            MaxTemperature = 42;
            MinTemperature = 25;
            CurrentTemperature = 36f;

            timer = new Timer();
        }

        public void Update()
        {
            if(timer.GetTime() >= 0.5f)
            {
                timer.Reset();

                CurrentHunger += HungerModificator;

                CurrentThirst += ThirstModificator;

                CurrentTemperature = MathHelper.Clamp(
                    CurrentTemperature + TemperatureModificator,
                    MinTemperature, MaxTemperature);
            }
        }

    }
}

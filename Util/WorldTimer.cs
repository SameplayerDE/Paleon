using Microsoft.Xna.Framework;
using System;

namespace Paleon
{
    public enum Season
    {
        Early_Winter,
        Mid_Winter,
        Late_Winter,
        Early_Spring,
        Mid_Spring,
        Late_Spring,
        Early_Summer,
        Mid_Summer,
        Late_Summer,
        Early_Autumn,
        Mid_Autumn,
        Late_Autumn,
        None
    }

    public class WorldTimer
    {
        private Season currentSeason;
        public Season CurrentSeason 
        { 
            get { return currentSeason; } 
            private set
            {
                currentSeason = value;

                cbOnSeasonChanged?.Invoke(value);
            }
        }

        public int CurrentDay { get; private set; }

        public float CurrentTimeInDegrees { get; private set; }

        public int CurrentTimeInHours { get; private set; }

        public bool IsNight { get; private set; }

        private int divider = 15;

        private Action<Season> cbOnSeasonChanged;


        public WorldTimer()
        {
            
        }

        public void Begin()
        {
            CurrentDay = 9;
            CurrentSeason = Season.Mid_Summer;
        }

        public void Update()
        {
            CurrentTimeInDegrees += Engine.GameDeltaTime;

            if (CurrentTimeInDegrees > 359)
            {
                CurrentTimeInDegrees = 0;
                NextDay();
            }

            CurrentTimeInHours = (int)Math.Floor(CurrentTimeInDegrees / divider);

            IsNight = CurrentTimeInHours >= 0 && CurrentTimeInHours < 6;
        }

        public void SetTime(int hour)
        {
            CurrentTimeInDegrees = MathHelper.Clamp(hour * divider, 0, 359);
            CurrentTimeInHours = (int)Math.Floor(CurrentTimeInDegrees / divider);
        }

        private void NextDay()
        {
            CurrentDay += 1;

            if (CurrentDay == 8)
                NextSeason();
            else if (CurrentDay == 15)
                NextSeason();
            else if(CurrentDay == 22)
            {
                CurrentDay = 1;
                NextSeason();
            }
        }

        private void NextSeason()
        {
            if (CurrentSeason == Season.Early_Winter)
                CurrentSeason = Season.Mid_Winter;
            else if (CurrentSeason == Season.Mid_Winter)
                CurrentSeason = Season.Late_Winter;
            else if (CurrentSeason == Season.Late_Winter)
                CurrentSeason = Season.Early_Spring;
            else if (CurrentSeason == Season.Early_Spring)
                CurrentSeason = Season.Mid_Spring;
            else if (CurrentSeason == Season.Mid_Spring)
                CurrentSeason = Season.Late_Spring;
            else if (CurrentSeason == Season.Late_Spring)
                CurrentSeason = Season.Early_Summer;
            else if (CurrentSeason == Season.Early_Summer)
                CurrentSeason = Season.Mid_Summer;
            else if (CurrentSeason == Season.Mid_Summer)
                CurrentSeason = Season.Late_Summer;
            else if (CurrentSeason == Season.Late_Summer)
                CurrentSeason = Season.Early_Autumn;
            else if (CurrentSeason == Season.Early_Autumn)
                CurrentSeason = Season.Mid_Autumn;
            else if (CurrentSeason == Season.Mid_Autumn)
                CurrentSeason = Season.Late_Autumn;
            else if (CurrentSeason == Season.Late_Autumn)
                CurrentSeason = Season.Early_Winter;
        }

        public void RegisterOnSeasonChangedCallback(Action<Season> callback)
        {
            cbOnSeasonChanged += callback;
        }

        public void UnregisterOnSeasonChangedCallback(Action<Season> callback)
        {
            cbOnSeasonChanged -= callback;
        }

    }
}

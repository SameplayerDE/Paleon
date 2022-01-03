using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paleon
{
    public class DayColorChanger
    {

        public static Color DAY_COLOR { get; private set; } = new Color(255, 255, 255, 255);
        public static Color NIGHT_COLOR { get; private set; } = new Color(63, 137, 255, 255);

        public static Color GetTimeOfDayColor(float timeInDegrees)
        {
            if (timeInDegrees >= 90 && timeInDegrees <= 95)
            {
                float amount = Math.Min(1.0f, (timeInDegrees - 90) / 5);
                return Color.Lerp(NIGHT_COLOR, DAY_COLOR, amount);
            }
            else if (timeInDegrees >= 95 && timeInDegrees <= 359)
            {
                return DAY_COLOR;
            }
            else if (timeInDegrees >= 0 && timeInDegrees <= 5)
            {
                float amount = Math.Min(1.0f, timeInDegrees / 5);
                return Color.Lerp(DAY_COLOR, NIGHT_COLOR, amount);
            }
            else if(timeInDegrees >= 5 && timeInDegrees <= 90)
            {
                return NIGHT_COLOR;
            }

            return Color.Black;
        }

    }
}

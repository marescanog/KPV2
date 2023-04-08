using System;
using System.Collections;
using System.Collections.Generic;

namespace GameConstants
{
    namespace TimeSystem
    {
        public static class TimeDefinitions
        {
            public const int ARROW_Z_ROT_START = -6;
            public const int ARROW_Z_ROT_END = -177;

            public const float ARROW_Z_ARC_LENGTH = 171f;

            public const int TOTAL_MINUTES_IN_DAY = 600; // 10 hours in a game day, 60 minutes an hour

            public const float TICK_TIMER_MAX = 0.1f;      // 10 ticks in 1 second
            public const float MINUTE_MAX_TICKS = 75f;    // 0.75 seconds real life is 1 minute in-game
            public const int DAY_MAX_MIN = 59;             // starts at 0 for total 60
            public const int DAY_MAX_HOURS = 25;           // in-game day stops at the 25th hour on a 24 hour clock and timed-event plays
            public const int DEFAULT_START_HOUR = 5;
            public static readonly int[] HOUR_TO_MIN_CONVERSION = { 0, 60, 120, 180, 240, 300, 360, 420, 480, 540, 600, 660, 720, 780, 840, 900, 960, 1020, 1080, 1140, 1200, 1260, 1320, 1380, 1440, 1500, 1560 };
            public static int MilitaryTo12H(int hours)
            {
                return (hours <= 12) ? hours : ((hours > 24) ? 1: hours - 12);
            }


            public static float NormalizeArrowRotation(int hours, int minutes)
            {
                int totalTimeInMinutes = HOUR_TO_MIN_CONVERSION[hours] + minutes;
                return 0-(((totalTimeInMinutes-300)*0.1425f)+6);
            }

        }
    }

    public static class Temp
    {
        public const int MAX = 6000;
        public const int START_COOK = 1600;

        public const int HIGH = 4250;
        public const int MEDIUM = 3500;
        public const int MEDLOW = 3000;
        public const int LOW = 1800;

        public const int HIGH_MAX = 4500;
        public const int MEDIUM_MAX = 3750;
        public const int MEDLOW_MAX = 3250;
        public const int LOW_MAX = 2000;

        public const int SEAR = 4000;
        public const int BOIL = 2100;    // Liquids do evaporate
        public const int SIMMER = 1800;  // Liquids do not evaporate

        public const int FOOD_HOT_HOLDING = 1350; // above no bacteria grows
        public const int FOOD_ROOM_TEMP = 700;    // starting food temperature
        public const int FOOD_COLD_HOLDING = 400; // below no bacteria grows
        public const int FOOD_FROZEN = -100;

        public static int CookTime(int temp)
        {
            if(temp >= HIGH_MAX)
            {
                return 30;
            } else if (temp >= HIGH)
            {
                return 20;
            }
            else if (temp >= MEDIUM)
            {
                return 10;
            }
            else if (temp >= LOW_MAX)
            {
                return 7;
            }
            else if (temp >= LOW)
            {
                return 5;
            } else
            {
                return 0;
            }
        }
    }
}


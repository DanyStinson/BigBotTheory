using System;
using static Microsoft.Bot.Builder.Luis.BuiltIn.DateTime;

namespace BotExample.Extensions
{
    public static class StringExtensions
    {
        public static DayOfWeek GetDayOfWeek(this string datetime)
        {
            DayOfWeek? day;
            DateTimeResolution resolution;
            if (DateTimeResolution.TryParse(datetime, out resolution))
            {
                day = resolution.DayOfWeek;
                DayOfWeek dayOfWeek;
                if (day != null)
                {
                    dayOfWeek = day.Value;
                }
                else
                {
                    if (resolution.Year.HasValue && resolution.Month.HasValue && resolution.Day.HasValue)
                    {
                        dayOfWeek = new DateTime(resolution.Year.Value, resolution.Month.Value, resolution.Day.Value).DayOfWeek;
                    }
                    else
                    {
                        dayOfWeek = DateTime.Now.DayOfWeek;
                    }
                }
                return dayOfWeek;
            }
            else
            {
                return DateTime.Now.DayOfWeek;
            }
        }
    }
}
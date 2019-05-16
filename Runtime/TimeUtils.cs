using System;

namespace Toolbox.Runtime
{
    public static class TimeUtils
    {
        private static readonly DateTime HistoricOffset = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        
        public static string PrettyTime(double seconds)
        {
            var time = System.TimeSpan.FromSeconds(seconds);
            var result = "";
            if (time.Days > 0) result += $"{time.Days.ToString()}d";
            if (time.Hours > 0) result += $"{time.Hours.ToString()}h";
            if (time.Minutes > 0) result += $"{time.Minutes.ToString()}m";
            if (time.Seconds > 0) result += $"{time.Seconds.ToString()}s";
			
            return result != "" ? result : "0s";
        }
        
        public static string PrettyTimeWithMilliseconds(float seconds)
        {
            var time = System.TimeSpan.FromSeconds(seconds);
            var result = "";
            if (time.Days > 0) result += $"{time.Days.ToString()}d";
            if (time.Hours > 0) result += $"{time.Hours.ToString()}h";
            if (time.Minutes > 0) result += $"{time.Minutes.ToString()}m";
			
            if (time.Milliseconds > 0) result += $"{time.Seconds.ToString()}.{(time.Milliseconds / 100).ToString()}s";
            else if (time.Seconds > 0) result += $"{time.Seconds.ToString()}s";
			
            return result != "" ? result : "0s";
        }
        
        public static double ToEpoch(this DateTime dateTime)
        {
            return (dateTime - HistoricOffset).TotalSeconds;
        }
    }
}

using System;

namespace DeepFreeze.Packages.Toolbox.Runtime
{
    public static class TimeUtils
    {
        private static readonly DateTime HistoricOffset = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

		public static string PrettyTime(double timeInSeconds, string delimiter = "", bool decimals = false, bool clipIfLong = false)
		{
			var time = TimeSpan.FromSeconds(timeInSeconds);

            var format = "";

			if (time.Days > 0)
			{
                //4d
                format = "d\\d";
                
                //4d23:h12m
                if(time.Hours > 0 || time.Minutes > 0 )
                {
                    format += $"%h\\h{delimiter}m\\m";
                }
			}
			else if (time.Hours > 0)
			{
                //12h
                format = "%h\\h";
                
                //12h:20m:23s
                if(time.Minutes > 0 || time.Seconds > 0)
                {
					if(clipIfLong)
					{
						if(time.Hours < 10 && time.Days == 0)
						{
							format += $"m\\m{delimiter}s\\s";
						}
						else
						{
                            format += $"m\\m";
						}
					}
					else
					{
                        format += $"m\\m{delimiter}s\\s";
					}
                }
			}
			else if (time.Minutes > 0)
			{
                //20m:0s
                format = $"m\\m{delimiter}s\\s";
			}

			else if(clipIfLong)
			{
                if (time.Seconds > 1 && time.Hours < 10 && time.Days == 0)
                {
                    //2.2s
                    format = decimals ? "s\\.f\\s" : "s\\s";
                }
                else if (time.Hours < 10 && time.Days == 0)
                {
                    //0.25s
                    format = decimals ? "s\\.ff\\s" : "s\\s";
                }
			}
			else
			{
				if (time.Seconds > 1)
                {
                    //2.2s
                    format = decimals ? "s\\.f\\s" : "s\\s";
                }
                else
                {
                    //0.25s
                    format = decimals ? "s\\.ff\\s" : "s\\s";
                }
			}

			return time.ToString(format);
		}

		public static double ToEpoch(this DateTime dateTime)
		{
			return (dateTime - HistoricOffset).TotalSeconds;
		}

		public static DateTime ToDateTime(this double epoch)
		{
			return HistoricOffset.AddSeconds(epoch);
		}

		public static DateTime ToDateTime(this int epoch)
		{
			return HistoricOffset.AddSeconds(epoch);
		}
    }
}

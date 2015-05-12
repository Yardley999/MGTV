using System;

namespace MGTV.Common
{
    public static class Extensions
    {
        /// <summary>
        /// return time format as hh:mm:ss
        /// </summary>
        /// <param name="timeSpan"></param>
        /// <returns></returns>
        public static string ToShortFromatString(this TimeSpan timeSpan)
        {
            int mins = 24 * 60 * timeSpan.Days + 60 * timeSpan.Hours + timeSpan.Minutes;
            return string.Format("{0:d2}:{1:d2}", mins, timeSpan.Seconds);
        }
    }
}

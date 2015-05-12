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
            return timeSpan.ToString(@"hh\:mm\:ss");
        }
    }
}

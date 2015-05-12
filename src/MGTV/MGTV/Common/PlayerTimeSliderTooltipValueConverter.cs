using System;
using Windows.UI.Xaml.Data;

namespace MGTV.Common
{
    public class PlayerTimeSliderTooltipValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            TimeSpan time = TimeSpan.Zero;

            if(value != null)
            {
                double doubleVal = Double.Parse(value.ToString());
                time = TimeSpan.FromSeconds(doubleVal);
            }

            return time.ToShortFromatString();

        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}

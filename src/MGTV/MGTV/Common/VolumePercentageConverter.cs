using System;
using Windows.UI.Xaml.Data;

namespace MGTV.Common
{
    public class VolumePercentageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            int percentage = 0;
            if (value != null)
            {
                percentage = int.Parse(double.Parse(value.ToString()).ToString());
            }

            return string.Format("{0}%", percentage);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}

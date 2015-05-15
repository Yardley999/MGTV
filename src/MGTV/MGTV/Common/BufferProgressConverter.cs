using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace MGTV.Common
{
    public class BufferProgressPercentageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            int percentage = 0;
            if(value != null)
            {
                percentage = int.Parse((double.Parse(value.ToString()) * 10000).ToString());
            }

            return string.Format("{0}%", percentage);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }


    public class BufferProgressToVisiblityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if(value == null)
            {
                return Visibility.Collapsed;
            }

            double val = double.Parse(value.ToString());

            return val >= 1 ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class BufferProgressToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
            {
                return Visibility.Collapsed;
            }

            double val = double.Parse(value.ToString());

            return val >= 1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}

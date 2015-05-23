using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace SharedFx.Data
{
    /// <summary>
    /// Value converter that translates true to <see cref="Visibility.Visible"/> and false to
    /// <see cref="Visibility.Collapsed"/>.
    /// </summary>
    public sealed class AnythingToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            bool result = (bool)(new AnythingToBooleanConverter()).Convert(value, targetType, parameter, language);
            return result ? Visibility.Visible : Visibility.Collapsed;

        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return value is Visibility && (Visibility)value == Visibility.Visible;
        }
    }

    public sealed class AnythingToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            bool result = false;

            bool negate = false;
            if (parameter != null && parameter.ToString().ToLower() == "neg")
            {
                negate = true;
            }

            if (value is bool)
            {
                result = (bool)value;
            }
            else if (value is string)
            {
                result = !string.IsNullOrEmpty(value.ToString().Trim());
            }
            else if (value is int)
            {
                result = (int)value > 0;
            }
            else if (value is double)
            {
                result = (double)value > 0;
            }
            else if (value is DateTime)
            {
                result = (DateTime)value > DateTime.Now;
            }
            else
            {
                result = value == null ? false : true;
            }

            if (negate)
            {
                result = !result;
            }

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public sealed class BooleanSwitchToSymbolIconConverter : IValueConverter
    {
        /// <param name="parameter">Format is [SymbolIconForTrue]_[SymbolIconForFlase]</param> 
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null
                || parameter == null)
            {
                return null;
            }

            string[] icons = parameter.ToString().Split(new char[] { '_' }, StringSplitOptions.RemoveEmptyEntries);

            if(icons.Length < 2)
            {
                return null;
            }

            bool isTure = bool.Parse(value.ToString());
            Symbol symbol;
            
            if (isTure)
            {
                Enum.TryParse<Symbol>(icons[0], true, out symbol);
            }
            else
            {
                Enum.TryParse<Symbol>(icons[1], true, out symbol);
            }

            return new SymbolIcon(symbol);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public sealed class StringFromatConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            string val = string.Empty;
            if(value != null)
            {
                val = value.ToString();
            }

            string format = string.Empty;
            if(parameter != null)
            {
                format = parameter.ToString();
            }

            if(string.IsNullOrEmpty(format))
            {
                return val;
            }
            else
            {
                return string.Format(format, val);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}

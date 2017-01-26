using System;
using System.Windows.Data;
using Microsoft.Web.Administration;

namespace SenseNet.Installer.Converters
{
    public class IdentityTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            ProcessModelIdentityType eValue;
            if (!Enum.TryParse((string) parameter, out eValue))
                return false;

            return (ProcessModelIdentityType)value == eValue;
        }
        
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (bool)value ? parameter : null;
        }
    }
}

using System.Globalization;

namespace DeviceManager.Mobile.Uteis
{
    public class BoolToStatusIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool synced)
                return synced ? "check.png" : "sync.png";
            return "sync.png";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}

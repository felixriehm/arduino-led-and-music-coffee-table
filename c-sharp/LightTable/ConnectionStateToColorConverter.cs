using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using LightTable.Controller;
using LightTable.Controller.Connection;

namespace LightTable
{
    public class ConnectionStateToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            ConnectionState state = (ConnectionState) value;
            switch (state)
            {
                case ConnectionState.Connected:
                    return new SolidColorBrush(Windows.UI.Colors.Green);
                case ConnectionState.Disconnected:
                    return new SolidColorBrush(Windows.UI.Colors.Red);
                case ConnectionState.Connecting:
                    return new SolidColorBrush(Windows.UI.Colors.Orange);
                case ConnectionState.Enumerating:
                    return new SolidColorBrush(Windows.UI.Colors.Orange);
                case ConnectionState.Enumerated:
                    return new SolidColorBrush(Windows.UI.Colors.Orange);
                case ConnectionState.Failure:
                    return new SolidColorBrush(Windows.UI.Colors.Red);
            }
            return new SolidColorBrush(Windows.UI.Colors.Red);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return true;
        }
    }
}

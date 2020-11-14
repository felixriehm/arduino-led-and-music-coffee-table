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
    public class ConnectionStateToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            ConnectionState state = (ConnectionState)value;
            switch (state)
            {
                case ConnectionState.Connected:
                    return "Mit Tisch verbunden.";
                case ConnectionState.Disconnected:
                    return "Mit Tisch nicht verbunden.";
                case ConnectionState.Connecting:
                    return "Verbinde mit Tisch ...";
                case ConnectionState.Enumerating:
                    return "Suche nach Tisch ...";
                case ConnectionState.Failure:
                    return "Verbindung fehgeschlagen.";
                case ConnectionState.Enumerated:
                    return "Es wurde(n) " + TableController.Instance.Btc.Devices.Count + " Gerät(e) gefunden";
            }
            return "Kein Status.";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return true;
        }
    }
}

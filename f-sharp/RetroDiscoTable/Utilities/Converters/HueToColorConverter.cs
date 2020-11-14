using RetroDiscoTable.Controller.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Data;

namespace RetroDiscoTable
{
    public class HueToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            int hue = (int)value;
            switch (hue)
            {
                case 700:
                    return Color.FromArgb(255, 0, 0, 0);
                case 701:
                    return Color.FromArgb(255, 255, 255, 255);
                default:
                    int r, g, b;
                    ColorConverter.HsvToRgb(hue, 1, 1, out r, out g, out b);
                    return Color.FromArgb(255, (byte)r, (byte)g, (byte)b);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return true;
        }
    }
}

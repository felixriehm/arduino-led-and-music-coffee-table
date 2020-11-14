using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using LightTable.ViewModel;

namespace LightTable.Model
{
    public class Tile : INotifyPropertyChanged
    {
        public int _x;
        public int _y;
        public Color _color;

        public int X
        {
            get { return _x; }
            set
            {
                if (value != _x)
                {
                    _x = value;
                    NotifyPropertyChanged("X");
                }
            }
        }

        public int Y
        {
            get { return _y; }
            set
            {
                if (value != _y)
                {
                    _y = value;
                    NotifyPropertyChanged("Y");
                }
            }
        }

        public Color Color
        {
            get { return _color; }
            set
            {
                if (value != _color)
                {
                    _color = value;
                    NotifyPropertyChanged("Color");
                }
            }
        }

        public Tile(int column, int row, Color color)
        {
            Color = color;
            X = row;
            Y = column;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged(String propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}

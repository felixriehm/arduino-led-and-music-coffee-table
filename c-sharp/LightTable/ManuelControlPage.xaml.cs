using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;
using LightTable.Controller;
using LightTable.Model;
using LightTable.ViewModel;

// Die Elementvorlage "Leere Seite" ist unter http://go.microsoft.com/fwlink/?LinkId=234238 dokumentiert.

namespace LightTable
{
    /// <summary>
    /// Eine leere Seite, die eigenständig verwendet werden kann oder auf die innerhalb eines Rahmens navigiert werden kann.
    /// </summary>
    public sealed partial class ManuelControlPage : Page
    {
        private ManuelControlViewModel mc;
        public ViewDirection UserViewDirection = ViewDirection.Right;

        public ManuelControlPage()
        {
            this.InitializeComponent();
            mc = new ManuelControlViewModel();
            this.DataContext = mc;
        }

        public enum ViewDirection
        {
            Up,
            Right,
            Down,
            Left
        }

        private void allOn_Tapped(object sender, TappedRoutedEventArgs e)
        {
            mc.allOn_Tapped();
        }

        private void allOff_Tapped(object sender, TappedRoutedEventArgs e)
        {
            mc.allOff_Tapped();
        }

        private void Grid_Tapped_1(object sender, TappedRoutedEventArgs e)
        {
            Grid grid = sender as Grid;
            SolidColorBrush brush = grid.Background as SolidColorBrush;
            mc.SelectedColor = brush.Color;
        }

        private void Grid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Grid grid = sender as Grid;
            Tile tile = grid.DataContext as Tile;
            mc.TableController.SetTile(tile.Y,tile.X, mc.SelectedColor);
        }

        private void leftRotation_Tapped(object sender, TappedRoutedEventArgs e)
        {
            UserViewDirection = (ViewDirection)Mod((int)UserViewDirection - 1, 4);
            ChangeDirections();
        }

        private void rightRotation_Tapped(object sender, TappedRoutedEventArgs e)
        {
            UserViewDirection = (ViewDirection)Mod((int)UserViewDirection + 1, 4);
            ChangeDirections();
        }

        private int Mod(int x, int m)
        {
            int r = x % m;
            return r < 0 ? r + m : r;
        }

        private void ChangeDirections()
        {
            RotateTransform rt = new RotateTransform();
            rt.CenterX = 0;//-gridView.Margin.Right*2;
            rt.CenterY = 0;//-gridView.Margin.Bottom*2;
            switch (UserViewDirection)
            {
                case ViewDirection.Up:
                    rt.Angle = 270;
                    image.Source = new BitmapImage(new Uri("ms-appx:///Assets/LightTableRotation1.png"));
                    break;
                case ViewDirection.Down:
                    rt.Angle = 90;
                    image.Source = new BitmapImage(new Uri("ms-appx:///Assets/LightTableRotation3.png"));
                    break;
                case ViewDirection.Left:
                    rt.Angle = 180;
                    image.Source = new BitmapImage(new Uri("ms-appx:///Assets/LightTableRotation.png"));
                    break;
                case ViewDirection.Right:
                    rt.Angle = 0;
                    image.Source = new BitmapImage(new Uri("ms-appx:///Assets/LightTableRotation2.png"));
                    break;
            }
            gridView.RenderTransform = rt;
        }
    }
}

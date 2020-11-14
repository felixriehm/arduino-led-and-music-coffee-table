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
using RetroDiscoTable.Controller;
using RetroDiscoTable.Controller.Utilities;
using ApplicationLogicLibrary.Controller;
using ApplicationLogicLibrary.Model;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.Graphics.Display;

// Die Elementvorlage "Leere Seite" ist unter http://go.microsoft.com/fwlink/?LinkId=234238 dokumentiert.

namespace RetroDiscoTable
{
    /// <summary>
    /// Eine leere Seite, die eigenständig verwendet werden kann oder auf die innerhalb eines Rahmens navigiert werden kann.
    /// </summary>
    public sealed partial class ManuelControlPage : Page
    {
        public ViewDirection UserViewDirection = ViewDirection.Left;

        public ObservableCollection<SelectionColor> ColorSelection { get; set; }
        public Color SelectedColor = Color.FromArgb(255, 131, 181, 221);

        public ManuelControlPage()
        {
            this.InitializeComponent();
            gridView.ItemsSource = Table.tiles;
            Events.LibraryEvents.Instance.AllTilesUpdateEvent += AllTilesUpdateEvent;
            Events.LibraryEvents.Instance.TileUpdateEvent += TileUpdateEvent;

            int r, g, b;
            ColorConverter.HsvToRgb(200, 1, 1, out r, out g, out b);
            SelectedColor = Color.FromArgb(255, (byte)r, (byte)g, (byte)b);

            ColorSelection = new ObservableCollection<SelectionColor>();
            ColorSelection.Add(new SelectionColor() { Color = Color.FromArgb(255, 0, 0, 0) });
            ColorSelection.Add(new SelectionColor() { Color = Color.FromArgb(255, 255, 255, 255) });
            ColorConverter.HsvToRgb(0, 1, 1, out r, out g, out b);
            ColorSelection.Add(new SelectionColor() { Color = Color.FromArgb(255, (byte)r, (byte)g, (byte)b) });
            ColorConverter.HsvToRgb(36, 1, 1, out r, out g, out b);
            ColorSelection.Add(new SelectionColor() { Color = Color.FromArgb(255, (byte)r, (byte)g, (byte)b) });
            ColorConverter.HsvToRgb(60, 1, 1, out r, out g, out b);
            ColorSelection.Add(new SelectionColor() { Color = Color.FromArgb(255, (byte)r, (byte)g, (byte)b) });
            ColorConverter.HsvToRgb(80, 1, 1, out r, out g, out b);
            ColorSelection.Add(new SelectionColor() { Color = Color.FromArgb(255, (byte)r, (byte)g, (byte)b) });
            ColorConverter.HsvToRgb(116, 1, 1, out r, out g, out b);
            ColorSelection.Add(new SelectionColor() { Color = Color.FromArgb(255, (byte)r, (byte)g, (byte)b) });
            ColorConverter.HsvToRgb(178, 1, 1, out r, out g, out b);
            ColorSelection.Add(new SelectionColor() { Color = Color.FromArgb(255, (byte)r, (byte)g, (byte)b) });
            ColorConverter.HsvToRgb(200, 1, 1, out r, out g, out b);
            ColorSelection.Add(new SelectionColor() { Color = Color.FromArgb(255, (byte)r, (byte)g, (byte)b) });
            ColorConverter.HsvToRgb(240, 1, 1, out r, out g, out b);
            ColorSelection.Add(new SelectionColor() { Color = Color.FromArgb(255, (byte)r, (byte)g, (byte)b) });
            ColorConverter.HsvToRgb(280, 1, 1, out r, out g, out b);
            ColorSelection.Add(new SelectionColor() { Color = Color.FromArgb(255, (byte)r, (byte)g, (byte)b) });
            ColorConverter.HsvToRgb(300, 1, 1, out r, out g, out b);
            ColorSelection.Add(new SelectionColor() { Color = Color.FromArgb(255, (byte)r, (byte)g, (byte)b) });

            this.DataContext = this;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            DisplayOrientations orientations = DisplayOrientations.None;
            orientations |= DisplayOrientations.Landscape;
            DisplayInformation.AutoRotationPreferences = orientations;
        }

        private void TileUpdateEvent(object sender, Events.TileUpdateEventArgs e)
        {
            CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
            () =>
            {
                //((GridViewItem)((GridViewItem) ------------- ).ContentTemplateRoot).Background = new SolidColorBrush(Color.FromArgb(255, (byte)r, (byte)g, (byte)b))
                var item = gridView.ItemsPanelRoot.Children.First(x => ((ModelTypes.Tile)(((GridViewItem)x).Content)).X == e.X && ((ModelTypes.Tile)(((GridViewItem)x).Content)).Y == e.Y);
                var item2 = ((GridViewItem)item).ContentTemplateRoot;
                ((Grid)item2).Background = new SolidColorBrush(ColorConverter.HueToColor(e.Hue));
            });
        }

        private void AllTilesUpdateEvent(object sender, Events.AllTilesUpdateEventArgs e)
        {
            CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
            () =>
            {
                for (int i = 0; i < e.Tiles.Count(); i++)
                {
                    var item2 = ((GridViewItem)gridView.ItemsPanelRoot.Children.ElementAt(i)).ContentTemplateRoot;
                    ((Grid)item2).Background = new SolidColorBrush(ColorConverter.HueToColor(e.Tiles.ElementAt(i).Hue));
                }
            });
        }

        public enum ViewDirection
        {
            Up,
            Right,
            Down,
            Left
        }

        public class SelectionColor
        {
            public Color Color { get; set; }
        }

        private void allOn_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (SelectedColor.R == 0 && SelectedColor.G == 0 && SelectedColor.B == 0)
            {
                Table.setAllTilesToColor(370);
                return;
            }
            if (SelectedColor.R == 255 && SelectedColor.G == 255 && SelectedColor.B == 255)
            {
                Table.setAllTilesToColor(371);
                return;
            }
            Table.setAllTilesToColor((int)ColorConverter.RGB2HSV(SelectedColor)[0]);
        }

        private void allOff_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Table.setAllTilesToColor(370);
        }

        private void Grid_Tapped_1(object sender, TappedRoutedEventArgs e)
        {
            Grid grid = sender as Grid;
            SolidColorBrush brush = grid.Background as SolidColorBrush;
            SelectedColor = brush.Color;
        }

        private void Grid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Grid grid = sender as Grid;
            ModelTypes.Tile tile = grid.DataContext as ModelTypes.Tile;
            if(SelectedColor.R == 0 && SelectedColor.G == 0 && SelectedColor.B == 0)
            {
                Table.setTile(tile.X, tile.Y, 370);
                return;
            }
            if (SelectedColor.R == 255 && SelectedColor.G == 255 && SelectedColor.B == 255)
            {
                Table.setTile(tile.X, tile.Y, 371);
                return;
            }
            Table.setTile(tile.X,tile.Y, (int)ColorConverter.RGB2HSV(SelectedColor)[0]);
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
                    image.Source = new BitmapImage(new Uri("ms-appx:///Assets/RetroDiscoTableRotation1.png"));
                    break;
                case ViewDirection.Down:
                    rt.Angle = 90;
                    image.Source = new BitmapImage(new Uri("ms-appx:///Assets/RetroDiscoTableRotation3.png"));
                    break;
                case ViewDirection.Left:
                    rt.Angle = 180;
                    image.Source = new BitmapImage(new Uri("ms-appx:///Assets/RetroDiscoTableRotation.png"));
                    break;
                case ViewDirection.Right:
                    rt.Angle = 0;
                    image.Source = new BitmapImage(new Uri("ms-appx:///Assets/RetroDiscoTableRotation2.png"));
                    break;
            }
            gridView.RenderTransform = rt;
        }

        private void allOn_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var tile = (GridViewItem)gridView.ItemsPanelRoot.Children.First();
            var selectionTile = (GridViewItem)colorSelection.ItemsPanelRoot.Children.First();
            if (allOn.Height == 40)
            {
                tile.Height = 90;
                tile.Width = 90;
                selectionTile.MinHeight = 50;
                selectionTile.MinWidth = 50;
                selectionTile.Height = 50;
                selectionTile.Width = 50;
            }
            else
            {
                tile.Height = 30;
                tile.Width = 30;
                selectionTile.MinHeight = 20;
                selectionTile.MinWidth = 20;
                selectionTile.Height = 20;
                selectionTile.Width = 20;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using LightTable.Model;
using LightTable.ViewModel;

// Die Elementvorlage "Leere Seite" ist unter http://go.microsoft.com/fwlink/?LinkId=234238 dokumentiert.

namespace LightTable
{
    /// <summary>
    /// Eine leere Seite, die eigenständig verwendet werden kann oder auf die innerhalb eines Rahmens navigiert werden kann.
    /// </summary>
    public sealed partial class GamesPage : Page
    {
        private GamesPageViewModel gpvm;

        public ViewDirection UserViewDirection = ViewDirection.Up;

        public GamesPage()
        {
            this.InitializeComponent();
            gpvm = new GamesPageViewModel();
            this.DataContext = gpvm;
        }

        public enum ViewDirection
        {
            Up,
            Right,
            Down,
            Left
        }

        private void start_Tapped(object sender, TappedRoutedEventArgs e)
        {
            gpvm.StartGame((Game)listView.SelectedItem);
        }

        private void stop_Tapped(object sender, TappedRoutedEventArgs e)
        {
            gpvm.StopGame();
        }

        private void up_Tapped(object sender, TappedRoutedEventArgs e)
        {
            gpvm.GameInput(Game.GameUserInput.Up);
        }

        private void down_Tapped(object sender, TappedRoutedEventArgs e)
        {
            gpvm.GameInput(Game.GameUserInput.Down);
        }

        private void right_Tapped(object sender, TappedRoutedEventArgs e)
        {
            gpvm.GameInput(Game.GameUserInput.Right);
        }

        private void left_Tapped(object sender, TappedRoutedEventArgs e)
        {
            gpvm.GameInput(Game.GameUserInput.Left);
        }

        private void action_Tapped(object sender, TappedRoutedEventArgs e)
        {
            gpvm.GameInput(Game.GameUserInput.Action);
        }

        private void leftRotation_Tapped(object sender, TappedRoutedEventArgs e)
        {
            UserViewDirection = (ViewDirection)Mod((int)UserViewDirection - 1,4);
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
            up.Tapped -= up_Tapped;
            up.Tapped -= down_Tapped;
            up.Tapped -= left_Tapped;
            up.Tapped -= right_Tapped;
            down.Tapped -= up_Tapped;
            down.Tapped -= down_Tapped;
            down.Tapped -= left_Tapped;
            down.Tapped -= right_Tapped;
            left.Tapped -= up_Tapped;
            left.Tapped -= down_Tapped;
            left.Tapped -= left_Tapped;
            left.Tapped -= right_Tapped;
            right.Tapped -= up_Tapped;
            right.Tapped -= down_Tapped;
            right.Tapped -= left_Tapped;
            right.Tapped -= right_Tapped;

            switch (UserViewDirection)
            {
               case ViewDirection.Up:
                    up.Tapped += up_Tapped;
                    down.Tapped += down_Tapped;
                    left.Tapped += left_Tapped;
                    right.Tapped += right_Tapped;
                    image.Source = new BitmapImage(new Uri("ms-appx:///Assets/LightTableRotation1.png"));
                    break;
                case ViewDirection.Down:
                    up.Tapped += down_Tapped;
                    down.Tapped += up_Tapped;
                    left.Tapped += right_Tapped;
                    right.Tapped += left_Tapped;
                    image.Source = new BitmapImage(new Uri("ms-appx:///Assets/LightTableRotation3.png"));
                    break;
                case ViewDirection.Left:
                    up.Tapped += right_Tapped;
                    down.Tapped += left_Tapped;
                    left.Tapped += up_Tapped;
                    right.Tapped += down_Tapped;
                    image.Source = new BitmapImage(new Uri("ms-appx:///Assets/LightTableRotation.png"));
                    break;
                case ViewDirection.Right:
                    up.Tapped += left_Tapped;
                    down.Tapped += right_Tapped;
                    left.Tapped += down_Tapped;
                    right.Tapped += up_Tapped;
                    image.Source = new BitmapImage(new Uri("ms-appx:///Assets/LightTableRotation2.png"));
                    break;
            }
        }
    }
}

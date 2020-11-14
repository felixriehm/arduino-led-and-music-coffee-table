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
using RetroDiscoTable.Controller;
using ApplicationLogicLibrary.Controller;
using ApplicationLogicLibrary.Model;
using Windows.Graphics.Display;

// Die Elementvorlage "Leere Seite" ist unter http://go.microsoft.com/fwlink/?LinkId=234238 dokumentiert.

namespace RetroDiscoTable
{
    /// <summary>
    /// Eine leere Seite, die eigenständig verwendet werden kann oder auf die innerhalb eines Rahmens navigiert werden kann.
    /// </summary>
    public sealed partial class GamesPage : Page
    {

        public ViewDirection UserViewDirection = ViewDirection.Left;

        public GamesPage()
        {
            this.InitializeComponent();
            listView.ItemsSource = GameController.games;
            this.DataContext = this;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            DisplayOrientations orientations = DisplayOrientations.None;
            orientations |= DisplayOrientations.Portrait;
            DisplayInformation.AutoRotationPreferences = orientations;
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
            GameController.startGame((ModelTypes.Game)listView.SelectedItem);
        }

        private void stop_Tapped(object sender, TappedRoutedEventArgs e)
        {
            GameController.stopGame();
        }

        private void up_Tapped(object sender, TappedRoutedEventArgs e)
        {
            GameController.UserInput = ControllerTypes.UserInput.Left;
            GameController.userInputAvailable = true;
        }

        private void down_Tapped(object sender, TappedRoutedEventArgs e)
        {
            GameController.UserInput = ControllerTypes.UserInput.Right;
            GameController.userInputAvailable = true;
        }

        private void right_Tapped(object sender, TappedRoutedEventArgs e)
        {
            GameController.UserInput = ControllerTypes.UserInput.Up;
            GameController.userInputAvailable = true;
        }

        private void left_Tapped(object sender, TappedRoutedEventArgs e)
        {
            GameController.UserInput = ControllerTypes.UserInput.Down;
            GameController.userInputAvailable = true;
        }

        private void action_Tapped(object sender, TappedRoutedEventArgs e)
        {
            GameController.UserInput = ControllerTypes.UserInput.Action;
            GameController.userInputAvailable = true;
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
                    image.Source = new BitmapImage(new Uri("ms-appx:///Assets/RetroDiscoTableRotation1.png"));
                    break;
                case ViewDirection.Down:
                    up.Tapped += down_Tapped;
                    down.Tapped += up_Tapped;
                    left.Tapped += right_Tapped;
                    right.Tapped += left_Tapped;
                    image.Source = new BitmapImage(new Uri("ms-appx:///Assets/RetroDiscoTableRotation3.png"));
                    break;
                case ViewDirection.Left:
                    up.Tapped += right_Tapped;
                    down.Tapped += left_Tapped;
                    left.Tapped += up_Tapped;
                    right.Tapped += down_Tapped;
                    image.Source = new BitmapImage(new Uri("ms-appx:///Assets/RetroDiscoTableRotation.png"));
                    break;
                case ViewDirection.Right:
                    up.Tapped += left_Tapped;
                    down.Tapped += right_Tapped;
                    left.Tapped += down_Tapped;
                    right.Tapped += up_Tapped;
                    image.Source = new BitmapImage(new Uri("ms-appx:///Assets/RetroDiscoTableRotation2.png"));
                    break;
            }
        }
    }
}

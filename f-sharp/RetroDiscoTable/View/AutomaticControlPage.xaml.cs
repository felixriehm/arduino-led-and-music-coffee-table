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
    public sealed partial class AutomaticControlPage : Page
    {
        public AutomaticControlPage()
        {
            this.InitializeComponent();
            listView.ItemsSource = ModiController.modis;
            this.DataContext = this;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            DisplayOrientations orientations = DisplayOrientations.None;
            orientations |= DisplayOrientations.Portrait;
            DisplayInformation.AutoRotationPreferences = orientations;
        }

        private void play_Tapped(object sender, TappedRoutedEventArgs e)
        {
            ModiController.startModi((ModelTypes.Modi)listView.SelectedItem);
        }

        private void stop_Tapped(object sender, TappedRoutedEventArgs e)
        {
            ModiController.stopModi();
        }
    }
}

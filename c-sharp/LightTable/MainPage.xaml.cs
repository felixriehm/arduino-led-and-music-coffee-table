using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Enumeration;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using LightTable.Controller;
using LightTable.Controller.Connection;
using LightTable.ViewModel;

// Die Vorlage "Leere Seite" ist unter http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409 dokumentiert.

namespace LightTable
{
    /// <summary>
    /// Eine leere Seite, die eigenständig verwendet werden kann oder auf die innerhalb eines Rahmens navigiert werden kann.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public static MainPage Current;
        private MainPageViewModel mpvm;
        List<Scenario> scenarios = new List<Scenario>
        {
            new Scenario() { Title="Verbindung", ClassType=typeof(ConnectionPage)},
            new Scenario() { Title="Manuelle Ansteuerung", ClassType=typeof(ManuelControlPage)},
            new Scenario() { Title="Automatische Ansteuerung", ClassType=typeof(AutomaticControlPage)},
            new Scenario() { Title="Spiele", ClassType=typeof(GamesPage)},
            new Scenario() { Title="Musik", ClassType=typeof(MusicPage)}
        };

        public List<Scenario> Scenarios
        {
            get { return this.scenarios; }
        }

        public class Scenario
        {
            public string Title { get; set; }
            public Type ClassType { get; set; }
        }

        public MainPage()
        {
            this.InitializeComponent();
            Current = this;
            mpvm = new MainPageViewModel();
            this.DataContext = mpvm;
            textBox.TextChanged += TextBox_TextChanged;
            Debugger.Instance.MessageReceived += BluetoothManager_MessageReceived;
        }

        /// <summary>
        /// Called whenever the user changes selection in the scenarios list.  This method will navigate to the respective
        /// sample scenario page.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ScenarioControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox scenarioListBox = sender as ListBox;
            Scenario s = scenarioListBox.SelectedItem as Scenario;
            if (s != null)
            {
                ScenarioFrame.Navigate(s.ClassType);
                if (Window.Current.Bounds.Width < 640)
                {
                    Splitter.IsPaneOpen = false;
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Splitter.IsPaneOpen = !Splitter.IsPaneOpen;
        }

        async void Footer_Click(object sender, RoutedEventArgs e)
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri(((HyperlinkButton)sender).Tag.ToString()));
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // Populate the scenario list from the SampleConfiguration.cs file
            ScenarioControl.ItemsSource = scenarios;
            if (Window.Current.Bounds.Width < 640)
            {
                ScenarioControl.SelectedIndex = -1;
            }
            else
            {
                ScenarioControl.SelectedIndex = 0;
            }
        }

        private async void BluetoothManager_MessageReceived(object sender, string message, Debugger.Device dev)
        {
            textBox.Text = textBox.Text + "[" + String.Format("{0:T}", DateTime.Now) + "] - " + dev.ToString() + ":\n" + message + "\n\n";
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var grid = (Grid)VisualTreeHelper.GetChild(textBox, 0);
            for (var i = 0; i <= VisualTreeHelper.GetChildrenCount(grid) - 1; i++)
            {
                object obj = VisualTreeHelper.GetChild(grid, i);
                if (!(obj is ScrollViewer)) continue;
                ((ScrollViewer)obj).ChangeView(0.0f, ((ScrollViewer)obj).ExtentHeight, 1.0f);
                break;
            }
        }

        private void freeRam_Tapped(object sender, TappedRoutedEventArgs e)
        {
            mpvm.ReportFreeRam();
        }
    }
}

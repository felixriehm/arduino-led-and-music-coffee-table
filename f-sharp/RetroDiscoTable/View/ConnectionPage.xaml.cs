using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Core;
using Windows.Devices.Enumeration;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using RetroDiscoTable.Controller.Connection;
using RetroDiscoTable.Controller;
using Windows.Graphics.Display;

// Die Elementvorlage "Leere Seite" ist unter http://go.microsoft.com/fwlink/?LinkId=234238 dokumentiert.

namespace RetroDiscoTable
{
    /// <summary>
    /// Eine leere Seite, die eigenständig verwendet werden kann oder auf die innerhalb eines Rahmens navigiert werden kann.
    /// </summary>
    public sealed partial class ConnectionPage : Page
    {
        public ConnectionPage()
        {
            this.InitializeComponent();
            comboBoxConnectionType.ItemsSource =  new List<string>()
            {
                "Bluetooth",
                "USB",
            };
            this.DataContext = this;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            DisplayOrientations orientations = DisplayOrientations.None;
            orientations |= DisplayOrientations.Portrait;
            DisplayInformation.AutoRotationPreferences = orientations;
        }

        private void connect_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Configuration.Instance.Connection.ConnectToDevice((DeviceInformation) comboBox.SelectedItem).ContinueWith((t) => {
                if (!t.IsCanceled && !t.IsFaulted){
                    ApplicationLogicLibrary.Controller.ConnectionController.Connected();
                }
            });
        }

        private void disconnect_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Configuration.Instance.Connection.Disconnect();
        }

        private void search_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Configuration.Instance.Connection.LookForDevices().ContinueWith((t) => getDevices());
        }

        private void getDevices()
        {
            CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
            () =>
            {
                if (Configuration.Instance.Connection.Devices.Count != 0)
                {
                    comboBox.ItemsSource = Configuration.Instance.Connection.Devices;
                    comboBox.SelectedIndex = 0;
                }
            });
        }

        private void comboBoxConnectionType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((string)comboBoxConnectionType.SelectedItem == "Bluetooth")
            {
                Configuration.Instance.ChangeConnectionType(BluetoothConnection.Instance);
            }
            else
            {
                Configuration.Instance.ChangeConnectionType(SerialConnection.Instance);
            }
            Configuration.Instance.Connection.LookForDevices().ContinueWith((t) => getDevices());
        }
    }
}

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
using LightTable.ViewModel;
using LightTable.Controller.Connection;

// Die Elementvorlage "Leere Seite" ist unter http://go.microsoft.com/fwlink/?LinkId=234238 dokumentiert.

namespace LightTable
{
    /// <summary>
    /// Eine leere Seite, die eigenständig verwendet werden kann oder auf die innerhalb eines Rahmens navigiert werden kann.
    /// </summary>
    public sealed partial class ConnectionPage : Page
    {
        private ConnectionPageViewModel cpvm;
        SerialConnection s;
        public ConnectionPage()
        {
            this.InitializeComponent();
            cpvm = new ConnectionPageViewModel();
            this.DataContext = cpvm;
        }

        private async void bla()
        {
            var newView = CoreApplication.CreateNewView();
            int newViewId = 0;
            await newView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                var frame = new Frame();
                frame.Navigate(typeof(MusicPage));
                Window.Current.Content = frame;

                newViewId = ApplicationView.GetForCurrentView().Id;
            });
            bool viewShown = await ApplicationViewSwitcher.TryShowAsStandaloneAsync(newViewId);
        }

        private void connect_Tapped(object sender, TappedRoutedEventArgs e)
        {
            /*s = new SerialConnection();
            s.ConnectToDevice(null);*/
            cpvm.Connect((DeviceInformation) comboBox.SelectedItem);
        }

        private void disconnect_Tapped(object sender, TappedRoutedEventArgs e)
        {
            //s.Disconnect();
            cpvm.Disconnect();
        }

        private void search_Tapped(object sender, TappedRoutedEventArgs e)
        {
            cpvm.SearchDevices();
        }
    }
}

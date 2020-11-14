using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;

namespace RetroDiscoTable.Controller.Connection
{
    class MockUpConnection : IConnection, INotifyPropertyChanged
    {

        private static MockUpConnection instance;

        private MockUpConnection()
        {
            State = ConnectionState.Disconnected;
        }

        public static MockUpConnection Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new MockUpConnection();
                }
                return instance;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)//Attention: usially: protected void
        {
            var eventHandler = this.PropertyChanged;
            if (eventHandler != null)
                eventHandler(this, new PropertyChangedEventArgs(propertyName));
        }

        public ObservableCollection<DeviceInformation> Devices { get; set; }
        private ConnectionState _State;
        public ConnectionState State { get { return _State; } set { _State = value; OnPropertyChanged("State"); } }

        public Task ConnectToDevice(DeviceInformation device)
        {
            State = ConnectionState.Connected;
            return null;
        }

        public void Disconnect()
        {
            State = ConnectionState.Disconnected;
        }

        public Task LookForDevices()
        {
            State = ConnectionState.Enumerating;
            return null;
        }

        public void SendCommand(string s)
        {

        }
    }
}

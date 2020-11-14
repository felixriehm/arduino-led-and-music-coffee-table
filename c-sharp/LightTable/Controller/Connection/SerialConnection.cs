using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.Rfcomm;
using Windows.Devices.Enumeration;
using Windows.Devices.SerialCommunication;
using Windows.Foundation;
using Windows.Networking.Proximity;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;

namespace LightTable.Controller.Connection
{
    public class SerialConnection : IConnection, INotifyPropertyChanged
    {
        private Debugger Debugger;

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)//Attention: usially: protected void
        {
            var eventHandler = this.PropertyChanged;
            if (eventHandler != null)
                eventHandler(this, new PropertyChangedEventArgs(propertyName));
        }

        public ObservableCollection<DeviceInformation> Devices { get; set; }

        private IAsyncAction connectAction;
        private SerialDevice serialConnection;
        private StreamSocket socket;
        private DataReader reader;
        private DataWriter writer;

        private ConnectionState _State;
        public ConnectionState State { get { return _State; } set { _State = value; OnPropertyChanged("State"); } }

        private static SerialConnection instance;

        private SerialConnection()
        {
            Devices = new ObservableCollection<DeviceInformation>();
            State = ConnectionState.Disconnected;
            Debugger = Debugger.Instance;
            LookForBluetoothDevices();
        }

        public static SerialConnection Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new SerialConnection();
                }
                return instance;
            }
        }

        public async Task ConnectToDevice(DeviceInformation device)
        {
            this.State = ConnectionState.Connecting;
            try
            {
                serialConnection = await SerialDevice.FromIdAsync(device.Id);
                if (serialConnection != null)
                {
                    serialConnection.BaudRate = 115200;
                    serialConnection.DataBits = 8;
                    serialConnection.Parity = SerialParity.None;
                    writer = new DataWriter(serialConnection.OutputStream);
                    reader = new DataReader(serialConnection.InputStream);
                    Task listen = ListenForMessagesAsync();
                    this.State = ConnectionState.Connected;
                }
                else {
                    Debugger.ReportToDebugger(this, "Unable to create service.\nMake sure that the 'serialcommunication' capability is declared with a function of type 'name:serialPort' in Package.appxmanifest.", Debugger.Device.Pc);
                    this.State = ConnectionState.Failure;
                }
            }
            catch (TaskCanceledException ex)
            {
                this.State = ConnectionState.Failure;
                Debugger.ReportToDebugger(this, ex.Message, Debugger.Device.Pc);
            }
            catch (Exception ex)
            {
                this.State = ConnectionState.Failure;
                Debugger.ReportToDebugger(this, ex.Message, Debugger.Device.Pc);
            }
        }

        public void Disconnect()
        {
            if (reader != null)
            {
                reader.DetachStream();
                reader = null;
            } 
            if (writer != null)
            {
                writer.DetachStream();
                writer = null;
            }
            if (socket != null)
            {
                socket.Dispose();
                socket = null;
            }
            if (serialConnection != null)
            {
                serialConnection.Dispose();
                serialConnection = null;
            }
            this.State = ConnectionState.Disconnected;
        }

        private async Task<uint> SendMessageAsync(string message)
        {
            uint sentMessageSize = 0;
            try
            {
                if (writer != null)
                {
                    sentMessageSize = writer.WriteString(message);
                    await writer.StoreAsync();
                }
            }
            catch (Exception ex)
            {
                this.State = ConnectionState.Disconnected;
                Debugger.ReportToDebugger(this, ex.Message, Debugger.Device.Pc);
            }
            return sentMessageSize;
        }

        private async Task ListenForMessagesAsync()
        {
            while (reader != null)
            {
                try
                {
                    // Read first byte (length of the subsequent message, 255 or less). 
                    uint sizeFieldCount = await reader.LoadAsync(1);
                    if (sizeFieldCount != 1)
                    {
                        // The underlying socket was closed before we were able to read the whole data. 
                        return;
                    }

                    // Read the message. 
                    
                    uint messageLength = reader.ReadByte();
                    uint actualMessageLength = await reader.LoadAsync(messageLength);
                    if (messageLength != actualMessageLength)
                    {
                        // The underlying socket was closed before we were able to read the whole data. 
                        return;
                    }
                    // Read the message and process it.
                    string message = reader.ReadString(actualMessageLength);
                    Debugger.ReportToDebugger(this, message, Debugger.Device.Arduino);
                }
                catch (Exception ex)
                {
                    if (reader != null)
                        Debugger.ReportToDebugger(this, ex.Message, Debugger.Device.Pc);
                }
            }
        }

        public async Task LookForBluetoothDevices()
        {
            Devices.Clear();
            var serviceInfoCollection = await DeviceInformation.FindAllAsync(SerialDevice.GetDeviceSelector());
            foreach (var serviceInfo in serviceInfoCollection)
            {
                Devices.Add(serviceInfo);
            }
        }
        public async void SendCommand(string s)
        {
            await SendMessageAsync(s);
        }
    }
}

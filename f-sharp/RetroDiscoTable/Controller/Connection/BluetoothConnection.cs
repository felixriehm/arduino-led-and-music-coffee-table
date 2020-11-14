using ApplicationLogicLibrary.Model;
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

namespace RetroDiscoTable.Controller.Connection
{
    public class BluetoothConnection : IConnection, INotifyPropertyChanged
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
        private RfcommDeviceService rfcommService;
        private StreamSocket socket;
        private DataReader reader;
        private DataWriter writer;

        private ConnectionState _State;
        public ConnectionState State { get { return _State; } set { _State = value; OnPropertyChanged("State"); } }

        private static BluetoothConnection instance;

        private BluetoothConnection()
        {
            Events.LibraryEvents.Instance.ArduinoCommandEvent += OnArduinoCommandEvent;
            Devices = new ObservableCollection<DeviceInformation>();
            State = ConnectionState.Disconnected;
            Debugger = Debugger.Instance;
        }

        public static BluetoothConnection Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new BluetoothConnection();
                }
                return instance;
            }
        }

        private void OnArduinoCommandEvent(object sender, Events.ArduinoCommandEventArgs e)
        {
            SendCommand(e.Command);
        }

        public async Task ConnectToDevice(DeviceInformation device)
        {
            this.State = ConnectionState.Connecting;
            try
            {
                // Initialize the target Bluetooth RFCOMM device service
                rfcommService = await RfcommDeviceService.FromIdAsync(device.Id);
                if (rfcommService != null)
                {
                    // Create a socket and connect to the target 
                    socket = new StreamSocket();
                    connectAction = socket.ConnectAsync(rfcommService.ConnectionHostName, rfcommService.ConnectionServiceName, SocketProtectionLevel.BluetoothEncryptionAllowNullAuthentication);
                    await connectAction;//to make it cancellable
                    writer = new DataWriter(socket.OutputStream);
                    reader = new DataReader(socket.InputStream);
                    Task listen = ListenForMessagesAsync();
                    this.State = ConnectionState.Connected;
                }
                else {
                    Debugger.ReportToDebugger(this, "Unable to create service.\nMake sure that the 'bluetooth.rfcomm' capability is declared with a function of type 'name:serialPort' in Package.appxmanifest.",Debugger.Device.Pc);
                    this.State = ConnectionState.Failure;
                }
            }
            catch (TaskCanceledException ex)
            {
                this.State = ConnectionState.Failure;
                Debugger.ReportToDebugger(this,ex.Message,Debugger.Device.Pc);
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
                reader.Dispose();
                reader = null;
            }
            if (writer != null)
            {
                writer.Dispose();
                writer = null;
            }
            if (socket != null)
            {
                socket.Dispose();
                socket = null;
            }
            if (rfcommService != null)
            {
                rfcommService.Dispose();
                rfcommService = null;
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

        public async Task LookForDevices()
        {
            State = ConnectionState.Enumerating;
            Devices.Clear();
            var serviceInfoCollection = await DeviceInformation.FindAllAsync(RfcommDeviceService.GetDeviceSelector(RfcommServiceId.SerialPort));
            foreach (var serviceInfo in serviceInfoCollection)
            {
                Devices.Add(serviceInfo);
            }
            State = ConnectionState.Enumerated;
        }

        public async void SendCommand(string s)
        {
            await SendMessageAsync(s);
        }
    }
}

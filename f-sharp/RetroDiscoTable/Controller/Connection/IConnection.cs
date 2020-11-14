using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;

namespace RetroDiscoTable.Controller.Connection
{
    public interface IConnection
    {
        ObservableCollection<DeviceInformation> Devices { get; set; }

        ConnectionState State { get; set; }

        Task ConnectToDevice(DeviceInformation device);

        void Disconnect();

        Task LookForDevices();

        void SendCommand(string s);
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using LightTable.Controller;

namespace LightTable.ViewModel
{
    public class ConnectionPageViewModel
    {
        public TableController TableController { get; set; }

        public ConnectionPageViewModel()
        {
            TableController = TableController.Instance;
        }

        public void Connect(DeviceInformation device)
        {
            TableController.ConnectToBluetooth(device);
        }

        public void SearchDevices()
        {
            TableController.SearchDevices();
        }

        public void Disconnect()
        {
            TableController.Disconnect();
        }
    }
}

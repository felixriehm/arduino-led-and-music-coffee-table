using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using LightTable.Controller;
using LightTable.Model;

namespace LightTable.ViewModel
{
    public class MainPageViewModel
    {
        public TableController TableController { get; set; }

        public MainPageViewModel()
        {
            TableController = TableController.Instance;
        }

        public void ReportFreeRam()
        {
            TableController.ReportFreeRam();
        }
    }
}

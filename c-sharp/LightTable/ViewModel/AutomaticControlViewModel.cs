using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LightTable.Controller;
using LightTable.Model;

namespace LightTable.ViewModel
{
    public class AutomaticControlViewModel
    {
        public TableController TableController { get; set; }

        public AutomaticControlViewModel()
        {
            TableController = TableController.Instance;
        }

        public void PlayModi(Modi modi)
        {
            TableController.StartModi(modi);
        }

        public void StopModi()
        {
            TableController.StopModi();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightTable.Controller
{
    public class Debugger
    {
        //OnMessageReceived
        public delegate void AddOnMessageReceivedDelegate(object sender, string message, Device device);
        public event AddOnMessageReceivedDelegate MessageReceived;
        private void OnMessageReceivedEvent(object sender, string message, Device device)
        {
            if (MessageReceived != null)
                MessageReceived(sender, message, device);
        }

        private static Debugger instance;

        public static Debugger Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Debugger();
                }
                return instance;
            }
        }

        public void ReportToDebugger(object sender, string message, Device device)
        {
            OnMessageReceivedEvent(sender, message, device);
        }

        public enum Device
        {
            Arduino,
            Pc
        }
    }
}

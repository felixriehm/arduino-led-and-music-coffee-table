using RetroDiscoTable.Controller.Connection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetroDiscoTable.Controller
{
    public class Configuration : INotifyPropertyChanged
    {
        public IConnection _connection;

        public IConnection Connection
        {
            get { return _connection; }
            set
            {
                if (value != _connection)
                {
                    _connection = value;
                    NotifyPropertyChanged("Connection");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged(String propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private static Configuration instance;

        private Configuration()
        {
            
        }

        public static Configuration Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Configuration();
                }
                return instance;
            }
        }

        public void InitConfig()
        {
            Connection = BluetoothConnection.Instance;
        }

        public void ChangeConnectionType(IConnection c)
        {
            Connection.Disconnect();
            Connection = c;
        }
    }
}

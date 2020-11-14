using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Foundation.Metadata;
using Windows.UI;
using LightTable.Controller.Connection;
using LightTable.Model;

namespace LightTable.Controller
{
    public class TableController : INotifyPropertyChanged
    {
        public int Columns { get; private set; }
        public int Rows { get; private set; }
        public Table Table { get; set; }
        private Debugger Debugger { get; set; }

        public string StatusMsg { get; set; }
        public NotifyType StatusType { get; set; }

        public bool _modiIsPlaying = false;

        public bool ModiIsPlaying
        {
            get { return _modiIsPlaying; }
            set
            {
                if (value != _modiIsPlaying)
                {
                    _modiIsPlaying = value;
                    NotifyPropertyChanged("ModiIsPlaying");
                }
            }
        }

        public bool _musicIsPlaying;

        public bool MusicIsPlaying
        {
            get { return _musicIsPlaying; }
            set
            {
                if (value != _musicIsPlaying)
                {
                    _musicIsPlaying = value;
                    NotifyPropertyChanged("MusicIsPlaying");
                }
            }
        }

        private Game.GameUserInput CurrentGameUserInput { get; set; }

        private Game CurrentGame { get; set; }

        public IConnection Btc { get; set; }
        private static TableController instance;

        public TableController()
        {
            Columns = 8;
            Rows = 6;
            ModiIsPlaying = false;
            MusicIsPlaying = false;

            Btc = SerialConnection.Instance;
            //Btc = BluetoothConnection.Instance;
            //Btc = BluetoothConnectionMockUp.Instance;
            Debugger = Debugger.Instance;

            Table = new Table(Columns,Rows,Colors.Black);
        }

        public static TableController Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new TableController();
                }
                return instance;
            }
        }

        public void SetTile(int column, int row, Color color)
        {
            Table.SetTile(column, row, color);
            int startPixel;
            if (row % 2 != 0)
            {
                startPixel = row * Columns + column;
            }
            else
            {
                startPixel = row * Columns + Columns-1 - (column);
            }
            startPixel = startPixel * 3;
            Btc.SendCommand(Command.ArduinoCommand(CommandName.SetTile, new List<string>() { startPixel.ToString(), color.R.ToString(), color.G.ToString(), color.B.ToString() }));
        }

        public void SetAllTiles(Color color)
        {
            Table.SetAllTiles(color);
            Btc.SendCommand(Command.ArduinoCommand(CommandName.SetAllPixel, new List<string>() { color.R.ToString(), color.G.ToString(), color.B.ToString() }));
        }

        public void StartModi(Model.Modi modi)
        {
            /*var fileManager = new PortableLibrary1.TestModule.FileTask();
            fileManager.TaskCompletedEvent += new EventHandler<PortableLibrary1.TestModule.FileTaskEventArgs>(OnFinishedFileTask);*/
            /*******************************************************************/
            /*if(modi.CommandName == "9")
            {
                Controller.Modi.RandomHsvModi rndHsv = new Controller.Modi.RandomHsvModi(this);
                rndHsv.Start();
                return;
            }*/

            if (modi != null)
            {
                ModiIsPlaying = true;
                Btc.SendCommand(Command.ArduinoCommand(modi.CommandName));
            }
        }

        /*private static void OnFinishedFileTask(object sender, PortableLibrary1.TestModule.FileTaskEventArgs e)
        {
            
        }*/

        public void StartGame(Game g)
        {
            if (g != null)
            {
                ModiIsPlaying = true;
                CurrentGameUserInput = Game.GameUserInput.Right;
                CurrentGame = g;
                Btc.SendCommand(Command.ArduinoCommand(g.CommandName));
            }
        }

        public void GameUserInput(Game.GameUserInput direction)
        {
            byte directionCommand = 0;
            switch (direction)
            {
                case Game.GameUserInput.Right:
                    directionCommand = 0;
                    if (CurrentGame.Name == "Snake" && CurrentGameUserInput == Game.GameUserInput.Left)
                    {
                        return;
                    }
                    CurrentGameUserInput = Game.GameUserInput.Right;
                    break;
                case Game.GameUserInput.Up:
                    directionCommand = 1;
                    if (CurrentGame.Name == "Snake" && CurrentGameUserInput == Game.GameUserInput.Down)
                    {
                        return;
                    }
                    CurrentGameUserInput = Game.GameUserInput.Up;
                    break;
                case Game.GameUserInput.Left:
                    directionCommand = 2;
                    if (CurrentGame.Name == "Snake" && CurrentGameUserInput == Game.GameUserInput.Right)
                    {
                        return;
                    }
                    CurrentGameUserInput = Game.GameUserInput.Left;
                    break;
                case Game.GameUserInput.Down:
                    directionCommand = 3;
                    if (CurrentGame.Name == "Snake" && CurrentGameUserInput == Game.GameUserInput.Up)
                    {
                        return;
                    }
                    CurrentGameUserInput = Game.GameUserInput.Down;
                    break;
                case Game.GameUserInput.Action:
                    directionCommand = 4;
                    if (CurrentGame.Name == "Snake")
                    {
                        return;
                    }
                    CurrentGameUserInput = Game.GameUserInput.Action;
                    break;

            }
            Btc.SendCommand(Command.ArduinoCommand(CommandName.MoveDirection, new List<string>() { directionCommand.ToString() }));
        }

        public void StopModi()
        {
            Btc.SendCommand(Command.ArduinoCommand(CommandName.StopModi));
            ModiIsPlaying = false;
        }

        public async void ConnectToBluetooth(DeviceInformation device)
        {
            Debugger.ReportToDebugger(this, "Connecting to Device ...", Debugger.Device.Pc);
            await Btc.ConnectToDevice(device);
            Btc.SendCommand(Command.ArduinoCommand(CommandName.Connected));
        }

        public async void SearchDevices()
        {
            Btc.State = ConnectionState.Enumerating;
            await Btc.LookForBluetoothDevices();
            Btc.State = ConnectionState.Enumerated;
        }

        public void Disconnect()
        {
            if (ModiIsPlaying)
            {
                StopModi();
            }
            if (MusicIsPlaying)
            {
                StopMusic();
            }
            Btc.SendCommand(Command.ArduinoCommand(CommandName.Disconnect));
            Btc.Disconnect();
        }

        public void PlayMusic(Track track)
        {
            if (track != null)
            {
                MusicIsPlaying = true;
                Btc.SendCommand(Command.ArduinoCommand(CommandName.PlayMusic, new List<string>() { track.StorageName }));
            }
        }

        public void PauseMusic()
        {
            MusicIsPlaying = false;
            Btc.SendCommand(Command.ArduinoCommand(CommandName.PauseMusic));
        }

        public void StopMusic()
        {
            MusicIsPlaying = false;
            Btc.SendCommand(Command.ArduinoCommand(CommandName.StopMusic));
        }

        public enum NotifyType
        {
            StatusMessage,
            ErrorMessage
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged(String propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public void ReportFreeRam()
        {
            Btc.SendCommand(Command.ArduinoCommand(CommandName.FreeRam));
        }
    }
}

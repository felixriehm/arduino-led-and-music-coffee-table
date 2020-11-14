using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LightTable.Controller;
using LightTable.Model;

namespace LightTable.ViewModel
{
    public class GamesPageViewModel : INotifyPropertyChanged
    {
        public TableController TableController { get; set; }

        

        public GamesPageViewModel()
        {
            TableController = TableController.Instance;
        }

        public void GameInput(Game.GameUserInput direction)
        {
            TableController.GameUserInput(direction);
        }

        public void StartGame(Game g)
        {
            TableController.StartGame(g);
        }

        public void StopGame()
        {
            TableController.StopModi();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged(String propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}

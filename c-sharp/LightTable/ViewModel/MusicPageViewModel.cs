using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LightTable.Controller;
using LightTable.Model;

namespace LightTable.ViewModel
{
    public class MusicPageViewModel
    {
        public TableController TableController { get; set; }

        public MusicPageViewModel()
        {
            TableController = TableController.Instance;
        }

        public void PlayMusic(Track track)
        {
            TableController.PlayMusic(track);
        }

        public void StopMusic()
        {
            TableController.StopMusic();
        }

        public void PauseMusic()
        {
            TableController.PauseMusic();
        }
    }
}

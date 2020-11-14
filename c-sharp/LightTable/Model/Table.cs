using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml.Media;

namespace LightTable.Model
{
    public class Table
    {
        public ObservableCollection<Tile> Tiles { get; set; }
        public ObservableCollection<Track> Tracks { get; set; }
        public ObservableCollection<Modi> Modis { get; set; }
        public ObservableCollection<Game> Games { get; set; }

        public Table(int colums, int rows, Color color)
        {
            Tiles = new ObservableCollection<Tile>();
            for (int x = 0; x < rows; x++)
            {
                for (int y = 0; y < colums; y++)
                {
                    Tiles.Add(new Tile(y,x, color));
                }
            }

            Tracks = new ObservableCollection<Track>();
            Tracks.Add(new Track()
            {
                Title = "Get Lucky",
                StorageName = "gl.afm",
                Duration = new TimeSpan(0, 10, 0)
            });
            Tracks.Add(new Track()
            {
                Title = "What is Love",
                StorageName = "wil.afm",
                Duration = new TimeSpan(0, 4, 16)
            });
            Tracks.Add(new Track()
            {
                Title = "Cantina Theme",
                StorageName = "ct.afm",
                Duration = new TimeSpan(0, 2, 09)
            });

            Modis = new ObservableCollection<Modi>();
            Modis.Add(new Modi()
            {
                Name = "Random Hsv",
                CommandName = CommandName.RandomHsv
            });
            Modis.Add(new Modi()
            {
                Name = "Verlauf Hsv",
                CommandName = CommandName.TransitionHsv
            });
            Modis.Add(new Modi()
            {
                Name = "Pipes",
                CommandName = CommandName.Pipes
            });
            Modis.Add(new Modi()
            {
                Name = "Stripes",
                CommandName = CommandName.Stripes
            });

            Games = new ObservableCollection<Game>();
            Games.Add(new Game()
            {
                Name = "Snake",
                CommandName = CommandName.Snake
            });
            Games.Add(new Game()
            {
                Name = "Memory",
                CommandName = CommandName.Memory
            });
        }

        public void SetTile(int column, int row, Color color)
        {
            var tile = Tiles.FirstOrDefault(l => l.X == row && l.Y == column);
            tile.Color = color;

        }

        public void SetAllTiles(Color color)
        {
            foreach (var led in Tiles)
            {
                led.Color = color;
            }
        }
    }
}

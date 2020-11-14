using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using LightTable.Controller;
using LightTable.Controller.Utilities;
using LightTable.Model;

namespace LightTable.ViewModel
{
    public class ManuelControlViewModel
    {
        public ObservableCollection<SelectionColor> ColorSelection { get; set; }
        public Color SelectedColor = Color.FromArgb(255, 131, 181, 221);

        public TableController TableController { get; set; }

        public ManuelControlViewModel()
        {
            int r, g, b;
            TableController = TableController.Instance;

            ColorConverter.HsvToRgb(200, 1, 1, out r, out g, out b);
            SelectedColor = Color.FromArgb(255, (byte) r, (byte) g, (byte) b);

            ColorSelection = new ObservableCollection<SelectionColor>();
            ColorSelection.Add(new SelectionColor() { Color = Color.FromArgb(255, 0, 0, 0) });
            ColorSelection.Add(new SelectionColor() { Color = Color.FromArgb(255, 255, 255, 255) });
            ColorConverter.HsvToRgb(0, 1, 1, out r, out g, out b);
            ColorSelection.Add(new SelectionColor() { Color = Color.FromArgb(255, (byte)r, (byte)g, (byte)b) });
            ColorConverter.HsvToRgb(36, 1, 1, out r, out g, out b);
            ColorSelection.Add(new SelectionColor() { Color = Color.FromArgb(255, (byte)r, (byte)g, (byte)b) });
            ColorConverter.HsvToRgb(60, 1, 1, out r, out g, out b);
            ColorSelection.Add(new SelectionColor() { Color = Color.FromArgb(255, (byte)r, (byte)g, (byte)b) });
            ColorConverter.HsvToRgb(80, 1, 1, out r, out g, out b);
            ColorSelection.Add(new SelectionColor() { Color = Color.FromArgb(255, (byte)r, (byte)g, (byte)b) });
            ColorConverter.HsvToRgb(116, 1, 1, out r, out g, out b);
            ColorSelection.Add(new SelectionColor() { Color = Color.FromArgb(255, (byte)r, (byte)g, (byte)b) });
            ColorConverter.HsvToRgb(178, 1, 1, out r, out g, out b);
            ColorSelection.Add(new SelectionColor() { Color = Color.FromArgb(255, (byte)r, (byte)g, (byte)b) });
            ColorConverter.HsvToRgb(200, 1, 1, out r, out g, out b);
            ColorSelection.Add(new SelectionColor() { Color = Color.FromArgb(255, (byte)r, (byte)g, (byte)b) });
            ColorConverter.HsvToRgb(240, 1, 1, out r, out g, out b);
            ColorSelection.Add(new SelectionColor() { Color = Color.FromArgb(255, (byte)r, (byte)g, (byte)b) });
            ColorConverter.HsvToRgb(280, 1, 1, out r, out g, out b);
            ColorSelection.Add(new SelectionColor() { Color = Color.FromArgb(255, (byte)r, (byte)g, (byte)b) });
            ColorConverter.HsvToRgb(300, 1, 1, out r, out g, out b);
            ColorSelection.Add(new SelectionColor() { Color = Color.FromArgb(255, (byte)r, (byte)g, (byte)b) });
        }

        public class SelectionColor
        {
            public Color Color { get; set; }
        }

        public void allOn_Tapped()
        {
            TableController.SetAllTiles(Colors.White);
        }

        public void allOff_Tapped()
        {
            TableController.SetAllTiles(Colors.Black);
        }
    }
}

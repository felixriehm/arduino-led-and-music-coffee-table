using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightTable.Model
{
    public class Game
    {
        public string Name { get; set; }
        public CommandName CommandName { get; set; }

        public enum GameUserInput
        {
            Up,
            Left,
            Down,
            Right,
            Action
        }
    }
}

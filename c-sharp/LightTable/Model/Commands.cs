using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightTable.Model
{
    public enum CommandName
    {
        SetTile = 1,
        SetAllPixel = 2,
        MoveDirection = 3,
        TransitionHsv = 4,
        Snake = 0,
        PlayMusic = 5,
        StopMusic = 6,
        PauseMusic = 7,
        StopModi = 8,
        RandomHsv = 9,
        Disconnect = 11,
        Test = 99,
        Connected = 10,
        FreeRam = 13,
        Pipes = 14,
        Stripes = 15,
        Memory = 12
    }

    public static class Command
    {
        public static string ArduinoCommand(CommandName commandName, List<string> commandParameters)
        {
            StringBuilder parameters = new StringBuilder();
            foreach (string parameter in commandParameters)
            {
                parameters.Append(" " + parameter);
            }
            return ((int)commandName).ToString() + parameters.ToString() + "\r";
        }

        public static string ArduinoCommand(CommandName commandName)
        {
            return ((int)commandName).ToString() + "\r";
        }
    }
}

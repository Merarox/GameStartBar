using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameStartBar
{
    public class SettingsObject
    {
        public string GameFolderPath { get; set; }

        public List<Commands> Commands = new List<Commands>();
        public Position Position = new Position();
        public double FontSize { get; set; }

        public SettingsObject()
        {

        }

        public SettingsObject(string gamefolderpath, List<Commands> commandList, Position position, double fontsize)
        {
            Log.writeLog();
            this.GameFolderPath = gamefolderpath;
            this.Commands = commandList;
            this.Position = position;
            this.FontSize = fontsize;
        }
    }

    public class Commands
    {
        public string CommandName { get; set; }

        public string Command { get; set; }
        public string Argument { get; set; }

        public Commands(string commandName, string command, string argument)
        {
            Log.writeLog();
            CommandName = commandName;
            Command = command;
            Argument = argument;
        }
    }

    public class Position
    {
        public double PositionX { get; set; }
        public double PositionY { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }

        public Position()
        {
            Log.writeLog();
            PositionX = 0;
            PositionY = 0;
            Width = 0;
            Height = 0;
        }
    }
}

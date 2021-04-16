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
        public uint VK;
        public uint MOD;

        public SettingsObject()
        {

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
        public double Top { get; set; }
        public double Left { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }

        public Position()
        {
            Log.writeLog();
            this.Top = 0;
            this.Left = 0;
            this.Width = 0;
            this.Height = 0;
        }

        public Position(double top, double left, double width, double height)
        {
            Log.writeLog();
            this.Top = left;
            this.Left = top;
            this.Width = width;
            Console.WriteLine(Width);
            this.Height = height;
            Console.WriteLine(Height);
        }
    }
}

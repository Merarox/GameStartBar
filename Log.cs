using System;
using System.IO;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

namespace GameStartBar
{
    public static class Log
    {
        private const string LogPath = "Log.txt";

        private static DateTime now = DateTime.Now;

        public static void writeLog([CallerMemberName] string name = null, [CallerFilePath] string path = null)
        {
            string time = $"[{now.Hour}:{now.Minute}:{now.Second}]";
            writeToFile($"{time} {path} | {name}");

        }

        private static void writeToFile(string message)
        {
            if (File.Exists(LogPath))
            {
                using(StreamWriter sw = File.AppendText(LogPath))
                {
                    sw.WriteLine(message);
                }
            }
            else
            {
                using (StreamWriter sw = File.CreateText(LogPath))
                {
                    sw.WriteLine(message);
                }
            }
        }
    }
}

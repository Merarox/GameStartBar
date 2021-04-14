using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace GameStartBar
{
    public class Settings
    {
        //Path for the Settings File
        public string SettingsPath { get; private set; }
        public SettingsObject settingsObject = new SettingsObject();

        public Settings()
        {
            Log.writeLog();
            SettingsPath = "Settings.json";
        }

        //Reads the Settings JSON File
        public void readSettings()
        {
            Log.writeLog();
            //Checks if the JSON File exist or not
            if (File.Exists(SettingsPath))
            {
                //Deserialize the JSON File
                settingsObject = DeserializeObject();
                //Checks if the File have the GameFolderPath
                if (settingsObject.GameFolderPath == null)
                {
                    //Select the Game Folder Path
                    SelectGameFolderPath();
                }
                if(settingsObject.Position == null)
                {
                    settingsObject.Position = new Position();
                }
            }
            else
            {
                //Create File and select the Game Folder Path
                SelectGameFolderPath();

                //Set Default Values
                AddCommand("Settings", "Settings", "");
                AddCommand("Exit", "Exit", "");
                SetFontSize(48);
            }
            
        }

        //Write the Settings JSON Fileasd
        public void writeSettings()
        {
            Log.writeLog();
            //If the File exist the Object will be Serilized if not, the File will be createt
            if (File.Exists(SettingsPath))
            {
                SerializeObject();
            }
            else
            {
                File.Create(SettingsPath).Close();
                writeSettings();
            }
        }

        //Serialize the SettingsObject
        public void SerializeObject()
        {
            Log.writeLog();
            string output = JsonConvert.SerializeObject(settingsObject);
            File.WriteAllText(SettingsPath, output);
        }

        //Deserialize the SettingsObject
        public SettingsObject DeserializeObject()
        {
            Log.writeLog();
            string input = File.ReadAllText(SettingsPath);
            return JsonConvert.DeserializeObject<SettingsObject>(input);
        }

        //Method to select the Game Folder Path
        public void SelectGameFolderPath()
        {
            Log.writeLog();
            FolderBrowserDialog folderBrowser = new FolderBrowserDialog();
            folderBrowser.Description = "Select your Game Folder";
            DialogResult dialogResult = folderBrowser.ShowDialog();
            if (dialogResult == DialogResult.OK)
            {
                settingsObject.GameFolderPath = folderBrowser.SelectedPath;
            }
            else
            {
                //The Application will be closed if none Folder was selected
                System.Environment.Exit(1);
            }
        }
        public string GetGameFolderPath()
        {
            Log.writeLog();
            return settingsObject.GameFolderPath;
        }

        public void SetGameFolderPath(string path)
        {
            Log.writeLog();
            settingsObject.GameFolderPath = path;
        }

        public void AddCommand(string commandname, string command, string argument)
        {
            Log.writeLog();
            settingsObject.Commands.Add(new Commands(commandname, command, argument));
        }

        public void RemoveCommand(int position)
        {
            Log.writeLog();
            settingsObject.Commands.RemoveAt(position);
        }

        public void SetCommands(List<Commands> list)
        {
            Log.writeLog();
            settingsObject.Commands = list;
        }

        public List<Commands> GetCommands()
        {
            Log.writeLog();
            return settingsObject.Commands;
        }

        public Position GetPosition()
        {
            Log.writeLog();
            return settingsObject.Position;
        }

        public void SetXPosition(double value)
        {
            Log.writeLog();
            settingsObject.Position.PositionX = value;
        }

        public void SetYPosition(double value)
        {
            Log.writeLog();
            settingsObject.Position.PositionY = value;
        }

        public void SetWidth(double value)
        {
            Log.writeLog();
            settingsObject.Position.Width = value;
        }

        public void SetHeight(double value)
        {
            Log.writeLog();
            settingsObject.Position.Height = value;
        }

        public void SetFontSize(double value)
        {
            Log.writeLog();
            settingsObject.FontSize = value;
        }

        public int GetFontSize()
        {
            Log.writeLog();
            return (int)settingsObject.FontSize;
        }
    }
}

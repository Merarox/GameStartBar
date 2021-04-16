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
            //Default Values
            SettingsPath = "Settings.json";
            AddCommand("Settings", "Settings", "");
            AddCommand("Exit", "Exit", "");
            AddCommand("Update", "Update", "");
            SetFontSize(48);
            //C key
            SetVK(67);
            //Mod Alt
            SetMOD(0x0001);
        }

        public Settings(string gamefolderpath, List<Commands> commandList, Position position, double fontsize, uint vk, uint mod)
        {
            Log.writeLog();
            SettingsPath = "Settings.json";
            SetGameFolderPath(gamefolderpath);
            SetCommands(commandList);
            SetTopPosition(position.Top);
            SetLeftPosition(position.Left);
            SetFontSize(fontsize);
            SetVK(vk);
            SetMOD(mod);
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
                if(GetGameFolderPath() == "" || GetGameFolderPath() == null)
                {
                    System.Environment.Exit(1);
                }
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

        public void SetTopPosition(double value)
        {
            Log.writeLog();
            settingsObject.Position.Top = value;
        }

        public void SetLeftPosition(double value)
        {
            Log.writeLog();
            settingsObject.Position.Left = value;
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

        public uint GetVK()
        {
            return settingsObject.VK;
        }

        public void SetVK(uint key)
        {
            settingsObject.VK = key;
        }

        public uint GetMOD()
        {
            return settingsObject.MOD;
        }

        public void SetMOD(uint mod)
        {
            settingsObject.MOD = mod;
        }
    }
}

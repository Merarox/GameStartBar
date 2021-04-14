using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows;
using System.ComponentModel;
using System.Data;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Collections.ObjectModel;

namespace GameStartBar
{
    //GameFolderPath
    public partial class SettingWindow : Window, INotifyPropertyChanged
    {
        private Settings settings;
        public List<Commands> CommandList { get; set; }

        /// <summary>
        /// Properties for binding
        /// </summary>
        private int _currentIndx;
        public int CurrentIndex
        {
            get { return _currentIndx; }
            set
            {
                _currentIndx = value;
                if (CurrentIndex >= 0)
                {
                    CommandNameItem = CommandList[_currentIndx].CommandName;
                    CommandItem = CommandList[_currentIndx].Command;
                    CommandListView.Items.Refresh();
                }
                else
                {
                    CommandNameItem = "";
                    CommandItem = "";
                }
            }
        }

        private string _pathText;
        public string PathText
        {
            get { return _pathText; }
            set
            {
                if(_pathText != value)
                {
                    _pathText = value;
                    OnPropertyChanged();
                    if (Directory.Exists(_pathText)) {
                        settings.SetGameFolderPath(_pathText);
                    }
                }
            }
        }

        private string _commandNameItem;
        public string CommandNameItem
        {
            get { return _commandNameItem; }
            set
            {
                if (_commandNameItem != value)
                {
                    try
                    {
                        _commandNameItem = value;
                        CommandList[CurrentIndex].CommandName = _commandNameItem;
                        CommandListView.Items.Refresh();
                        OnPropertyChanged();
                    }
                    catch(ArgumentOutOfRangeException e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
            }
        }

        private string _commandItem;
        public string CommandItem
        {
            get { return _commandItem; }
            set
            {
                if (_commandItem != value)
                {
                    try
                    {
                        _commandItem = value;
                        CommandList[CurrentIndex].Command = _commandItem;
                        CommandListView.Items.Refresh();
                        OnPropertyChanged();
                    }
                    catch(ArgumentOutOfRangeException e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
            }
        }

        private string _argumentItem;
        public string ArgumentItem
        {
            get { return _argumentItem; }
            set
            {
                if (_argumentItem != value)
                {
                    try
                    {
                        _argumentItem = value;
                        CommandList[CurrentIndex].Argument = _argumentItem;
                        CommandListView.Items.Refresh();
                        OnPropertyChanged();
                    }
                    catch (ArgumentOutOfRangeException e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
            }
        }

        private int _currentFontSize;
        public int CurrentFontSize
        {
            get { return _currentFontSize; }
            set
            {
                if(_currentFontSize != value)
                {
                    _currentFontSize = value;
                    FontSizeSliderChangedEvent.FontSizeValue = _currentFontSize;
                    OnFontSizeSliderChanged(FontSizeSliderChangedEvent);
                }
            }
        }

        public WindowClosingEventArgs WindowClosingEvent = new WindowClosingEventArgs();
        public EditButtonPressedEventArgs EditButtonPressedEvent = new EditButtonPressedEventArgs();
        public FontSizeSliderChangedEventArgs FontSizeSliderChangedEvent = new FontSizeSliderChangedEventArgs();

        public SettingWindow(Settings settings)
        {
            Log.writeLog();
            this.DataContext = this;
            InitializeValues(settings);
            InitializeComponent();
        }

        private void InitializeValues(Settings settings)
        {
            Log.writeLog();
            CommandList = new List<Commands>();
            this.settings = settings;
            EditButtonPressedEvent.Editable = true;
            CurrentFontSize = settings.GetFontSize();
            LoadSettings();
        }

        //Load the Settings
        private void LoadSettings()
        {
            Log.writeLog();
            PathText = settings.GetGameFolderPath();
            CommandList = new List<Commands>(settings.GetCommands());
        }

        //Event for the GameFolderSelection
        private void GameFolderTextBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Log.writeLog();
            settings.SelectGameFolderPath();
            PathText = settings.GetGameFolderPath();
        }

        //Event to prevent, that the Window will be closed
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            Log.writeLog();
            e.Cancel = true;
            settings.SetCommands(CommandList);
            settings.SetGameFolderPath(PathText);
            WindowClosingEvent.SettingsObject = settings;
            this.EditButtonPressedEvent.Editable = true;
            OnWindowClosing(WindowClosingEvent);
            this.Visibility = Visibility.Hidden;
        }

        //Adds a new Line in the Command ListBox
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            Log.writeLog();
            CommandList.Add(new Commands("-", "-", ""));
            CurrentIndex = CommandList.Count()-1;
            CommandListView.Items.Refresh();
        }

        //Remove a Line in the Command ListBox
        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            Log.writeLog();
            CommandNameItem = "";
            CommandItem = "";
            ArgumentItem = "";
            try
            {
                CommandList.RemoveAt(CurrentIndex);
                CurrentIndex = -1;
                CommandListView.Items.Refresh();
            }
            catch(ArgumentOutOfRangeException ae)
            {
                Console.WriteLine(ae.Message);
            }
        }

        //Event that triggers if a new Items is selected in the ListBox
        private void ListViewItem_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Log.writeLog();
            CurrentIndex = CommandList.IndexOf(((ListView)sender).SelectedItem as Commands);
            CommandNameItem = CommandList[CurrentIndex].CommandName;
            CommandItem = CommandList[CurrentIndex].Command;
            ArgumentItem = CommandList[CurrentIndex].Argument;
        }

        //Button to change the Size and Position of the Main Window
        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            Log.writeLog();
            if (EditButtonPressedEvent.Editable == true)
            {
                OnEditButtonPressed(EditButtonPressedEvent);
                EditButtonPressedEvent.Editable = false;
                EditButton.Content = "Cancel";
            }
            else
            {
                OnEditButtonPressed(EditButtonPressedEvent);
                EditButtonPressedEvent.Editable = true;
                EditButton.Content = "Edit";
            }
        }

        /// <summary>
        /// First Method will be triggered if a Property is Changed
        /// The other three Methods trigger a Event in the Main Window
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            Log.writeLog();
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public EventHandler<WindowClosingEventArgs> WindowClosingHandler;
        protected void OnWindowClosing(WindowClosingEventArgs e)
        {
            Log.writeLog();
            EventHandler<WindowClosingEventArgs> handler = WindowClosingHandler;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public EventHandler<FontSizeSliderChangedEventArgs> FontSizeSliderChangedHandler;
        protected void OnFontSizeSliderChanged(FontSizeSliderChangedEventArgs e)
        {
            Log.writeLog();
            EventHandler<FontSizeSliderChangedEventArgs> handler = FontSizeSliderChangedHandler;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public EventHandler<EditButtonPressedEventArgs> EditButtonHandler;
        protected void OnEditButtonPressed(EditButtonPressedEventArgs e)
        {
            Log.writeLog();
            EventHandler<EditButtonPressedEventArgs> handler = EditButtonHandler;
            if (handler != null)
            {
                handler(this, e);
            }
        }
    }
}

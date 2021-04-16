using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace GameStartBar
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        //Structure for the Commands
        private struct CommandandGame
        {
            public string Name { get; set; }
            public string Command { get; set; }
            public string Argument { get; set; }

            public CommandandGame(string name, string command, string argument)
            {
                this.Name = name;
                this.Command = command;
                this.Argument = argument;
            }
        }

        //Structure for the Original Size that will be used for Resizing
        public struct FormObject
        {
            public double top;
            public double bottom;
            public double width;
            public double height;

            public FormObject(double top, double bottom, double width, double height)
            {
                this.top = top;
                this.bottom = bottom;
                this.width = width;
                this.height = height;
            }
           
            public void printObject()
            {
                Console.WriteLine("top: " + top);
                Console.WriteLine("bottom: " + bottom);
                Console.WriteLine("width: " + width);
                Console.WriteLine("height: " + height);
            }
        }

        private SettingWindow settingswindow;
        private Settings settings = new Settings();
        //Game List
        private string[] gamelist;
        //Command List
        private List<CommandandGame> CGList = new List<CommandandGame>();
        //Both List combined
        private List<CommandandGame> ResultCGList = new List<CommandandGame>();
        //A List with all TextBox Elements
        private List<TextBox> TextBoxList = new List<TextBox>();

        private int TextBoxIndex = 0;
        private int ResultIndex = 0;
        private bool editable = false;

        //The Main Input is binded to this Property
        private string _inputText;
        public string InputText
        {
            get { return _inputText; }
            set
            {
                if (_inputText != value)
                {
                    _inputText = value;
                    ResetBorder();
                    SearchResult();
                    TextBoxIndex = 0;
                    OnPropertyChanged();
                }
            }
        }

        public MainWindow()
        {
            Log.writeLog();
            this.DataContext = this;
            settings.readSettings();

            InitializeComponent();

            InitializeValues();
            InitializeEvents();
        }

        //Initialize the start values
        private void InitializeValues()
        {
            Log.writeLog();
            settingswindow = new SettingWindow(settings);

            SetOriginalSize();

            if (settings.GetPosition().Top != 0 && settings.GetPosition().Left != 0 && settings.GetPosition().Width != 0 && settings.GetPosition().Height != 0)
            {
                this.Top = settings.GetPosition().Top;
                this.Left = settings.GetPosition().Left;
                this.Width = settings.GetPosition().Width;
                this.Height = settings.GetPosition().Height;
            }

            TextBoxList.Add(gameTB1);
            TextBoxList.Add(gameTB2);
            TextBoxList.Add(gameTB3);

            LoadFolder();
            CreateCGList();
            SetFontSize(settings.GetFontSize());
        }

        //Initialize the Events
        private void InitializeEvents()
        {
            Log.writeLog();
            this.Activated += new EventHandler(GameTB_GotFocus);
            this.Deactivated += new EventHandler(GameTB_LostFocus);
            InputTB.GotFocus += new RoutedEventHandler(GameTB_GotFocus);
            InputTB.LostFocus += new RoutedEventHandler(GameTB_LostFocus);

            settingswindow.WindowClosingHandler += SettingsWindow_OnWindowClosing;
            settingswindow.EditButtonHandler += SettingsWindow_OnEditButtonPressed;
            settingswindow.FontSizeSliderChangedHandler += SettingsWindow_OnFontSizeSliderChanged;
        }

        /// <summary>
        /// This Methods and Variables that handles the Hotkey
        /// </summary>
        
        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, UInt32 fsModifiers, UInt32 vlc);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        private HwndSource _source;
        private const int HOTKEY_ID = 9000;

        protected override void OnSourceInitialized(EventArgs e)
        {
            Log.writeLog();
            base.OnSourceInitialized(e);
            var helper = new WindowInteropHelper(this);
            _source = HwndSource.FromHwnd(helper.Handle);
            _source.AddHook(HwndHook);
            RegisterHotKey();
        }

        private void RegisterHotKey()
        {
            Log.writeLog();
            var helper = new WindowInteropHelper(this);
            if (!RegisterHotKey(helper.Handle, HOTKEY_ID, settings.GetMOD(), settings.GetVK()))
            {
                MessageBox.Show("Couldn't register hotkey, closing application.");
                Application.Current.Shutdown();
            }
        }
        
        private void UnregisterHotKey()
        {
            Log.writeLog();
            var helper = new WindowInteropHelper(this);
            UnregisterHotKey(helper.Handle, HOTKEY_ID);
        }

        private IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            //Remove the Log for this Method because its trigger to often
            //Log.writeLog();
            const int WM_HOTKEY = 0x0312;
            switch (msg)
            {
                case WM_HOTKEY:
                    switch (wParam.ToInt32())
                    {
                        case HOTKEY_ID:
                            OnHotKeyPressed();
                            handled = true;
                            break;
                    }
                    break;
            }
            return IntPtr.Zero;
        }

        private void OnHotKeyPressed()
        {
            Log.writeLog();
            if (this.Visibility == Visibility.Hidden)
            {
                this.Show();
                this.Activate();
            }
            else
            {
                this.Hide();
            }
        }

        /// <summary>
        /// Three Events that the Settings Window trigger
        /// PropertyChangedEventHandler will be triggered if a Property is changed. This Event is Importend for Binding
        /// </summary>
        private void SettingsWindow_OnWindowClosing(object sender, WindowClosingEventArgs e)
        {
            settings = e.SettingsObject;
            settings.SetTopPosition(this.Top);
            settings.SetLeftPosition(this.Left); 
            settings.SetWidth(this.Width);
            settings.SetHeight(this.Height);
            settings.SetFontSize(InputTB.FontSize);
            
            //Unregister the old HotKey
            UnregisterHotKey();
            //Register the new HotKey
            RegisterHotKey();

            settings.writeSettings();

            //Load the new Folder and Create the Game List for the new Folder
            LoadFolder();
            CreateCGList();
            
            //if the Editing was not canceled
            if(editable == true)
            {
                this.Visibility = Visibility.Hidden;
                foreach (TextBox box in TextBoxList)
                {
                    box.Visibility = Visibility.Hidden;
                }
                InputTB.IsReadOnly = false;
                InputTB.Focusable = true;
                this.ResizeMode = ResizeMode.NoResize;
            }

            //Reset the Textbox
            InputText = "";
        }

        private void SettingsWindow_OnEditButtonPressed(object sender, EditButtonPressedEventArgs e)
        {
            editable = e.Editable;
            InputText = "Example";
            if(e.Editable == true)
            {
                this.Visibility = Visibility.Visible;
                foreach (TextBox box in TextBoxList)
                {
                    box.Text = "";
                    box.Visibility = Visibility.Visible;
                }
                InputTB.IsReadOnly = true;
                InputTB.Focusable = false;
                this.ResizeMode = ResizeMode.CanResizeWithGrip;
            }
            else
            {
                this.Visibility = Visibility.Hidden;
                foreach (TextBox box in TextBoxList)
                {
                    box.Visibility = Visibility.Hidden;
                }
                InputTB.IsReadOnly = false;
                InputTB.Focusable = true;
                this.ResizeMode = ResizeMode.NoResize;
            }
        }

        private void SettingsWindow_OnFontSizeSliderChanged(object sender, FontSizeSliderChangedEventArgs e)
        {
            SetFontSize(e.FontSizeValue);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        /// <summary>
        /// First Method Change the Font Size for all TextBoxes
        /// The Second Method Load the GameFolderPath from the Settings and check if the GameFolderPath is a Folder
        /// The Third Method creates a List with all Games
        /// </summary>
        private void SetFontSize(int value)
        {
            Log.writeLog();
            InputTB.FontSize = value;
            foreach (TextBox t in TextBoxList)
                t.FontSize = value;
        }
        public void LoadFolder()
        {
            Log.writeLog();
            if (Directory.Exists(settings.GetGameFolderPath()))
            {
                try
                {
                    gamelist = Directory.GetFiles(settings.GetGameFolderPath());
                }
                catch (IOException e)
                {
                    MessageBox.Show("Files not found!");
                    Console.WriteLine(e);
                    settings.SelectGameFolderPath();
                    settings.writeSettings();
                    LoadFolder();
                }
            }
            else
            {
                MessageBox.Show("Directory doesn't Exist!");
                settings.SelectGameFolderPath();
                settings.writeSettings();
                LoadFolder();
            }
        }
        private void CreateCGList()
        {
            Log.writeLog();
            CGList = new List<CommandandGame>();
            foreach(string s in gamelist)
            {
                int indexbackslash = s.LastIndexOf(@"\") + 1;
                int wordlength = s.IndexOf(".") - indexbackslash;
                CGList.Add(new CommandandGame(s.Substring(indexbackslash, wordlength), s, ""));
            }

            foreach(Commands c in settings.GetCommands())
            {
                CGList.Add(new CommandandGame(c.CommandName, c.Command, c.Argument));
            }
        }

        /// <summary>
        ///  Two Methods when the Main Window get Focus and lose Focus
        /// </summary>
        public void GameTB_GotFocus(object sender, EventArgs e)
        {
            Log.writeLog();
            Keyboard.Focus(InputTB);
        }

        public void GameTB_LostFocus(object sender, EventArgs e)
        {
            Log.writeLog();
            if(editable == false)
            {
                InputText = "";
                this.Hide();
            }
        }

        /// <summary>
        /// The First Method is a event that Handles the the Key Input in the Main TextBox
        /// The Second Method handles the Visiblity and Search Results
        /// </summary>
        private void gameTB_KeyDown(object sender, KeyEventArgs e)
        {
            Log.writeLog();
            if(e.Key == Key.Return && ResultCGList.Count >= 1)
            {
                if (TextBoxIndex == 0)
                {
                    if (ResultCGList[0].Command.ToLower() == "Settings".ToLower())
                    {
                        settingswindow.Visibility = Visibility.Visible;
                    }
                    else if (ResultCGList[0].Command.ToLower() == "Exit".ToLower())
                    {
                        Application.Current.Shutdown();
                    }
                    else if (ResultCGList[0].Command.ToLower() == "Update".ToLower())
                    {
                        LoadFolder();
                        CreateCGList();
                        InputText = "";
                    }
                    else
                    {
                        if (ResultCGList[0].Argument != "")
                        {
                            ProcessStartInfo processStartInfo = new ProcessStartInfo();
                            processStartInfo.FileName = ResultCGList[0].Command;
                            processStartInfo.Arguments = "/c " + ResultCGList[0].Argument;
                            Process.Start(processStartInfo);
                            this.Visibility = Visibility.Hidden;
                        }
                        else
                        {
                            Process.Start(ResultCGList[0].Command);
                            this.Visibility = Visibility.Hidden;
                        }
                    }
                }
                else
                {
                    if (ResultCGList[ResultIndex - 1].Command.ToLower() == "Settings".ToLower())
                    {
                        settingswindow.Visibility = Visibility.Visible;
                    }
                    else if (ResultCGList[ResultIndex - 1].Command.ToLower() == "Exit".ToLower())
                    {
                        Application.Current.Shutdown();
                    }
                    else if (ResultCGList[ResultIndex - 1].Command.ToLower() == "Update".ToLower())
                    {
                        LoadFolder();
                        CreateCGList();
                        InputText = "";
                    }
                    else
                    {
                        Process.Start(ResultCGList[ResultIndex - 1].Command);
                    }
                }
                ResultIndex = 0;
                TextBoxIndex = 0;
            }
            if(e.Key == Key.Tab)
            {
                if(TextBoxIndex <= ResultCGList.Count && ResultCGList != null)
                {
                    if(TextBoxIndex == 3 && TextBoxIndex < ResultCGList.Count)
                    {
                        TextBoxList[0].Text = TextBoxList[1].Text;
                        TextBoxList[1].Text = TextBoxList[2].Text;
                        TextBoxList[2].Text = ResultCGList[ResultIndex++].Name;

                        if (ResultIndex == ResultCGList.Count)
                            ResultIndex = 0;
                    }
                    else if (TextBoxIndex == ResultCGList.Count || TextBoxIndex == 3)
                    {
                        TextBoxIndex = 0;
                        ResultIndex = 0;
                        DrawBorder(TextBoxList[TextBoxIndex++]);
                        ResultIndex++;
                    }
                    else
                    {
                        DrawBorder(TextBoxList[TextBoxIndex++]);
                        ResultIndex++;
                    }                 
                }
            }
            if(e.Key == Key.Escape)
            {
                this.GameTB_LostFocus(sender, e);
            }
        }

        private void SearchResult()
        {
            ResultCGList = new List<CommandandGame>();

            if(InputText != "")
            {
                foreach (CommandandGame c in CGList)
                {
                    if (c.Name.ToLower().Contains(InputText.ToLower()))
                    {
                        ResultCGList.Add(c);
                    }
                }

                foreach (TextBox t in TextBoxList)
                {
                    t.Text = "";
                }

                if (ResultCGList.Count >= 3)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        TextBoxList[i].Text = ResultCGList[i].Name;
                        TextBoxList[i].Visibility = Visibility.Visible;
                    }
                }
                else
                {
                    for (int i = 0; i < ResultCGList.Count; i++)
                    {
                        TextBoxList[i].Text = ResultCGList[i].Name;
                        TextBoxList[i].Visibility = Visibility.Visible;
                    }

                    for (int i = 0; i < 3 - ResultCGList.Count; i++)
                    {
                        TextBoxList[i + ResultCGList.Count].Visibility = Visibility.Hidden;
                    }
                }
            }
            else
            {
                foreach (TextBox t in TextBoxList)
                {
                    ResultCGList = new List<CommandandGame>();
                    t.Visibility = Visibility.Hidden;
                }
            }
        }

        /// <summary>
        /// Two Methods to draw and remove the Border around the TextBox Elements
        /// </summary>

        private void DrawBorder(TextBox t)
        {
            Log.writeLog();
            foreach(TextBox box in TextBoxList)
            {
                box.BorderThickness = new Thickness(1, 1, 1, 1);
                box.BorderBrush = System.Windows.Media.Brushes.LightGray;
            }
            t.BorderThickness = new Thickness(5, 5, 5, 5);
            t.BorderBrush = System.Windows.Media.Brushes.Gray;
        }

        private void ResetBorder()
        {
            Log.writeLog();
            foreach (TextBox box in TextBoxList)
            {
                box.BorderThickness = new Thickness(1, 1, 1, 1);
                box.BorderBrush = System.Windows.Media.Brushes.LightGray;
            }
        }

        //If the Left Mouse Button is Pressed, the Window can be moved around
        private void main_Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Log.writeLog();
            if(e.LeftButton == MouseButtonState.Pressed && editable == true)
            {
                this.DragMove();
            }
        }


        /// <summary>
        /// Methods to Resize all TextBoxes
        /// The Methods are needed to resize the margin and the width and height
        /// </summary>

        private Size formOriginalSize;
        private FormObject InputTBOriginalSize;
        private FormObject TB1OriginalSize;
        private FormObject TB2OriginalSize;
        private FormObject TB3OriginalSize;
        private void SetOriginalSize()
        {
            Log.writeLog();
            formOriginalSize.Width = this.Width;
            formOriginalSize.Height = this.Height;

            InputTBOriginalSize = new FormObject(InputTB.Margin.Top, InputTB.Margin.Bottom, InputTB.Width, InputTB.Height);
            TB1OriginalSize = new FormObject(gameTB1.Margin.Top, gameTB1.Margin.Bottom, gameTB1.Width, gameTB1.Height);
            TB2OriginalSize = new FormObject(gameTB2.Margin.Top, gameTB2.Margin.Bottom, gameTB2.Width, gameTB2.Height);
            TB3OriginalSize = new FormObject(gameTB3.Margin.Top, gameTB3.Margin.Bottom, gameTB3.Width, gameTB3.Height);
        }

        //If the Form Size is changed, every Object will be resized
        private void main_Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Log.writeLog();
            ResizeAllObjectsSize();
        }

        //Use the Resize Method on all Elements
        private void ResizeAllObjectsSize()
        {
            Log.writeLog();
            ResizeController(InputTB, InputTBOriginalSize);
            ResizeController(gameTB1, TB1OriginalSize);
            ResizeController(gameTB2, TB2OriginalSize);
            ResizeController(gameTB3, TB3OriginalSize);
        }

        //Resize Method
        private void ResizeController(System.Windows.Controls.Control control, FormObject originalControl)
        {
            Log.writeLog();
            double xRatio = this.Width / formOriginalSize.Width;
            double yRatio = this.Height / formOriginalSize.Height;

            int heightdif = (int)(this.Height / 4);

            int newTop = (int) ((originalControl.top / (formOriginalSize.Height / 4)) * heightdif)-1;
            int newBottom = (int)((originalControl.bottom / (formOriginalSize.Height / 4)) * heightdif)-1;

            int newWidth = (int)(originalControl.width * xRatio);
            int newHeight = (int)(originalControl.height * yRatio);

            control.Margin = new Thickness(0, newTop, 0, newBottom);
            control.Width = newWidth;
            control.Height = newHeight;
        }
    }
}
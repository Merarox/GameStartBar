using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameStartBar
{
    public class WindowClosingEventArgs : EventArgs
    {
        public Settings SettingsObject { get; set; }
    }
}

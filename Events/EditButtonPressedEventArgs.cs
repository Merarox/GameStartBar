using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameStartBar
{
    public class EditButtonPressedEventArgs : EventArgs
    {
        public bool Editable { get; set; }
    }
}

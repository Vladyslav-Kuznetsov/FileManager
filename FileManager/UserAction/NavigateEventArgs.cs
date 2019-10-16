using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManager.UserAction
{
    public class NavigateEventArgs : EventArgs
    {
        public NavigateType Type { get; set; }
    }
}

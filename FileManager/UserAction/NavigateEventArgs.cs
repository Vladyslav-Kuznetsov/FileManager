using System;

namespace FileManager.UserAction
{
    public class NavigateEventArgs : EventArgs
    {
        public NavigateType Type { get; set; }
    }
}

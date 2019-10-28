using System;

namespace FileManager.UserAction
{
    public class OperationEventArgs : EventArgs
    {
        public OperationType Type { get; set; }
    }
}

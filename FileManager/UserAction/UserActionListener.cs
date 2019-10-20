using System;

namespace FileManager.UserAction
{
    public class UserActionListener
    {
        public event EventHandler<NavigateEventArgs> Navigated;
        public event EventHandler<OperationEventArgs> FileServiceOperation;
        public event Action TabSwitching;
        public event Action PropertyRequest;

        public void ReadInput()
        {
            ConsoleKey command = Console.ReadKey(true).Key;

            switch (command)
            {
                case ConsoleKey.UpArrow:
                    Navigated?.Invoke(this, new NavigateEventArgs() { Type = NavigateType.Up });
                    break;
                case ConsoleKey.DownArrow:
                    Navigated?.Invoke(this, new NavigateEventArgs() { Type = NavigateType.Down });
                    break;
                case ConsoleKey.Enter:
                    Navigated?.Invoke(this, new NavigateEventArgs() { Type = NavigateType.Enter });
                    break;
                case ConsoleKey.Backspace:
                    Navigated?.Invoke(this, new NavigateEventArgs() { Type = NavigateType.Back });
                    break;
                case ConsoleKey.Tab:
                    TabSwitching?.Invoke();
                    break;
                case ConsoleKey.F1:
                    FileServiceOperation?.Invoke(this, new OperationEventArgs() { Type = OperationType.Copy });
                    break;
                case ConsoleKey.F2:
                    FileServiceOperation?.Invoke(this, new OperationEventArgs() { Type = OperationType.Move });
                    break;
                case ConsoleKey.F6:
                    PropertyRequest?.Invoke();
                    break;
                case ConsoleKey.F7:
                    FileServiceOperation?.Invoke(this, new OperationEventArgs() { Type = OperationType.Rename });
                    break;
                case ConsoleKey.F9:
                    FileServiceOperation?.Invoke(this, new OperationEventArgs() { Type = OperationType.NewFolder});
                    break;
            }
        }
    }
}

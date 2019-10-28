using System;

namespace FileManager.UserAction
{
    public class UserActionListener
    {
        public event EventHandler<NavigateEventArgs> Navigated;
        public event EventHandler<OperationEventArgs> FileServiceOperation;
        public event Action TabSwitching;
        public event Action PropertyRequest;
        public event Action CompletionOfWork;

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
                case ConsoleKey.F3:
                    Navigated?.Invoke(this, new NavigateEventArgs() { Type = NavigateType.Root });
                    break;
                case ConsoleKey.F4:
                    Navigated?.Invoke(this, new NavigateEventArgs() { Type = NavigateType.Drives});
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
                case ConsoleKey.F5:
                    PropertyRequest?.Invoke();
                    break;
                case ConsoleKey.F6:
                    FileServiceOperation?.Invoke(this, new OperationEventArgs() { Type = OperationType.Rename });
                    break;
                case ConsoleKey.F7:
                    FileServiceOperation?.Invoke(this, new OperationEventArgs() { Type = OperationType.Search });
                    break;
                case ConsoleKey.F8:
                    FileServiceOperation?.Invoke(this, new OperationEventArgs() { Type = OperationType.NewFolder});
                    break;
                case ConsoleKey.Escape:
                    CompletionOfWork?.Invoke();
                    break;
            }
        }
    }
}

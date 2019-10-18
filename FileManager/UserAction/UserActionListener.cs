using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NConsoleGraphics;

namespace FileManager.UserAction
{
    public class UserActionListener
    {
        public event EventHandler<NavigateEventArgs> Navigated;
        public event Action Switch;
        public event EventHandler<ServiceEventArgs> Service;
        //public event Action Copy;
        //public event Action Move;
        //public event Action Rename;
        //public event Action CreateNewFolder;

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
                    Switch?.Invoke();
                    break;
                case ConsoleKey.F1:
                    Service?.Invoke(this, new ServiceEventArgs() { Type = ServiceCommandType.Copy });
                    //Copy?.Invoke();
                    break;
                case ConsoleKey.F2:
                    Service?.Invoke(this, new ServiceEventArgs() { Type = ServiceCommandType.Move });
                    //Move?.Invoke();
                    break;
                case ConsoleKey.F7:
                    Service?.Invoke(this, new ServiceEventArgs() { Type = ServiceCommandType.Rename });
                    //Rename?.Invoke();
                    break;
                case ConsoleKey.F9:
                    Service?.Invoke(this, new ServiceEventArgs() { Type = ServiceCommandType.NewFolder});
                    //CreateNewFolder?.Invoke();
                    break;
            }
        }
    }
}

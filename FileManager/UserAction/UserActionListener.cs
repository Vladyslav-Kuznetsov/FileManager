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
        public event Action<bool> AddToBuffer;
        public event Action Paste;

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
                    AddToBuffer?.Invoke(false);
                    break;
                case ConsoleKey.F2:
                    AddToBuffer?.Invoke(true);
                    break;
                case ConsoleKey.F3:
                    Paste?.Invoke();
                    break;

            }
        }
    }
}

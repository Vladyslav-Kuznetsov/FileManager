﻿using System;
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
        public event Action Copy;
        public event Action Move;

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
                    Copy?.Invoke();
                    break;
                case ConsoleKey.F2:
                    Move?.Invoke();
                    break;
                

            }
        }
    }
}

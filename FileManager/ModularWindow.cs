using NConsoleGraphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace FileManager
{
    public class ModularWindow
    {
        private ConsoleGraphics _graphics;

        public ModularWindow(ConsoleGraphics graphics)
        {
            _graphics = graphics;
        }

        public string EnterName(string oldName)
        {
            bool exit = false;
            List<char> name = new List<char>(oldName);
            string text;

            while (!exit)
            {
                text = (name.Count > 45) ? string.Join("", name.Skip(name.Count - 45)) : string.Join("", name);
                ShowWindow("Enter name :", text, true, false);

                ConsoleKeyInfo key = Console.ReadKey(true);

                switch (key.Key)
                {
                    case ConsoleKey.Enter:
                        exit = true;
                        break;
                    case ConsoleKey.Backspace when name.Count == 0:
                        break;
                    case ConsoleKey.Backspace:
                        name.RemoveAt(name.Count - 1);
                        break;
                    default:
                        name.Add(key.KeyChar);
                        break;
                }
            }

            return string.Join("", name);
        }

        public void ShowWindow(string titleText, string inputText, bool isDialogWindow, bool waitForClosing)
        {
            _graphics.FillRectangle(Settings.ActiveColor, Settings.MessageWindowCoordinateX, Settings.MessageWindowCoordinateY, Settings.MessageWindowWidth, Settings.MessageWindowHeiht);

            if (isDialogWindow)
            {
                _graphics.FillRectangle(0xFF0055de, Settings.MessageWindowCoordinateX + 10, Settings.MessageWindowCoordinateY + 40, Settings.MessageWindowWidth - 20, 30);
            }

            _graphics.DrawString(titleText, Settings.FontName, Settings.BlackColor, Settings.MessageWindowCoordinateX + 10, Settings.MessageWindowCoordinateY);
            _graphics.DrawString(inputText, Settings.FontName, Settings.BlackColor, Settings.MessageWindowCoordinateX + 10, Settings.MessageWindowCoordinateY + 40);
            _graphics.FlipPages();

            while (waitForClosing)
            {
                if (Console.ReadKey(true).Key == ConsoleKey.Enter)
                {
                    return;
                }
            }
        }

        public void ShowBar(string titleText, CancellationToken token)
        {
            int start = Settings.MessageWindowCoordinateX + 10;
            int end = (Settings.MessageWindowCoordinateX + 10) + (Settings.MessageWindowWidth - 20);
            int speed = 1;
            int coordinateX = start;
            int size = (Settings.MessageWindowWidth - 20) / 5;

            while (!token.IsCancellationRequested)
            {
                _graphics.FillRectangle(Settings.ActiveColor, Settings.MessageWindowCoordinateX, Settings.MessageWindowCoordinateY, Settings.MessageWindowWidth, Settings.MessageWindowHeiht);
                _graphics.DrawString(titleText, Settings.FontName, Settings.BlackColor, Settings.MessageWindowCoordinateX + 10, Settings.MessageWindowCoordinateY);
                _graphics.DrawRectangle(Settings.BlackColor, Settings.MessageWindowCoordinateX + 10, Settings.MessageWindowCoordinateY + 40, Settings.MessageWindowWidth - 20, 30);
                _graphics.FillRectangle(0xFF2cff03, coordinateX, Settings.MessageWindowCoordinateY + 41, size, 29);
                _graphics.FlipPages();

                coordinateX += speed;

                if (coordinateX + size == end)
                {
                    speed *= -1;
                }

                if (coordinateX == start)
                {
                    speed *= -1;
                }
            }

        }
    }
}

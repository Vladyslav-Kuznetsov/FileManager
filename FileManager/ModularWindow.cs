using NConsoleGraphics;
using System;
using System.Collections.Generic;
using System.Linq;

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
            string text = string.Empty;

            while (!exit)
            {
                if (name.Count > 45)
                {
                    text = string.Join("", name.Skip(name.Count - 45));
                }
                else
                {
                    text = string.Join("", name);
                }

                Show("Enter name :", text);

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

        private void Show(string titleText, string inputText)
        {
            _graphics.FillRectangle(Settings.ActiveColor, Settings.MessageWindowCoordinateX, Settings.MessageWindowCoordinateY, Settings.MessageWindowWidth, Settings.MessageWindowHeiht);
            _graphics.FillRectangle(0xFF0055de, Settings.MessageWindowCoordinateX + 10, Settings.MessageWindowCoordinateY + 40, Settings.MessageWindowWidth - 20, 30);
            _graphics.DrawString(titleText, "ISOCPEUR", Settings.BlackColor, Settings.MessageWindowCoordinateX + 10, Settings.MessageWindowCoordinateY);
            _graphics.DrawString(inputText, "ISOCPEUR", Settings.BlackColor, Settings.MessageWindowCoordinateX + 10, Settings.MessageWindowCoordinateY + 40);
            _graphics.FlipPages();
        }

        //    public static void CloseMessage()
        //    {
        //        bool exit = false;

        //        while (!exit)
        //        {
        //            ConsoleKeyInfo key = Console.ReadKey(true);

        //            if (key.Key == ConsoleKey.Enter)
        //            {
        //                exit = true;
        //            }
        //        }
        //    }
        //}
    }
}

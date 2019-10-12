using NConsoleGraphics;
using System;

namespace FileManager
{
    public static class Message
    {
        public static void ShowMessage(string titleText, string inputText, ConsoleGraphics graphics)
        {
            graphics.FillRectangle(Settings.ActiveColor, Settings.MessageWindowCoordinateX, Settings.MessageWindowCoordinateY, Settings.MessageWindowWidth, Settings.MessageWindowHeiht);
            graphics.DrawString(titleText, "ISOCPEUR", Settings.BlackColor, Settings.MessageWindowCoordinateX + 10, Settings.MessageWindowCoordinateY);
            graphics.DrawString(inputText, "ISOCPEUR", Settings.BlackColor, Settings.MessageWindowCoordinateX + 10, Settings.MessageWindowCoordinateY + 40);
            graphics.FlipPages();
        }

        public static void CloseMessage()
        {
            bool exit = false;

            while (!exit)
            {
                ConsoleKeyInfo key = Console.ReadKey(true);

                if (key.Key == ConsoleKey.Enter)
                {
                    exit = true;
                }
            }
        }
    }
}

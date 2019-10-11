using NConsoleGraphics;

namespace FileManager
{
    public static class Message
    {
        public static void ShowMessage(string titleText, string inputText, ConsoleGraphics graphics)
        {
            graphics.FillRectangle(Settings.ActiveColor, Settings.MessageWindowCoordinateX, Settings.MessageWindowCoordinateY, Settings.MessageWindowWidth, Settings.MessageWindowHeiht);
            graphics.DrawString(titleText, "ISOCPEUR", Settings.BlackColor, Settings.MessageWindowCoordinateX + 10, Settings.MessageWindowCoordinateY);
            graphics.FillRectangle(Settings.InputMessageColor, Settings.MessageWindowCoordinateX + 10, Settings.MessageWindowCoordinateY + 40, Settings.MessageFieldWidth, Settings.MessageFieldHeiht);
            graphics.DrawString(inputText, "ISOCPEUR", Settings.BlackColor, Settings.MessageWindowCoordinateX + 10, Settings.MessageWindowCoordinateY + 40);
            graphics.FlipPages();
        }
    }
}

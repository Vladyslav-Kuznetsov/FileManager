using NConsoleGraphics;

namespace FileManager.Views
{
    public class Hints
    {
        private ConsoleGraphics _graphics;

        public Hints(ConsoleGraphics graphics)
        {
            _graphics = graphics;
        }
        public void ShowHints()
        {
            _graphics.FillRectangle(Settings.HintsColor, Settings.LeftWindowCoordinateX, Settings.HintsCoordinateY, Settings.HintsWidth, Settings.HintsHeight);
            _graphics.DrawString(Settings.Hints, Settings.FontName, Settings.BlackColor, Settings.LeftWindowCoordinateX, Settings.HintsCoordinateY, 17);
        }
    }
}

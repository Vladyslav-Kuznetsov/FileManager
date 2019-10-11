using NConsoleGraphics;

namespace FileManager
{
    public class Engine
    {
        private readonly Explorer _left;
        private readonly Explorer _rigth;
        private readonly ConsoleGraphics _graphics;
        public SystemItem TempItem { get; set; }
        public bool IsLeftActive { get; set; }
        public bool IsRightActive { get; set; }
        public bool Exit { get; set; }
        public bool IsCut { get; set; }

        public Engine()
        {
            _left = new Explorer(Settings.LeftWindowCoordinateX);
            _rigth = new Explorer(Settings.RigthWindowCoordinateX);
            _graphics = new ConsoleGraphics();
            IsLeftActive = true;
            IsRightActive = false;
            Exit = false;
            IsCut = false;
        }

        public void Start()
        {
            while (!Exit)
            {
                _graphics.FillRectangle(Settings.BlackColor, 0, 0, _graphics.ClientWidth, _graphics.ClientHeight);
                ShowHints();
                _left.Show(_graphics, IsLeftActive);
                _rigth.Show(_graphics, IsRightActive);
                _graphics.FlipPages();

                if (IsLeftActive)
                {
                    _left.Navigate(this, _graphics);
                }
                else
                {
                    _rigth.Navigate(this, _graphics);
                }
            }
        }

        private void ShowHints()
        {
            _graphics.FillRectangle(Settings.HintsColor, Settings.LeftWindowCoordinateX, Settings.HintsCoordinateY, Settings.HintsWidth, Settings.HintsHeight);
            _graphics.DrawString(Settings.Hints, Settings.FontName, Settings.BlackColor, Settings.LeftWindowCoordinateX, Settings.HintsCoordinateY, 18);
        }
    }
}

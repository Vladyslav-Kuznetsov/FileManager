using FileManager.Navigation;
using NConsoleGraphics;
using System.Collections.Generic;

namespace FileManager
{
    public class Engine
    {
        //private readonly Tab _left;
        //private readonly Tab _rigth;
        private readonly List<Tab> _tabs;
        private readonly ConsoleGraphics _graphics;
        private readonly UserActionListener _listener = new UserActionListener();
        public SystemItem TempItem { get; set; }
        public bool IsLeftActive { get; set; }
        public bool IsRightActive { get; set; }
        public bool Exit { get; set; }
        public bool IsCut { get; set; }

        public Engine()
        {
            _tabs = new List<Tab>()
            {
                new Tab(Settings.LeftWindowCoordinateX, _listener),
                new Tab(Settings.RigthWindowCoordinateX, _listener)
            };
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

                foreach(var tab in _tabs)
                {
                    tab.Show(_graphics, IsLeftActive);
                }

                _graphics.FlipPages();
                _listener.ReadInput();
            }
        }

        private void ShowHints()
        {
            _graphics.FillRectangle(Settings.HintsColor, Settings.LeftWindowCoordinateX, Settings.HintsCoordinateY, Settings.HintsWidth, Settings.HintsHeight);
            _graphics.DrawString(Settings.Hints, Settings.FontName, Settings.BlackColor, Settings.LeftWindowCoordinateX, Settings.HintsCoordinateY, 18);
        }
    }
}

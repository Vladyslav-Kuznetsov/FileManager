using FileManager.UserAction;
using NConsoleGraphics;
using System;
using System.Collections.Generic;

namespace FileManager
{
    public class Engine
    {
        private readonly List<Tab> _tabs;
        private readonly ConsoleGraphics _graphics;
        private readonly UserActionListener _userActionListener = new UserActionListener();
        public SystemItem TempItem { get; set; }
        public bool Exit { get; set; }
        public bool IsCut { get; set; }

        public Engine()
        {
            _tabs = new List<Tab>()
            {
                new Tab(Settings.LeftWindowCoordinateX, _userActionListener) { IsActive = true},
                new Tab(Settings.RigthWindowCoordinateX, _userActionListener)
            };
            _graphics = new ConsoleGraphics();
            Exit = false;
            IsCut = false;
            _userActionListener.Switch += SelectNextTab;
        }

        private void SelectNextTab()
        {
            for (int i = 0; i < _tabs.Count; i++)
            {
                if (_tabs[i].IsActive == true)
                {
                    _tabs[i].IsActive = false;

                    if (i == _tabs.Count - 1)
                    {
                        _tabs[0].IsActive = true;
                    }
                    else
                    {
                        _tabs[i + 1].IsActive = true;
                    }

                    return;
                }
            }

        }

        public void Start()
        {
            while (!Exit)
            {
                _graphics.FillRectangle(Settings.BlackColor, 0, 0, _graphics.ClientWidth, _graphics.ClientHeight);
                ShowHints();

                foreach (var tab in _tabs)
                {
                    tab.Show(_graphics);
                }

                _graphics.FlipPages();
                _userActionListener.ReadInput();
            }
        }

        private void ShowHints()
        {
            _graphics.FillRectangle(Settings.HintsColor, Settings.LeftWindowCoordinateX, Settings.HintsCoordinateY, Settings.HintsWidth, Settings.HintsHeight);
            _graphics.DrawString(Settings.Hints, Settings.FontName, Settings.BlackColor, Settings.LeftWindowCoordinateX, Settings.HintsCoordinateY, 18);
        }
    }
}

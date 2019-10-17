using FileManager.Services;
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
        private readonly Clipboard _сlipboard;
        private readonly FileSystemService _fileSystemService;
        public bool Exit { get; set; }

        public Engine()
        {
            _сlipboard = new Clipboard();
            _fileSystemService = new FileSystemService(_сlipboard);
            _tabs = new List<Tab>()
            {
                new Tab(Settings.LeftWindowCoordinateX, _userActionListener,_сlipboard, _fileSystemService) { IsActive = true},
                new Tab(Settings.RigthWindowCoordinateX, _userActionListener,_сlipboard, _fileSystemService)
            };
            _graphics = new ConsoleGraphics();
            Exit = false;
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

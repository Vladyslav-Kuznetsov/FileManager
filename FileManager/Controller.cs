using FileManager.Services;
using FileManager.UserAction;
using NConsoleGraphics;
using System;
using System.Collections.Generic;

namespace FileManager
{
    public class Controller
    {
        private readonly ConsoleGraphics _graphics;
        private readonly UserActionListener _userActionListener;
        private readonly FileSystemService _fileSystemService;
        private readonly List<Tab> _tabs;
        private readonly ModularWindow _modularWindow;
        public bool Exit { get; set; }

        public Controller()
        {
            _graphics = new ConsoleGraphics();
            _fileSystemService = new FileSystemService();
            _userActionListener = new UserActionListener();
            _tabs = new List<Tab>()
            {
                new Tab(Settings.LeftWindowCoordinateX, _userActionListener, _fileSystemService) { IsActive = true},
                new Tab(Settings.RigthWindowCoordinateX, _userActionListener, _fileSystemService)
            };

            _modularWindow = new ModularWindow(_graphics);
            _userActionListener.Switch += SelectNextTab;
            _userActionListener.Service += Service;
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

        private void Service(object sender, ServiceEventArgs e)
        {
            switch (e.Type)
            {
                case ServiceCommandType.Copy:
                    Copy();
                    break;
                case ServiceCommandType.Move:
                    Move();
                    break;
                case ServiceCommandType.Rename:
                    Rename();
                    break;
                case ServiceCommandType.NewFolder:
                    NewFolder();
                    break;
            }
        }

        private void NewFolder()
        {
            for (int i = 0; i < _tabs.Count; i++)
            {
                if (_tabs[i].IsActive == true)
                {
                    string nameFolder = _modularWindow.EnterName("New folder");
                    _fileSystemService.CreateNewFolder(_tabs[i].CurrentPath, nameFolder);
                    return;
                }
            }
        }

        private void Rename()
        {
            for (int i = 0; i < _tabs.Count; i++)
            {
                if (_tabs[i].IsActive == true)
                {
                    string newName = _modularWindow.EnterName(_tabs[i].SelectedItem.Name);
                    string path = (_tabs[i].SelectedItem is FolderItem) ? _tabs[i].SelectedItem.FullName + @"\" : _tabs[i].SelectedItem.FullName;
                    _fileSystemService.Rename(path, newName);
                    return;
                }
            }
        }

        private void Move()
        {
            for (int i = 0; i < _tabs.Count; i++)
            {
                if (_tabs[i].IsActive == true)
                {
                    string sourcePath = (_tabs[i].SelectedItem is FolderItem) ? _tabs[i].SelectedItem.FullName + @"\" : _tabs[i].SelectedItem.FullName;

                    if (i == _tabs.Count - 1)
                    {
                        _fileSystemService.Move(sourcePath, _tabs[0].CurrentPath + $@"\{_tabs[i].SelectedItem.Name}");
                    }
                    else
                    {
                        _fileSystemService.Move(sourcePath, _tabs[i + 1].CurrentPath + $@"\{_tabs[i].SelectedItem.Name}");
                    }

                    return;
                }
            }
        }

        private void Copy()
        {
            for (int i = 0; i < _tabs.Count; i++)
            {
                if (_tabs[i].IsActive == true)
                {
                    string sourcePath = (_tabs[i].SelectedItem is FolderItem) ? _tabs[i].SelectedItem.FullName + @"\" : _tabs[i].SelectedItem.FullName;

                    if (i == _tabs.Count - 1)
                    {
                        _fileSystemService.Copy(sourcePath, _tabs[0].CurrentPath + $@"\{_tabs[i].SelectedItem.Name}");
                    }
                    else
                    {
                        _fileSystemService.Copy(sourcePath, _tabs[i + 1].CurrentPath + $@"\{_tabs[i].SelectedItem.Name}");
                    }

                    return;
                }
            }
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

        private void ShowHints()
        {
            _graphics.FillRectangle(Settings.HintsColor, Settings.LeftWindowCoordinateX, Settings.HintsCoordinateY, Settings.HintsWidth, Settings.HintsHeight);
            _graphics.DrawString(Settings.Hints, Settings.FontName, Settings.BlackColor, Settings.LeftWindowCoordinateX, Settings.HintsCoordinateY, 18);
        }
    }
}

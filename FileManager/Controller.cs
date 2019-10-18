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
            string nameFolder = _modularWindow.EnterName("New folder");
            _fileSystemService.CreateNewFolder(GetActiveAndNextTabs().active.CurrentPath, nameFolder);
        }

        private void Rename()
        {
            string newName = _modularWindow.EnterName(GetActiveAndNextTabs().active.SelectedItem.Name);
            string path = (GetActiveAndNextTabs().active.SelectedItem is FolderItem) ? GetActiveAndNextTabs().active.SelectedItem.FullName + @"\" : GetActiveAndNextTabs().active.SelectedItem.FullName;
            _fileSystemService.Rename(path, newName);
        }

        private void Move()
        {
            var tabs = GetActiveAndNextTabs();
            string sourcePath = (tabs.active.SelectedItem is FolderItem) ? tabs.active.SelectedItem.FullName + @"\" : tabs.active.SelectedItem.FullName;
            _fileSystemService.Move(sourcePath, tabs.next.CurrentPath + $@"\{tabs.active.SelectedItem.Name}");
        }

        private void Copy()
        {
            var tabs = GetActiveAndNextTabs();
            string sourcePath = (tabs.active.SelectedItem is FolderItem) ? tabs.active.SelectedItem.FullName + @"\" : tabs.active.SelectedItem.FullName;
            _fileSystemService.Copy(sourcePath, tabs.next.CurrentPath + $@"\{tabs.active.SelectedItem.Name}");
        }

        private void SelectNextTab()
        {
            var tabs = GetActiveAndNextTabs();
            tabs.active.IsActive = false;
            tabs.next.IsActive = true;
        }

        private (Tab active, Tab next) GetActiveAndNextTabs()
        {
            Tab active = null;
            Tab next = null;

            for (int i = 0; i < _tabs.Count; i++)
            {
                if (_tabs[i].IsActive == true)
                {
                    active = _tabs[i];

                    if (i == _tabs.Count - 1)
                    {
                        next = _tabs[0];
                    }
                    else
                    {
                        next = _tabs[i + 1];
                    }

                    break;
                }
            }

            return (active, next);
        }

        private void ShowHints()
        {
            _graphics.FillRectangle(Settings.HintsColor, Settings.LeftWindowCoordinateX, Settings.HintsCoordinateY, Settings.HintsWidth, Settings.HintsHeight);
            _graphics.DrawString(Settings.Hints, Settings.FontName, Settings.BlackColor, Settings.LeftWindowCoordinateX, Settings.HintsCoordinateY, 18);
        }
    }
}

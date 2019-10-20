using FileManager.Services;
using FileManager.UserAction;
using FileManager.Views;
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
        private readonly SystemItemView _systemItemView;
        private readonly TabView _tabView;
        private readonly ModularWindow _modularWindow;
        public Controller()
        {
            _graphics = new ConsoleGraphics();
            _userActionListener = new UserActionListener();
            _fileSystemService = new FileSystemService();

            _tabs = new List<Tab>()
            {
                new Tab(Settings.LeftWindowCoordinateX, Settings.WindowCoordinateY, _userActionListener, _fileSystemService) { IsActive = true},
                new Tab(Settings.RigthWindowCoordinateX,Settings.WindowCoordinateY, _userActionListener, _fileSystemService)
            };

            _systemItemView = new SystemItemView(_graphics);
            _tabView = new TabView(_tabs, _graphics, _systemItemView);
            _modularWindow = new ModularWindow(_graphics);
            _userActionListener.TabSwitching += SelectNextTab;
            _userActionListener.PropertyRequest += GetProperty;
            _userActionListener.FileServiceOperation += OperationEventHandler;
        }

        public bool Exit { get; set; }

        public void Start()
        {
            while (!Exit)
            {
                _graphics.FillRectangle(Settings.BlackColor, 0, 0, _graphics.ClientWidth, _graphics.ClientHeight);
                ShowHints();
                _tabView.Show();
                _graphics.FlipPages();
                _userActionListener.ReadInput();
            }
        }

        private void GetProperty()
        {
            _systemItemView.ShowProperties(GetActiveAndNextTabs().active.SelectedItem);
        }

        private void OperationEventHandler(object sender, OperationEventArgs e)
        {
            switch (e.Type)
            {
                case OperationType.Copy:
                    Copy();
                    break;
                case OperationType.Move:
                    Move();
                    break;
                case OperationType.Rename:
                    Rename();
                    break;
                case OperationType.NewFolder:
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
            _graphics.DrawString(Settings.Hints, Settings.FontName, Settings.BlackColor, Settings.LeftWindowCoordinateX, Settings.HintsCoordinateY, 17);
        }
    }
}

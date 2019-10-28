using FileManager.Services;
using FileManager.UserAction;
using FileManager.Views;
using NConsoleGraphics;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

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
        private readonly Hints _hints;
        public Controller()
        {
            _graphics = new ConsoleGraphics();
            _userActionListener = new UserActionListener();
            _modularWindow = new ModularWindow(_graphics);
            _fileSystemService = new FileSystemService();

            _tabs = new List<Tab>()
            {
                new Tab(Settings.LeftWindowCoordinateX, Settings.WindowCoordinateY, _userActionListener, _fileSystemService) { IsActive = true},
                new Tab(Settings.RigthWindowCoordinateX,Settings.WindowCoordinateY, _userActionListener, _fileSystemService)
            };

            _systemItemView = new SystemItemView(_graphics);
            _tabView = new TabView(_tabs, _graphics, _systemItemView);
            _hints = new Hints(_graphics);

            _userActionListener.TabSwitching += SelectNextTab;
            _userActionListener.PropertyRequest += GetProperty;
            _userActionListener.FileServiceOperation += OperationEventHandler;
            _userActionListener.CompletionOfWork += () => Exit = true;
        }

        public bool Exit { get; private set; }

        public void Start()
        {
            while (!Exit)
            {
                _graphics.FillRectangle(Settings.BlackColor, 0, 0, _graphics.ClientWidth, _graphics.ClientHeight);
                _hints.ShowHints();
                _tabView.Show();
                _graphics.FlipPages();
                _userActionListener.ReadInput();
            }
        }

        private void OperationEventHandler(object sender, OperationEventArgs e)
        {
            switch (e.Type)
            {
                case OperationType.Copy when GetActiveAndNextTabs().active.IsNotEmpty && GetActiveAndNextTabs().next.CurrentPath != string.Empty:
                    Copy();
                    break;
                case OperationType.Move when GetActiveAndNextTabs().active.IsNotEmpty && GetActiveAndNextTabs().next.CurrentPath != string.Empty:
                    Move();
                    break;
                case OperationType.Rename when GetActiveAndNextTabs().active.IsNotEmpty:
                    Rename();
                    break;
                case OperationType.NewFolder when GetActiveAndNextTabs().active.CurrentPath != string.Empty:
                    NewFolder();
                    break;
                case OperationType.Search when GetActiveAndNextTabs().active.CurrentPath != string.Empty:
                    SearchFile();
                    break;
            }
        }

        private void NewFolder()
        {
            string nameFolder = _modularWindow.EnterName("New folder");
            string path = $@"{GetActiveAndNextTabs().active.CurrentPath}\{nameFolder}\";

            if (!_fileSystemService.Exists(path))
            {
                _fileSystemService.CreateNewFolder(path);
            }
            else
            {
                _modularWindow.ShowWindow("An element with this name already exists at the specified path", "Press Enter to continue", false, true);
            }
        }

        private void Rename()
        {
            string newName = _modularWindow.EnterName(GetActiveAndNextTabs().active.SelectedItem.Name);
            string path = (GetActiveAndNextTabs().active.SelectedItem is FolderItem) ? GetActiveAndNextTabs().active.SelectedItem.FullName + @"\" : GetActiveAndNextTabs().active.SelectedItem.FullName;
            string pathToCheck = (GetActiveAndNextTabs().active.SelectedItem is FolderItem) ? $@"{GetActiveAndNextTabs().active.CurrentPath}\{newName}\" : $@"{GetActiveAndNextTabs().active.CurrentPath}\{newName}";

            if (!_fileSystemService.Exists(pathToCheck))
            {
                _fileSystemService.Rename(path, newName);
            }
            else
            {
                _modularWindow.ShowWindow("An element with this name already exists at the specified path", "Press Enter to continue", false, true);
            }
        }

        private void GetProperty()
        {
            if (!GetActiveAndNextTabs().active.IsNotEmpty)
            {
                return;
            }

            if (GetActiveAndNextTabs().active.SelectedItem is FolderItem folder)
            {
                using (CancellationTokenSource cts = new CancellationTokenSource())
                {
                    CancellationToken token = cts.Token;
                    Task.Run(() => _modularWindow.ShowBar("Properties are being processed. Please, wait", token));
                    var info = _fileSystemService.GetFolderProperties(folder.FullName);
                    cts.Cancel();
                    _systemItemView.ShowProperties(new FolderItem(folder, info.size, info.countFolder, info.countFiles));
                }
            }
            else
            {
                _systemItemView.ShowProperties(GetActiveAndNextTabs().active.SelectedItem);
            }
        }

        private void SearchFile()
        {
            var name = _modularWindow.EnterName("");
            bool isFind;

            using (CancellationTokenSource cts = new CancellationTokenSource())
            {
                CancellationToken token = cts.Token;
                Task.Run(() => _modularWindow.ShowBar($"Search file", token));
                isFind = _fileSystemService.FindFileByName(name.ToLower(), GetActiveAndNextTabs().active.CurrentPath);
                cts.Cancel();
            }

            if (!isFind)
            {
                _modularWindow.ShowWindow("File not found", "Press Enter to continue", false, true);
            }
        }

        private void Move()
        {
            var tabs = GetActiveAndNextTabs();
            string sourcePath;
            string destPath;

            if (tabs.active.SelectedItem is FolderItem)
            {
                sourcePath = tabs.active.SelectedItem.FullName + @"\";
                destPath = tabs.next.CurrentPath + $@"\{tabs.active.SelectedItem.Name}\";
            }
            else
            {
                sourcePath = tabs.active.SelectedItem.FullName;
                destPath = tabs.next.CurrentPath + $@"\{tabs.active.SelectedItem.Name}";
            }

            if (!_fileSystemService.Exists(destPath))
            {
                using (CancellationTokenSource cts = new CancellationTokenSource())
                {
                    CancellationToken token = cts.Token;
                    Task.Run(() => _modularWindow.ShowBar($"Moving to: {tabs.next.CurrentPath}", token));
                    _fileSystemService.Move(sourcePath, destPath);
                    cts.Cancel();
                    Thread.Sleep(10);
                }
            }
            else
            {
                _modularWindow.ShowWindow("An element with this name already exists at the specified path", "Press Enter to continue", false, true);
            }
        }

        private void Copy()
        {
            var tabs = GetActiveAndNextTabs();
            string sourcePath;
            string destPath;

            if (tabs.active.SelectedItem is FolderItem)
            {
                sourcePath = tabs.active.SelectedItem.FullName + @"\";
                destPath = tabs.next.CurrentPath + $@"\{tabs.active.SelectedItem.Name}\";
            }
            else
            {
                sourcePath = tabs.active.SelectedItem.FullName;
                destPath = tabs.next.CurrentPath + $@"\{tabs.active.SelectedItem.Name}";
            }

            if (!_fileSystemService.Exists(destPath))
            {
                using (CancellationTokenSource cts = new CancellationTokenSource())
                {
                    CancellationToken token = cts.Token;
                    Task.Run(() => _modularWindow.ShowBar($"Copy to: {tabs.next.CurrentPath}", token));
                    _fileSystemService.Copy(sourcePath, destPath);
                    cts.Cancel();
                    Thread.Sleep(10);
                }
            }
            else
            {
                _modularWindow.ShowWindow("An element with this name already exists at the specified path", "Press Enter to continue", false, true);
            }
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
    }
}

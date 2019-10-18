using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using FileManager.Services;
using FileManager.UserAction;
using NConsoleGraphics;

namespace FileManager
{
    public class Tab
    {
        private readonly int _windowCordinateX;
        private readonly List<DriveInfo> _drives;
        private readonly List<SystemItem> _folderContent;
        private readonly UserActionListener _listener;
        private readonly FileSystemService _fileSystemService;
        private int _startPosition;
        private int _endPosition;
        private int _position;
        private string _currentPath;
        private bool _isFind;
        private bool _isActive;
        public bool IsActive
        {
            get { return _isActive; }
            set
            {
                if (_isActive == value)
                {
                    return;
                }

                _isActive = value;

                if (_isActive)
                {
                    _listener.Navigated += Navigate;
                }
                else
                {
                    _listener.Navigated -= Navigate;
                }
            }
        }
        public string CurrentPath
        {
            get
            {
                return _currentPath;
            }
        }

        public SystemItem SelectedItem
        {
            get
            {
                return _folderContent[_position];
            }
        }

        public Tab(int windowCordinateX, UserActionListener listener, FileSystemService fileSystemService)
        {
            _windowCordinateX = windowCordinateX;
            _currentPath = string.Empty;
            _folderContent = new List<SystemItem>();
            _drives = new List<DriveInfo>();
            _drives.AddRange(DriveInfo.GetDrives().Where(drive => drive.DriveType == DriveType.Fixed));
            _isFind = false;
            _listener = listener;
            _fileSystemService = fileSystemService;
        }

        public void Show(ConsoleGraphics graphics)
        {
            uint color = (IsActive) ? Settings.ActiveColor : Settings.InactiveColor;
            graphics.DrawRectangle(color, _windowCordinateX, Settings.WindowCoordinateY, Settings.WindowWidth, Settings.WindowHeight);

            if (_currentPath == string.Empty)
            {
                DisplayDrives(graphics, color);
            }
            else
            {
                InitCurrentDirectory();
                CheckPosition();
                DisplayFolderContent(graphics, color);
            }
        }

        private void Navigate(object sender, NavigateEventArgs e)
        {
            switch (e.Type)
            {
                case NavigateType.Up:
                    MoveUp();
                    break;
                case NavigateType.Down:
                    MoveDown();
                    break;
                case NavigateType.Enter when _currentPath == string.Empty:
                    InFolder(_drives[_position].Name);
                    break;
                case NavigateType.Enter when _folderContent.Any() && (_folderContent[_position] is FolderItem):
                    InFolder(_folderContent[_position].Name);
                    break;
                case NavigateType.Back when _currentPath == string.Empty:
                    break;
                case NavigateType.Back:
                    InFolder("..");
                    break;
            }
        }

        private void DisplayDrives(ConsoleGraphics graphics, uint color)
        {
            int coordinateY = Settings.WindowCoordinateY;

            for (int i = 0; i < _drives.Count; i++)
            {
                if (i == _position)
                {
                    graphics.FillRectangle(color, _windowCordinateX, coordinateY + 5, Settings.WindowWidth, Settings.FontSize);
                    graphics.DrawString($"{_drives[i].Name}", Settings.FontName, Settings.BlackColor, _windowCordinateX, coordinateY, Settings.FontSize);
                }
                else
                {
                    graphics.DrawString($"{_drives[i].Name}", Settings.FontName, color, _windowCordinateX, coordinateY, Settings.FontSize);
                }

                coordinateY += Settings.FontSize + 1;
            }
        }

        private void DisplayFolderContent(ConsoleGraphics graphics, uint color)
        {
            int coordinateY = Settings.WindowCoordinateY;
            _endPosition = (_folderContent.Count > _startPosition + Settings.NumberOfDisplayedStrings) ? _startPosition + Settings.NumberOfDisplayedStrings : _folderContent.Count;

            for (int i = _startPosition; i < _endPosition; i++)
            {
                if (i == _position)
                {
                    graphics.FillRectangle(color, _windowCordinateX, coordinateY + 5, Settings.WindowWidth, Settings.FontSize);
                    _folderContent[i].ShowInfo(graphics, Settings.BlackColor, _windowCordinateX, coordinateY);
                }
                else
                {
                    _folderContent[i].ShowInfo(graphics, color, _windowCordinateX, coordinateY);
                }

                coordinateY += Settings.FontSize + 1;
            }
        }

        private void InitCurrentDirectory()
        {
            _folderContent.Clear();
            _folderContent.AddRange(_fileSystemService.GetFolderContent(_currentPath));
        }

        private void CheckPosition()
        {
            if (_position > _folderContent.Count - 1)
            {
                _position--;
            }
            else if (_position < 0)
            {
                _position++;
            }
        }

        private void SetStartingPosition()
        {
            if (_position == _startPosition - 1 && _startPosition != 0)
            {
                _startPosition--;
            }
            else if (_position == _endPosition && _endPosition != _folderContent.Count)
            {
                _startPosition++;
            }
            else if (_position == 0)
            {
                _startPosition = 0;
            }
            else if (_position == _folderContent.Count - 1 && _endPosition != _folderContent.Count)
            {
                _startPosition = _folderContent.Count - Settings.NumberOfDisplayedStrings;
            }
        }

        private void MoveDown()
        {
            if (_currentPath == string.Empty)
            {
                _position = (_position++ == _drives.Count - 1) ? 0 : _position++;
            }
            else
            {
                _position = (_position++ == _folderContent.Count - 1) ? 0 : _position++;
                SetStartingPosition();
            }
        }

        private void MoveUp()
        {
            if (_currentPath == string.Empty)
            {
                _position = (_position-- <= 0) ? _drives.Count - 1 : _position--;
            }
            else
            {
                _position = (_position-- <= 0) ? _folderContent.Count - 1 : _position--;
                SetStartingPosition();
            }
        }

        private void InFolder(string folderPath)
        {
            _currentPath = Path.Combine(_currentPath, folderPath);
            _currentPath = new DirectoryInfo(_currentPath).FullName;
            _position = 0;
            _startPosition = 0;
        }

        //private string EnterName(ConsoleGraphics graphics)
        //{
        //    bool exit = false;
        //    List<char> name = new List<char>();
        //    string result = string.Empty;

        //    while (!exit)
        //    {
        //        Message.ShowMessage("Enter name:", result, graphics);
        //        ConsoleKeyInfo key = Console.ReadKey(true);

        //        switch (key.Key)
        //        {
        //            case ConsoleKey.Enter:
        //                exit = true;
        //                break;
        //            case ConsoleKey.Backspace when name.Count == 0:
        //                break;
        //            case ConsoleKey.Backspace:
        //                name.RemoveAt(name.Count - 1);
        //                break;
        //            default:
        //                name.Add(key.KeyChar);
        //                break;
        //        }

        //        result = string.Join("", name);
        //    }
        //    return result;
        //}

        //private void FindFileByName(string name, string currentPath, ConsoleGraphics graphics)
        //{
        //    DirectoryInfo directory = new DirectoryInfo(currentPath);
        //    //Message.ShowMessage("Search in folder:", directory.Name, graphics);

        //    if (SystemItem.HasFolderPermission(directory))
        //    {
        //        foreach (var file in directory.EnumerateFiles().Where(f => !f.Attributes.HasFlag(FileAttributes.Hidden)))
        //        {
        //            if (file.Name.ToLower().Contains(name))
        //            {
        //                var col = directory.EnumerateDirectories().Where(dir => !dir.Attributes.HasFlag(FileAttributes.Hidden)).Cast<FileSystemInfo>().Concat(directory.EnumerateFiles().Where(f => !f.Attributes.HasFlag(FileAttributes.Hidden)).Cast<FileSystemInfo>()).ToList();
        //                _currentPath = directory.FullName;
        //                _position = col.IndexOf(col.Where(f => f.Name.ToLower().Contains(name)).First());
        //                _isFind = true;
        //                return;
        //            }
        //        }

        //        foreach (var dir in directory.EnumerateDirectories().Where(d => !d.Attributes.HasFlag(FileAttributes.Hidden)))
        //        {
        //            FindFileByName(name, dir.FullName, graphics);
        //        }
        //    }
        //}

        private void ShowIfFileFound(ConsoleGraphics graphics)
        {
            if (_isFind == false)
            {
                //Message.ShowMessage("File not found:", "Press Enter to continue", graphics);
                //Message.CloseMessage();
            }

            _isFind = false;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using NConsoleGraphics;

namespace FileManager
{
    public class Explorer
    {
        private readonly int _windowCordinateX;
        private readonly List<DriveInfo> _drives;
        private readonly List<SystemItem> _folderContent;
        private int _startPosition;
        private int _endPosition;
        private int _position;
        private string _currentPath;
        private bool _isFind;

        public Explorer(int windowCordinateX)
        {
            _windowCordinateX = windowCordinateX;
            _currentPath = string.Empty;
            _folderContent = new List<SystemItem>();
            _drives = new List<DriveInfo>();
            _drives.AddRange(DriveInfo.GetDrives().Where(drive => drive.DriveType == DriveType.Fixed));
            _isFind = false;
        }

        public void Show(ConsoleGraphics graphics, bool isActive)
        {
            uint color = (isActive) ? Settings.ActiveColor : Settings.InactiveColor;
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

        public void Navigate(Engine engine, ConsoleGraphics graphics)
        {
            ConsoleKey command = Console.ReadKey(true).Key;

            switch (command)
            {
                case ConsoleKey.DownArrow:
                    MoveDown();
                    break;
                case ConsoleKey.UpArrow:
                    MoveUp();
                    break;
                case ConsoleKey.Enter when _currentPath == string.Empty:
                    InFolder(_drives[_position].Name);
                    break;
                case ConsoleKey.Enter when _folderContent.Any() && (_folderContent[_position] is FolderItem):
                    InFolder(_folderContent[_position].Name);
                    break;
                case ConsoleKey.Enter when _folderContent.Any() && _folderContent[_position] is FileItem:
                    Process.Start(_folderContent[_position].FullName);
                    break;
                case ConsoleKey.Backspace when _currentPath == string.Empty:
                    break;
                case ConsoleKey.Backspace:
                    InFolder("..");
                    break;
                case ConsoleKey.Tab:
                    engine.IsLeftActive = !engine.IsLeftActive;
                    engine.IsRightActive = !engine.IsRightActive;
                    break;
                case ConsoleKey.F1 when _folderContent.Any():
                    SystemItem.Copy(engine, _folderContent[_position]);
                    break;
                case ConsoleKey.F2 when _folderContent.Any():
                    SystemItem.Cut(engine, _folderContent[_position]);
                    break;
                case ConsoleKey.F3:
                    SystemItem.Paste(engine, _currentPath, graphics);
                    break;
                case ConsoleKey.F4 when _currentPath != string.Empty:
                    InFolder(Path.GetPathRoot(_currentPath));
                    break;
                case ConsoleKey.F5:
                    _currentPath = string.Empty;
                    _position = 0;
                    break;
                case ConsoleKey.F6 when _currentPath != string.Empty && _folderContent.Any():
                    _folderContent[_position].ShowProperties(graphics);
                    break;
                case ConsoleKey.F7 when _folderContent.Any():
                    _folderContent[_position].Rename(EnterName(graphics));
                    break;
                case ConsoleKey.F8 when _currentPath != string.Empty:
                    FindFileByName(EnterName(graphics), _currentPath, graphics);
                    ShowIfFileFound(graphics);
                    break;
                case ConsoleKey.F9 when _currentPath != string.Empty:
                    Directory.CreateDirectory($@"{_currentPath}\{EnterName(graphics)}");
                    break;
                case ConsoleKey.Escape:
                    engine.Exit = !engine.Exit;
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
            _endPosition= (_folderContent.Count > _startPosition + Settings.NumberOfDisplayedStrings) ? _startPosition + Settings.NumberOfDisplayedStrings : _folderContent.Count;

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
            DirectoryInfo directory = new DirectoryInfo(_currentPath);
            _folderContent.Clear();
            _folderContent.AddRange(directory.EnumerateDirectories().Where(dir => SystemItem.HasFolderPermission(dir) && !dir.Attributes.HasFlag(FileAttributes.Hidden)).Select(dir => new FolderItem(dir)).Cast<SystemItem>().Concat(directory.EnumerateFiles().Where(file => !file.Attributes.HasFlag(FileAttributes.Hidden)).Select(file => new FileItem(file)).Cast<SystemItem>()));
        }

        private void CheckPosition()
        {
            if (_position > _folderContent.Count - 1)
            {
                _position--;
            }
            else if(_position < 0)
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

        private string EnterName(ConsoleGraphics graphics)
        {
            bool exit = false;
            List<char> name = new List<char>();
            string result = string.Empty;

            while (!exit)
            {
                Message.ShowMessage("Enter name:", result, graphics);
                ConsoleKeyInfo key = Console.ReadKey(true);

                switch (key.Key)
                {
                    case ConsoleKey.Enter:
                        exit = true;
                        break;
                    case ConsoleKey.Backspace when name.Count == 0:
                        break;
                    case ConsoleKey.Backspace:
                        name.RemoveAt(name.Count - 1);
                        break;
                    default:
                        name.Add(key.KeyChar);
                        break;
                }

                result = string.Join("", name);
            }
            return result;
        }

        private void FindFileByName(string name, string currentPath, ConsoleGraphics graphics)
        {
            DirectoryInfo directory = new DirectoryInfo(currentPath);
            Message.ShowMessage("Search in folder:", directory.Name, graphics);

            if(SystemItem.HasFolderPermission(directory))
            {
                foreach (var file in directory.EnumerateFiles().Where(f => !f.Attributes.HasFlag(FileAttributes.Hidden)))
                {
                    if (file.Name.ToLower().Contains(name))
                    {
                        var col = directory.EnumerateDirectories().Where(dir => !dir.Attributes.HasFlag(FileAttributes.Hidden)).Cast<FileSystemInfo>().Concat(directory.EnumerateFiles().Where(f => !f.Attributes.HasFlag(FileAttributes.Hidden)).Cast<FileSystemInfo>()).ToList();
                        _currentPath = directory.FullName;
                        _position = col.IndexOf(col.Where(f => f.Name.ToLower().Contains(name)).First());
                        _isFind = true;
                        return;
                    }
                }

                foreach (var dir in directory.EnumerateDirectories().Where(d => !d.Attributes.HasFlag(FileAttributes.Hidden)))
                {
                    FindFileByName(name, dir.FullName, graphics);
                }
            }
        }

        private void ShowIfFileFound(ConsoleGraphics graphics)
        {
            if (_isFind == false)
            {
                Message.ShowMessage("File not found:", "Press Enter to continue", graphics);
                Message.CloseMessage();
            }

            _isFind = false;
        }
    }
}

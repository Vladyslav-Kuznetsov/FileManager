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
        private int _position;
        private string _currentPath;
        private bool _propertiesActive;
        private bool _isFind;

        public Explorer(int windowCordinateX)
        {
            _windowCordinateX = windowCordinateX;
            _currentPath = string.Empty;
            _folderContent = new List<SystemItem>();
            _drives = new List<DriveInfo>();
            _drives.AddRange(DriveInfo.GetDrives().Where(drive => drive.DriveType == DriveType.Fixed));
            _propertiesActive = false;
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

                if (_position > _folderContent.Count - 1)
                {
                    _position--;
                }

                DisplayFolderContent(graphics, color);

                if (_propertiesActive)
                {
                    _folderContent[_position].ShowProperties(graphics, _windowCordinateX);
                    _propertiesActive = false;
                }
            }
        }

        public void Navigate(Engine engine, ConsoleGraphics graphics)
        {
            ConsoleKey command = Console.ReadKey().Key;

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
                case ConsoleKey.Enter when _folderContent[_position] is FileItem:
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
                case ConsoleKey.F1:
                    SystemItem.Copy(engine, _folderContent[_position]);
                    break;
                case ConsoleKey.F2:
                    SystemItem.Cut(engine, _folderContent[_position]);
                    break;
                case ConsoleKey.F3:
                    SystemItem.Paste(engine, _currentPath);
                    break;
                case ConsoleKey.F4:
                    _currentPath = _folderContent[_position].Root;
                    break;
                case ConsoleKey.F5:
                    _currentPath = string.Empty;
                    _position = 0;
                    break;
                case ConsoleKey.F6 when _currentPath != string.Empty:
                    _propertiesActive = true;
                    break;
                case ConsoleKey.F7:
                    _folderContent[_position].Rename(EnterName(graphics));
                    break;
                case ConsoleKey.F8:
                    FindFileByName(EnterName(graphics), _currentPath);
                    ShowIfFileFound(graphics);
                    break;
                case ConsoleKey.F9:
                    Directory.CreateDirectory($@"{_currentPath}\\{EnterName(graphics)}");
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

            for (int i = 0; i < _folderContent.Count; i++)
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
            //_folderContent.AddRange(directory.GetDirectories().Select(dir => new FolderItem(dir)).Cast<SystemItem>().Concat(directory.GetFiles().Select(file => new FileItem(file)).Cast<SystemItem>()));
            _folderContent.AddRange(directory.EnumerateDirectories().Where(dir => !dir.Attributes.HasFlag(FileAttributes.Hidden)).Select(dir => new FolderItem(dir)).Cast<SystemItem>().Concat(directory.EnumerateFiles().Where(file => !file.Attributes.HasFlag(FileAttributes.Hidden)).Select(file => new FileItem(file)).Cast<SystemItem>()));
           
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
            }
        }

        private void InFolder(string folderPath)
        {
            _currentPath = Path.Combine(_currentPath, folderPath);
            _currentPath = new DirectoryInfo(_currentPath).FullName;
            _position = 0;
        }

        private string EnterName(ConsoleGraphics graphics)
        {
            bool exit = false;
            List<char> name = new List<char>();
            string result = string.Empty;

            while (!exit)
            {
                graphics.FillRectangle(Settings.ActiveColor, Settings.InputWindowCoordinateX, Settings.InputWindowCoordinateY, Settings.InputWindowWidth, Settings.InputWindowHeiht);
                graphics.DrawString("Enter name:", "ISOCPEUR", Settings.BlackColor, Settings.InputWindowCoordinateX + 10, Settings.InputWindowCoordinateY);
                graphics.DrawRectangle(Settings.BlackColor, Settings.InputWindowCoordinateX + 10, Settings.InputWindowCoordinateY + 40, Settings.InputFieldWidth, Settings.InputFieldHeiht);
                graphics.DrawString(result, "ISOCPEUR", Settings.BlackColor, Settings.InputWindowCoordinateX + 10, Settings.InputWindowCoordinateY + 40);
                graphics.FlipPages();

                ConsoleKeyInfo key = Console.ReadKey();

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

        private void FindFileByName(string name, string currentPath)
        {
            DirectoryInfo directory = new DirectoryInfo(currentPath);

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
                FindFileByName(name, dir.FullName);
            }
        }

        private void ShowIfFileFound(ConsoleGraphics graphics)
        {
            if (_isFind == false)
            {
                graphics.FillRectangle(Settings.ActiveColor, Settings.InputWindowCoordinateX, Settings.InputWindowCoordinateY, Settings.InputWindowWidth, Settings.InputWindowHeiht);
                graphics.DrawString("File not found:", "ISOCPEUR", Settings.BlackColor, Settings.InputWindowCoordinateX + 10, Settings.InputWindowCoordinateY);
                graphics.DrawString("Press Enter to continue", "ISOCPEUR", Settings.BlackColor, Settings.InputWindowCoordinateX + 10, Settings.InputWindowCoordinateY + 40);
                graphics.FlipPages();
                Console.ReadLine();
            }

            _isFind = false;
        }
    }
}

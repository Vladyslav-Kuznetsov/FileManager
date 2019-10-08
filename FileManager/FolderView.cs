using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using NConsoleGraphics;

namespace FileManager
{
    public class FolderView
    {
        private readonly int _windowCordinateX;
        private readonly List<DriveInfo> _drives;
        private readonly List<FileSystemInfo> _folderContent;
        private int _position;
        private string _currentPath;
        private bool _propertiesActive;

        public FolderView(int windowCordinateX)
        {
            _windowCordinateX = windowCordinateX;
            _currentPath = string.Empty;
            _folderContent = new List<FileSystemInfo>();
            _drives = new List<DriveInfo>();
            _drives.AddRange(DriveInfo.GetDrives());
            _propertiesActive = false;
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
                DisplayFolderContent(graphics, color);

                if (_propertiesActive)
                {
                    ShowProperties(graphics, color);
                    _propertiesActive = false;
                }
            }
        }

        public void Navigate(Window window)
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
                case ConsoleKey.Enter when _folderContent.Any() && (_folderContent[_position] is DirectoryInfo):
                    InFolder(_folderContent[_position].Name);
                    break;
                case ConsoleKey.Enter when _folderContent[_position] is FileInfo:
                    Process.Start(_folderContent[_position].FullName);
                    break;
                case ConsoleKey.Backspace when _currentPath == string.Empty:
                    break;
                case ConsoleKey.Backspace:
                    InFolder("..");
                    break;
                case ConsoleKey.Tab:
                    window.IsLeftActive = !window.IsLeftActive;
                    window.IsRightActive = !window.IsRightActive;
                    break;
                case ConsoleKey.F1:
                    Copy(window);
                    break;
                case ConsoleKey.F2:
                    Cut(window);
                    break;
                case ConsoleKey.F3:
                    Paste(window);
                    break;
                case ConsoleKey.F4:
                    MoveToRoot(_folderContent[_position]);
                    break;
                case ConsoleKey.F5:
                    _currentPath = string.Empty;
                    break;
                case ConsoleKey.F6 when _currentPath != string.Empty:
                    _propertiesActive = true;
                    break;
                case ConsoleKey.Escape:
                    window.Exit = !window.Exit;
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

                coordinateY += Settings.FontSize;
            }
        }

        private void DisplayFolderContent(ConsoleGraphics graphics, uint color)
        {
            int coordinateY = Settings.WindowCoordinateY;

            for (int i = 0; i < _folderContent.Count; i++)
            {
                if (i == _position)
                {
                    graphics.FillRectangle(color, _windowCordinateX, coordinateY + 5, Settings.WindowWidth, Settings.FontSize - 1);

                    graphics.DrawString($"{_folderContent[i].Name}", Settings.FontName, Settings.BlackColor, _windowCordinateX, coordinateY, Settings.FontSize);

                    if (_folderContent[i] is FileInfo file)
                    {
                        graphics.DrawString($"<{file.Extension}>", Settings.FontName, Settings.BlackColor, _windowCordinateX + Settings.ExtensionCoodrinateX, coordinateY, Settings.FontSize);
                        graphics.DrawString(ConvertByte(file.Length), Settings.FontName, Settings.BlackColor, _windowCordinateX + Settings.SizeCoodrinateX, coordinateY, Settings.FontSize);
                    }
                    else
                    {
                        graphics.DrawString("<dir>", Settings.FontName, Settings.BlackColor, _windowCordinateX + Settings.ExtensionCoodrinateX, coordinateY, Settings.FontSize);
                    }
                }
                else
                {
                    graphics.DrawString($"{_folderContent[i].Name}", Settings.FontName, color, _windowCordinateX, coordinateY, Settings.FontSize);

                    if (_folderContent[i] is FileInfo file)
                    {
                        graphics.DrawString($"<{file.Extension}>", Settings.FontName, color, _windowCordinateX + Settings.ExtensionCoodrinateX, coordinateY, Settings.FontSize);
                        graphics.DrawString(ConvertByte(file.Length), Settings.FontName, color, _windowCordinateX + Settings.SizeCoodrinateX, coordinateY, Settings.FontSize);
                    }
                    else
                    {
                        graphics.DrawString("<dir>", Settings.FontName, color, _windowCordinateX + Settings.ExtensionCoodrinateX, coordinateY, Settings.FontSize);
                    }
                }

                coordinateY += Settings.FontSize;
            }
        }

        private void InitCurrentDirectory()
        {
            _folderContent.Clear();
            _folderContent.AddRange(Directory.GetDirectories(_currentPath).Select(str => new DirectoryInfo(str)).Cast<FileSystemInfo>().Concat(Directory.GetFiles(_currentPath).Select(str => new FileInfo(str)).Cast<FileSystemInfo>()));
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
            if (folderPath == ".." && Path.GetPathRoot(_currentPath) == _currentPath)
            {
                _currentPath = string.Empty;
            }
            else
            {
                _currentPath = Path.Combine(_currentPath, folderPath);
                _currentPath = new DirectoryInfo(_currentPath).FullName;
            }

            _position = 0;
        }

        private void Copy(Window window)
        {
            window.TempItem = _folderContent[_position];
        }

        private void Paste(Window window)
        {
            if(window.TempItem is FileInfo file)
            {
                if(window.IsCut)
                {
                    file.MoveTo($@"{ _currentPath}\\{file.Name}");
                    window.IsCut = false;
                }
                else
                {
                    file.CopyTo($@"{ _currentPath}\\{file.Name}");
                } 
            }

            if(window.TempItem is DirectoryInfo directory)
            {
                //if (window.IsCut)
                //{
                //    file.MoveTo($@"{ _currentPath}\\{file.Name}");
                //    window.IsCut = false;
                //}
                //else
                //{
                //    directory.CopyTo($@"{ _currentPath}\\{file.Name}");
                //}
            }
        }

        private void Cut(Window window)
        {
            window.TempItem = _folderContent[_position];
            window.IsCut = true;
        }

        private void ShowProperties(ConsoleGraphics graphics, uint color)
        {
            graphics.DrawRectangle(color, _windowCordinateX, Settings.PropertiesCoordinateY, Settings.WindowWidth, Settings.PropertiesHeight);

            if (_folderContent[_position] is FileInfo file)
            {
                graphics.DrawString("Name:", Settings.FontName, color, _windowCordinateX, Settings.PropertiesCoordinateY, Settings.FontSize);
                graphics.DrawString($"{file.Name}", Settings.FontName, color, _windowCordinateX + Settings.PropertiesInfoCoordinateX, Settings.PropertiesCoordinateY, Settings.FontSize);
                graphics.DrawString("Parent directory:", Settings.FontName, color, _windowCordinateX, Settings.PropertiesCoordinateY + Settings.FontSize, Settings.FontSize);
                graphics.DrawString($"{file.DirectoryName}", Settings.FontName, color, _windowCordinateX + Settings.PropertiesInfoCoordinateX, Settings.PropertiesCoordinateY + Settings.FontSize, Settings.FontSize);
                graphics.DrawString("Root directory:", Settings.FontName, color, _windowCordinateX, Settings.PropertiesCoordinateY + Settings.FontSize * 2, Settings.FontSize);
                graphics.DrawString($"{file.Directory.Root}", Settings.FontName, color, _windowCordinateX + Settings.PropertiesInfoCoordinateX, Settings.PropertiesCoordinateY + Settings.FontSize * 2, Settings.FontSize);
                graphics.DrawString("Is read only:", Settings.FontName, color, _windowCordinateX, Settings.PropertiesCoordinateY + Settings.FontSize * 3, Settings.FontSize);
                graphics.DrawString($"{file.IsReadOnly}", Settings.FontName, color, _windowCordinateX + Settings.PropertiesInfoCoordinateX, Settings.PropertiesCoordinateY + Settings.FontSize * 3, Settings.FontSize);
                graphics.DrawString("Last read time:", Settings.FontName, color, _windowCordinateX, Settings.PropertiesCoordinateY + Settings.FontSize * 4, Settings.FontSize);
                graphics.DrawString($"{file.LastAccessTime}", Settings.FontName, color, _windowCordinateX + Settings.PropertiesInfoCoordinateX, Settings.PropertiesCoordinateY + Settings.FontSize * 4, Settings.FontSize);
                graphics.DrawString("Last write time:", Settings.FontName, color, _windowCordinateX, Settings.PropertiesCoordinateY + Settings.FontSize * 5, Settings.FontSize);
                graphics.DrawString($"{file.LastWriteTime}", Settings.FontName, color, _windowCordinateX + Settings.PropertiesInfoCoordinateX, Settings.PropertiesCoordinateY + Settings.FontSize * 5, Settings.FontSize);
                graphics.DrawString("Size:", Settings.FontName, color, _windowCordinateX, Settings.PropertiesCoordinateY + Settings.FontSize * 6, Settings.FontSize);
                graphics.DrawString(ConvertByte(file.Length), Settings.FontName, color, _windowCordinateX + Settings.PropertiesInfoCoordinateX, Settings.PropertiesCoordinateY + Settings.FontSize * 6, Settings.FontSize);
            }

            if (_folderContent[_position] is DirectoryInfo directory)
            {
                var currentDirectory = new DirectoryInfo(directory.FullName);
                var parentDirectory = (directory.Parent.Name == string.Empty) ? directory.Root.Name : directory.Parent.Name;
                var countFiles = currentDirectory.GetFiles(".", SearchOption.AllDirectories).Count();
                var countDirectories = currentDirectory.GetDirectories(".", SearchOption.AllDirectories).Count();
                var sizeFiles = currentDirectory.GetFiles(".", SearchOption.AllDirectories).Sum(f => f.Length);

                graphics.DrawString("Name:", Settings.FontName, color, _windowCordinateX, Settings.PropertiesCoordinateY, Settings.FontSize);
                graphics.DrawString($"{directory.Name}", Settings.FontName, color, _windowCordinateX + Settings.PropertiesInfoCoordinateX, Settings.PropertiesCoordinateY, Settings.FontSize);
                graphics.DrawString("Parent directory:", Settings.FontName, color, _windowCordinateX, Settings.PropertiesCoordinateY + Settings.FontSize, Settings.FontSize);
                graphics.DrawString($"{parentDirectory}", Settings.FontName, color, _windowCordinateX + Settings.PropertiesInfoCoordinateX, Settings.PropertiesCoordinateY + Settings.FontSize, Settings.FontSize);
                graphics.DrawString("Root directory:", Settings.FontName, color, _windowCordinateX, Settings.PropertiesCoordinateY + Settings.FontSize * 2, Settings.FontSize);
                graphics.DrawString($"{directory.Root}", Settings.FontName, color, _windowCordinateX + Settings.PropertiesInfoCoordinateX, Settings.PropertiesCoordinateY + Settings.FontSize * 2, Settings.FontSize);
                graphics.DrawString("Last read time:", Settings.FontName, color, _windowCordinateX, Settings.PropertiesCoordinateY + Settings.FontSize * 4, Settings.FontSize);
                graphics.DrawString($"{directory.LastAccessTime}", Settings.FontName, color, _windowCordinateX + Settings.PropertiesInfoCoordinateX, Settings.PropertiesCoordinateY + Settings.FontSize * 4, Settings.FontSize);
                graphics.DrawString("Last write time:", Settings.FontName, color, _windowCordinateX, Settings.PropertiesCoordinateY + Settings.FontSize * 5, Settings.FontSize);
                graphics.DrawString($"{directory.LastWriteTime}", Settings.FontName, color, _windowCordinateX + Settings.PropertiesInfoCoordinateX, Settings.PropertiesCoordinateY + Settings.FontSize * 5, Settings.FontSize);
                graphics.DrawString("Size:", Settings.FontName, color, _windowCordinateX, Settings.PropertiesCoordinateY + Settings.FontSize * 6, Settings.FontSize);
                graphics.DrawString(ConvertByte(sizeFiles), Settings.FontName, color, _windowCordinateX + Settings.PropertiesInfoCoordinateX, Settings.PropertiesCoordinateY + Settings.FontSize * 6, Settings.FontSize);
                graphics.DrawString("Files:", Settings.FontName, color, _windowCordinateX, Settings.PropertiesCoordinateY + Settings.FontSize * 7, Settings.FontSize);
                graphics.DrawString($"{countFiles}", Settings.FontName, color, _windowCordinateX + Settings.PropertiesInfoCoordinateX, Settings.PropertiesCoordinateY + Settings.FontSize * 7, Settings.FontSize);
                graphics.DrawString("Folders:", Settings.FontName, color, _windowCordinateX, Settings.PropertiesCoordinateY + Settings.FontSize * 8, Settings.FontSize);
                graphics.DrawString($"{countDirectories}", Settings.FontName, color, _windowCordinateX + Settings.PropertiesInfoCoordinateX, Settings.PropertiesCoordinateY + Settings.FontSize * 8, Settings.FontSize);
            }
        }

        private string ConvertByte(long bytes)
        {
            var fileSize = string.Empty;

            if (bytes > Math.Pow(2, 30))
            {
                fileSize = $"{Math.Round(bytes / Math.Pow(2, 30), 2)} GB";
            }
            else if (bytes > Math.Pow(2, 20))
            {
                fileSize = $"{Math.Round(bytes / Math.Pow(2, 20), 2)} MB";
            }
            else if (bytes > Math.Pow(2, 10))
            {
                fileSize = $"{Math.Round(bytes / Math.Pow(2, 10), 2)} KB";
            }
            else
            {
                fileSize = $"{bytes} Byte";
            }

            return fileSize;
        }

        private void MoveToRoot(FileSystemInfo item)
        {
            if(item is FileInfo file)
            {
                _currentPath = file.Directory.Root.Name;
            }

            if (item is DirectoryInfo directory)
            {
                _currentPath = directory.Root.Name;
            }
        }
    }
}

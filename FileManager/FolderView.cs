using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using NConsoleGraphics;
using System.Text;
using System.Threading.Tasks;

namespace FileManager
{
    public class FolderView
    {
        private readonly int _windowCordinateX;
        private readonly List<DriveInfo> _drives;
        private readonly List<FileSystemInfo> _folderContent;
        private int _position;
        private string _currentPath;

        public FolderView(int windowCordinateX)
        {
            _windowCordinateX = windowCordinateX;
            _currentPath = string.Empty;
            _folderContent = new List<FileSystemInfo>();
            _drives = new List<DriveInfo>();
            _drives.AddRange(DriveInfo.GetDrives());
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
                DisplayFolderContent(graphics, color);
            }
        }

        public void Navigate(ref bool isLeftActive, ref bool isRightActive)
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
                    if (isLeftActive)
                    {
                        isLeftActive = false;
                        isRightActive = true;
                    }
                    else if (isRightActive)
                    {
                        isLeftActive = true;
                        isRightActive = false;
                    }
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
                    graphics.FillRectangle(color, _windowCordinateX, coordinateY+5, Settings.WindowWidth, Settings.FontSize);
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
                    graphics.FillRectangle(color, _windowCordinateX, coordinateY+5, Settings.WindowWidth, Settings.FontSize-1);
                    graphics.DrawString($"{_folderContent[i].Name}", Settings.FontName, Settings.BlackColor, _windowCordinateX, coordinateY, Settings.FontSize);

                    if (_folderContent[i] is FileInfo file)
                    {
                        graphics.DrawString($"<{file.Extension}>", Settings.FontName, Settings.BlackColor, _windowCordinateX + Settings.ExtensionCoodrinateX, coordinateY, Settings.FontSize);
                       graphics.DrawString($"{file.Length} Byte", Settings.FontName, Settings.BlackColor, _windowCordinateX + Settings.SizeCoodrinateX, coordinateY, Settings.FontSize);
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
                        graphics.DrawString($"{file.Length} Byte", Settings.FontName, color, _windowCordinateX + Settings.SizeCoodrinateX, coordinateY, Settings.FontSize);
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
                InitCurrentDirectory();
            }

            _position = 0;
        }
    }
}

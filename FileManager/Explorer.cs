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
        private readonly List<DirectoryItem> _folderContent;
        private int _position;
        private string _currentPath;
        private bool _propertiesActive;

        public Explorer(int windowCordinateX)
        {
            _windowCordinateX = windowCordinateX;
            _currentPath = string.Empty;
            _folderContent = new List<DirectoryItem>();
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
                    DirectoryItem.Copy(engine, _folderContent[_position]);
                    //Copy(engine);
                    break;
                case ConsoleKey.F2:
                    DirectoryItem.Cut(engine, _folderContent[_position]);
                    //Cut(engine);
                    break;
                case ConsoleKey.F3:
                    DirectoryItem.Paste(engine, _currentPath);
                    //Paste(engine);
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
                    graphics.FillRectangle(color, _windowCordinateX, coordinateY + 5, Settings.WindowWidth, Settings.FontSize);
                    _folderContent[i].ShowInfo(graphics, Settings.BlackColor, _windowCordinateX, coordinateY);
                }
                else
                {
                    _folderContent[i].ShowInfo(graphics, color, _windowCordinateX, coordinateY);
                }

                coordinateY += Settings.FontSize;
            }
        }

        private void InitCurrentDirectory()
        {
            DirectoryInfo directory = new DirectoryInfo(_currentPath);
            _folderContent.Clear();
            _folderContent.AddRange(directory.GetDirectories().Select(dir => new FolderItem(dir)).Cast<DirectoryItem>().Concat(directory.GetFiles().Select(file => new FileItem(file)).Cast<DirectoryItem>()));
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
                graphics.FillRectangle(Settings.ActiveColor, 500, 400, 400, 80);
                graphics.DrawString("Enter name:", "ISOCPEUR", Settings.BlackColor, 510, 400);
                graphics.DrawRectangle(Settings.BlackColor, 510, 440, 375, 30);
                graphics.DrawString(result, "ISOCPEUR", Settings.BlackColor, 510, 440);
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

        //private void Copy(Engine engine)
        //{
        //    engine.TempItem = _folderContent[_position];
        //    engine.IsCut = false;
        //}

        //private void Paste(Engine engine)
        //{
        //    if (engine.TempItem is FileItem file)
        //    {
        //        if (engine.IsCut)
        //        {
        //            File.Move(file.FullName, $@"{ _currentPath}\\{file.Name}");
        //            engine.TempItem = null;
        //        }
        //        else
        //        {
        //            File.Copy(file.FullName, $@"{ _currentPath}\\{file.Name}");
        //        }
        //    }

        //    if (engine.TempItem is FolderItem directory)
        //    {
        //        if (engine.IsCut)
        //        {
        //            CopyFolder(directory.FullName, $@"{_currentPath}\\{directory.Name}");
        //            Directory.Delete(directory.FullName, true);
        //            engine.TempItem = null;
        //        }
        //        else
        //        {
        //            CopyFolder(directory.FullName, $@"{_currentPath}\\{directory.Name}");
        //        }
        //    }

        //    engine.IsCut = false;
        //}

        //private void Cut(Engine engine)
        //{
        //    engine.TempItem = _folderContent[_position];
        //    engine.IsCut = true;
        //}

        //private void CopyFolder(string sourcePath, string destPath)
        //{
        //    Directory.CreateDirectory(destPath);

        //    foreach (string s1 in Directory.GetFiles(sourcePath))
        //    {
        //        string s2 = destPath + "\\" + Path.GetFileName(s1);
        //        File.Copy(s1, s2);
        //    }
        //    foreach (string s in Directory.GetDirectories(sourcePath))
        //    {
        //        CopyFolder(s, destPath + "\\" + Path.GetFileName(s));
        //    }
        //}
    }
}

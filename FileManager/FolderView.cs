using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManager
{
    public class FolderView
    {
        private int _position;
        private string _currentPath;
        private List<FileSystemInfo> _items;

        public FolderView()
        {
            _currentPath = Settings.DefaultPath;
            _items = new List<FileSystemInfo>();
        }

        public void Show()
        {
            _items.AddRange(Directory.GetDirectories(_currentPath).Select(str => new DirectoryInfo(str)).Cast<FileSystemInfo>().Concat(Directory.GetFiles(_currentPath).Select(str => new FileInfo(str)).Cast<FileSystemInfo>()));

            for (int i = 0; i < _items.Count; i++)
            {
                if (i == _position)
                {
                    DisplaySelectItem(_items[i]);
                }
                else
                {
                    DisplayInfo(_items[i]);
                }

            }
        }

        public void Explorer()
        {
            do
            {
                Show();
                HandleClick();
            }
            while (true);
        }

        public void HandleClick()
        {
            var click = Console.ReadKey();

            if (click.Key == ConsoleKey.DownArrow)
            {
                _position = (_position++ == _items.Count - 1) ? 0 : _position++;
            }
            else if (click.Key == ConsoleKey.UpArrow)
            {
                _position = (_position-- <= 0) ? _items.Count - 1 : _position--;
            }
            else if (_items.Any() && (click.Key == ConsoleKey.Enter) && (_items[_position] is DirectoryInfo))
            {
                InFolder(_items[_position].FullName);
                _position = 0;
            }
            else if (click.Key == ConsoleKey.Escape)
            {
                InFolder("..");
                _position = 0;
            }

            _items.Clear();
            Console.Clear();
        }

        private void InFolder(string folderPath)
        {
            _currentPath = Path.Combine(_currentPath, folderPath);
            _currentPath = new DirectoryInfo(_currentPath).FullName;
        }

        private void DisplaySelectItem(FileSystemInfo systemFile)
        {
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Black;
            DisplayInfo(systemFile);
            Console.ResetColor();
        }

        private void DisplayInfo(FileSystemInfo systemFile)
        {
            if (systemFile is DirectoryInfo directory)
            {
                Console.WriteLine($"{directory.Name}{new string(' ', Settings.NameRowSize - directory.Name.Length + 1)}<{"dir"}>{new string(' ', Settings.TypeRowSize - "dir".Length + 3)}{new string(' ', Settings.SizeRowSize)}");
            }

            if (systemFile is FileInfo file)
            {
                Console.WriteLine($"{file.Name}{new string(' ', Settings.NameRowSize - file.Name.Length + 1)}<{file.Extension}>{new string(' ', Settings.TypeRowSize - file.Extension.Length + 3)}{file.Length} Bytes{new string(' ', Settings.SizeRowSize - (file.Length.ToString().Length+6))}");
            }
        }
    }
}

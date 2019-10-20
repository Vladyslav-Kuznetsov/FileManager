using NConsoleGraphics;
using FileManager.Extensions;
using System;

namespace FileManager.Views
{
    public class SystemItemView
    {
        private ConsoleGraphics _graphics;

        public SystemItemView(ConsoleGraphics graphics)
        {
            _graphics = graphics;
        }

        public void ShowProperties(SystemItem item)
        {
            bool exit = false;

            while(!exit)
            {
                _graphics.FillRectangle(Settings.ActiveColor, Settings.PropertiesCoordinateX, Settings.PropertiesCoordinateY, Settings.PropertiesWidth, Settings.PropertiesHeight);
                _graphics.DrawString("Name:", Settings.FontName, Settings.BlackColor, Settings.PropertiesCoordinateX, Settings.PropertiesCoordinateY, Settings.FontSize);
                _graphics.DrawString(item.Name, Settings.FontName, Settings.BlackColor, Settings.PropertiesCoordinateX + Settings.PropertiesInfoCoordinateX, Settings.PropertiesCoordinateY, Settings.FontSize);
                _graphics.DrawString("Parent directory:", Settings.FontName, Settings.BlackColor, Settings.PropertiesCoordinateX, Settings.PropertiesCoordinateY + Settings.FontSize + 1, Settings.FontSize);
                _graphics.DrawString(item.ParentDirectory.Name, Settings.FontName, Settings.BlackColor, Settings.PropertiesCoordinateX + Settings.PropertiesInfoCoordinateX, Settings.PropertiesCoordinateY + Settings.FontSize + 1, Settings.FontSize);
                _graphics.DrawString("Root directory:", Settings.FontName, Settings.BlackColor, Settings.PropertiesCoordinateX, Settings.PropertiesCoordinateY + (Settings.FontSize * 2) + 1, Settings.FontSize);
                _graphics.DrawString(item.Root, Settings.FontName, Settings.BlackColor, Settings.PropertiesCoordinateX + Settings.PropertiesInfoCoordinateX, Settings.PropertiesCoordinateY + (Settings.FontSize * 2) + 1, Settings.FontSize);
                _graphics.DrawString("Last read time:", Settings.FontName, Settings.BlackColor, Settings.PropertiesCoordinateX, Settings.PropertiesCoordinateY + (Settings.FontSize * 3) + 1, Settings.FontSize);
                _graphics.DrawString($"{item.LastAccessTime}", Settings.FontName, Settings.BlackColor, Settings.PropertiesCoordinateX + Settings.PropertiesInfoCoordinateX, Settings.PropertiesCoordinateY + (Settings.FontSize * 3) + 1, Settings.FontSize);
                _graphics.DrawString("Last write time:", Settings.FontName, Settings.BlackColor, Settings.PropertiesCoordinateX, Settings.PropertiesCoordinateY + (Settings.FontSize * 4) + 1, Settings.FontSize);
                _graphics.DrawString($"{item.LastWriteTime}", Settings.FontName, Settings.BlackColor, Settings.PropertiesCoordinateX + Settings.PropertiesInfoCoordinateX, Settings.PropertiesCoordinateY + (Settings.FontSize * 4) + 1, Settings.FontSize);
                _graphics.DrawString("Size:", Settings.FontName, Settings.BlackColor, Settings.PropertiesCoordinateX, Settings.PropertiesCoordinateY + (Settings.FontSize * 5) + 1, Settings.FontSize);
                _graphics.DrawString(item.Size.ToViewableSize(), Settings.FontName, Settings.BlackColor, Settings.PropertiesCoordinateX + Settings.PropertiesInfoCoordinateX, Settings.PropertiesCoordinateY + (Settings.FontSize * 5) + 1, Settings.FontSize);

                if (item is FileItem file)
                {
                    _graphics.DrawString("Is read only:", Settings.FontName, Settings.BlackColor, Settings.PropertiesCoordinateX, Settings.PropertiesCoordinateY + (Settings.FontSize * 6) + 1, Settings.FontSize);
                    _graphics.DrawString($"{file.IsReadOnly}", Settings.FontName, Settings.BlackColor, Settings.PropertiesCoordinateX + Settings.PropertiesInfoCoordinateX, Settings.PropertiesCoordinateY + (Settings.FontSize * 6) + 1, Settings.FontSize);
                }
                if (item is FolderItem folder)
                {
                    _graphics.DrawString("Files:", Settings.FontName, Settings.BlackColor, Settings.PropertiesCoordinateX, Settings.PropertiesCoordinateY + (Settings.FontSize * 6) + 1, Settings.FontSize);
                    _graphics.DrawString($"{folder.CountFiles}", Settings.FontName, Settings.BlackColor, Settings.PropertiesCoordinateX + Settings.PropertiesInfoCoordinateX, Settings.PropertiesCoordinateY + (Settings.FontSize * 6) + 1, Settings.FontSize);
                    _graphics.DrawString("Folders:", Settings.FontName, Settings.BlackColor, Settings.PropertiesCoordinateX, Settings.PropertiesCoordinateY + (Settings.FontSize * 7) + 1, Settings.FontSize);
                    _graphics.DrawString($"{folder.CountFolders}", Settings.FontName, Settings.BlackColor, Settings.PropertiesCoordinateX + Settings.PropertiesInfoCoordinateX, Settings.PropertiesCoordinateY + (Settings.FontSize * 7) + 1, Settings.FontSize);
                }

                _graphics.DrawString("Press Enter to continue :", Settings.FontName, Settings.BlackColor, Settings.PropertiesCoordinateX, Settings.PropertiesCoordinateY + (Settings.FontSize * 9) + 1, Settings.FontSize + 3);
                _graphics.FlipPages();

                if (Console.ReadKey(true).Key == ConsoleKey.Enter)
                {
                    exit = true;
                }
            }
        }

        public void ShowInfo(SystemItem item, uint color, int coordinateX, int coordinateY)
        {
            if (item is FolderItem folder)
            {
                _graphics.DrawString(folder.Name, Settings.FontName, color, coordinateX, coordinateY, Settings.FontSize);
                _graphics.DrawString("<dir>", Settings.FontName, color, coordinateX + Settings.ExtensionCoodrinateX, coordinateY, Settings.FontSize);
            }

            if (item is FileItem file)
            {
                _graphics.DrawString(file.Name, Settings.FontName, color, coordinateX, coordinateY, Settings.FontSize);
                _graphics.DrawString($"<{file.Extension}>", Settings.FontName, color, coordinateX + Settings.ExtensionCoodrinateX, coordinateY, Settings.FontSize);
                _graphics.DrawString(file.Size.ToViewableSize(), Settings.FontName, color, coordinateX + Settings.SizeCoodrinateX, coordinateY, Settings.FontSize);
            }
        }
    }
}

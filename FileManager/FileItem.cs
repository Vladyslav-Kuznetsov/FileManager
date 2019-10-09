using System;
using System.IO;
using NConsoleGraphics;

namespace FileManager
{
    public class FileItem : DirectoryItem
    {
        //public string Name { get; }
        //public string FullName { get; }
        //public string ParentDirectory { get; }
        //public string Root { get; }
        //public string Extension { get; }
        //public long Size { get; }

        public FileItem(FileInfo file)
        {
            Name = file.Name;
            FullName = file.FullName;
            ParentDirectory = file.Directory;
            Root = file.Directory.Root.Name;
            Size = file.Length;
            Extension = file.Extension;
        }

        public override void Rename(string newName)
        {
            File.Move(FullName, $@"{ParentDirectory.FullName}\{newName}{Extension}");
        }

        public override void ShowInfo(ConsoleGraphics graphics, uint color, int coordinateX, int coordinateY)
        {
            graphics.DrawString(Name, Settings.FontName, color, coordinateX, coordinateY, Settings.FontSize);
            graphics.DrawString($"<{Extension}>", Settings.FontName, color, coordinateX + Settings.ExtensionCoodrinateX, coordinateY, Settings.FontSize);
            graphics.DrawString(ConvertByte(Size), Settings.FontName, color, coordinateX + Settings.SizeCoodrinateX, coordinateY, Settings.FontSize);
        }

        public override void ShowProperties(ConsoleGraphics graphics, int coordinateX)
        {
            FileInfo file = new FileInfo(FullName);

            graphics.DrawRectangle(Settings.ActiveColor, coordinateX, Settings.PropertiesCoordinateY, Settings.WindowWidth, Settings.PropertiesHeight);
            graphics.DrawString("Name:", Settings.FontName, Settings.ActiveColor, coordinateX, Settings.PropertiesCoordinateY, Settings.FontSize);
            graphics.DrawString(Name, Settings.FontName, Settings.ActiveColor, coordinateX + Settings.PropertiesInfoCoordinateX, Settings.PropertiesCoordinateY, Settings.FontSize);
            graphics.DrawString("Parent directory:", Settings.FontName, Settings.ActiveColor, coordinateX, Settings.PropertiesCoordinateY + Settings.FontSize, Settings.FontSize);
            graphics.DrawString(ParentDirectory.Name, Settings.FontName, Settings.ActiveColor, coordinateX + Settings.PropertiesInfoCoordinateX, Settings.PropertiesCoordinateY + Settings.FontSize, Settings.FontSize);
            graphics.DrawString("Root directory:", Settings.FontName, Settings.ActiveColor, coordinateX, Settings.PropertiesCoordinateY + Settings.FontSize * 2, Settings.FontSize);
            graphics.DrawString(Root, Settings.FontName, Settings.ActiveColor, coordinateX + Settings.PropertiesInfoCoordinateX, Settings.PropertiesCoordinateY + Settings.FontSize * 2, Settings.FontSize);
            graphics.DrawString("Is read only:", Settings.FontName, Settings.ActiveColor, coordinateX, Settings.PropertiesCoordinateY + Settings.FontSize * 3, Settings.FontSize);
            graphics.DrawString($"{file.IsReadOnly}", Settings.FontName, Settings.ActiveColor, coordinateX + Settings.PropertiesInfoCoordinateX, Settings.PropertiesCoordinateY + Settings.FontSize * 3, Settings.FontSize);
            graphics.DrawString("Last read time:", Settings.FontName, Settings.ActiveColor, coordinateX, Settings.PropertiesCoordinateY + Settings.FontSize * 4, Settings.FontSize);
            graphics.DrawString($"{file.LastAccessTime}", Settings.FontName, Settings.ActiveColor, coordinateX + Settings.PropertiesInfoCoordinateX, Settings.PropertiesCoordinateY + Settings.FontSize * 4, Settings.FontSize);
            graphics.DrawString("Last write time:", Settings.FontName, Settings.ActiveColor, coordinateX, Settings.PropertiesCoordinateY + Settings.FontSize * 5, Settings.FontSize);
            graphics.DrawString($"{file.LastWriteTime}", Settings.FontName, Settings.ActiveColor, coordinateX + Settings.PropertiesInfoCoordinateX, Settings.PropertiesCoordinateY + Settings.FontSize * 5, Settings.FontSize);
            graphics.DrawString("Size:", Settings.FontName, Settings.ActiveColor, coordinateX, Settings.PropertiesCoordinateY + Settings.FontSize * 6, Settings.FontSize);
            graphics.DrawString(ConvertByte(Size), Settings.FontName, Settings.ActiveColor, coordinateX + Settings.PropertiesInfoCoordinateX, Settings.PropertiesCoordinateY + Settings.FontSize * 6, Settings.FontSize);
        }

        //public string ConvertByte(long bytes)
        //{
        //    var fileSize = string.Empty;

        //    if (bytes > Math.Pow(2, 30))
        //    {
        //        fileSize = $"{Math.Round(bytes / Math.Pow(2, 30), 2)} GB";
        //    }
        //    else if (bytes > Math.Pow(2, 20))
        //    {
        //        fileSize = $"{Math.Round(bytes / Math.Pow(2, 20), 2)} MB";
        //    }
        //    else if (bytes > Math.Pow(2, 10))
        //    {
        //        fileSize = $"{Math.Round(bytes / Math.Pow(2, 10), 2)} KB";
        //    }
        //    else
        //    {
        //        fileSize = $"{bytes} Byte";
        //    }

        //    return fileSize;
        //}
    }
}

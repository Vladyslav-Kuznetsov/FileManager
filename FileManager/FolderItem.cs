using System;
using System.IO;
using System.Linq;
using NConsoleGraphics;

namespace FileManager
{
    public class FolderItem : SystemItem
    {
        public int CountFolders { get; private set; }
        public int CountFiles { get; private set; }

        public FolderItem(DirectoryInfo directory)
        {
            //Name = directory.Name;
            Name = (directory.Name.Length > 45) ? string.Join("", directory.Name.Take(45)) + "..." : directory.Name;
            FullName = directory.FullName;
            ParentDirectory = (directory.Parent.Name == string.Empty) ? directory.Root : directory.Parent;
            Root = directory.Root.Name;
            Extension = "<dir>";
        }

        public override void ShowInfo(ConsoleGraphics graphics, uint color, int coordinateX, int coordinateY)
        {
            graphics.DrawString(Name, Settings.FontName, color, coordinateX, coordinateY, Settings.FontSize);
            graphics.DrawString(Extension, Settings.FontName, color, coordinateX + Settings.ExtensionCoodrinateX, coordinateY, Settings.FontSize);
        }

        public override void ShowProperties(ConsoleGraphics graphics, int coordinateX)
        {
            DirectoryInfo directory = new DirectoryInfo(FullName);

            //Size = directory.GetFiles(".", SearchOption.AllDirectories).Sum(f => f.Length);
            //CountFolders = directory.GetDirectories(".", SearchOption.AllDirectories).Count();
            //CountFiles = directory.GetFiles(".", SearchOption.AllDirectories).Count();

            Size = directory.EnumerateFiles(".", SearchOption.AllDirectories).Where(file => !file.Attributes.HasFlag(FileAttributes.Hidden)).Sum(f => f.Length);
            CountFolders = directory.EnumerateDirectories(".", SearchOption.AllDirectories).Where(dir => !dir.Attributes.HasFlag(FileAttributes.Hidden)).Count();
            CountFiles = directory.EnumerateFiles(".", SearchOption.AllDirectories).Where(file => !file.Attributes.HasFlag(FileAttributes.Hidden)).Count();

            graphics.DrawRectangle(Settings.ActiveColor, coordinateX, Settings.PropertiesCoordinateY, Settings.WindowWidth, Settings.PropertiesHeight);
            graphics.DrawString("Name:", Settings.FontName, Settings.ActiveColor, coordinateX, Settings.PropertiesCoordinateY, Settings.FontSize);
            graphics.DrawString(Name, Settings.FontName, Settings.ActiveColor, coordinateX + Settings.PropertiesInfoCoordinateX, Settings.PropertiesCoordinateY, Settings.FontSize);
            graphics.DrawString("Parent directory:", Settings.FontName, Settings.ActiveColor, coordinateX, Settings.PropertiesCoordinateY + Settings.FontSize + 1, Settings.FontSize);
            graphics.DrawString(ParentDirectory.Name, Settings.FontName, Settings.ActiveColor, coordinateX + Settings.PropertiesInfoCoordinateX, Settings.PropertiesCoordinateY + Settings.FontSize + 1, Settings.FontSize);
            graphics.DrawString("Root directory:", Settings.FontName, Settings.ActiveColor, coordinateX, Settings.PropertiesCoordinateY + (Settings.FontSize * 2) + 1, Settings.FontSize);
            graphics.DrawString(Root, Settings.FontName, Settings.ActiveColor, coordinateX + Settings.PropertiesInfoCoordinateX, Settings.PropertiesCoordinateY + (Settings.FontSize * 2) + 1, Settings.FontSize);
            graphics.DrawString("Last read time:", Settings.FontName, Settings.ActiveColor, coordinateX, Settings.PropertiesCoordinateY + (Settings.FontSize * 3) + 1, Settings.FontSize);
            graphics.DrawString($"{directory.LastAccessTime}", Settings.FontName, Settings.ActiveColor, coordinateX + Settings.PropertiesInfoCoordinateX, Settings.PropertiesCoordinateY + (Settings.FontSize * 3) + 1, Settings.FontSize);
            graphics.DrawString("Last write time:", Settings.FontName, Settings.ActiveColor, coordinateX, Settings.PropertiesCoordinateY + (Settings.FontSize * 4) + 1, Settings.FontSize);
            graphics.DrawString($"{directory.LastWriteTime}", Settings.FontName, Settings.ActiveColor, coordinateX + Settings.PropertiesInfoCoordinateX, Settings.PropertiesCoordinateY + (Settings.FontSize * 4) + 1, Settings.FontSize);
            graphics.DrawString("Size:", Settings.FontName, Settings.ActiveColor, coordinateX, Settings.PropertiesCoordinateY + (Settings.FontSize * 5) + 1, Settings.FontSize);
            graphics.DrawString(ConvertByte(Size), Settings.FontName, Settings.ActiveColor, coordinateX + Settings.PropertiesInfoCoordinateX, Settings.PropertiesCoordinateY + (Settings.FontSize * 5) + 1, Settings.FontSize);
            graphics.DrawString("Files:", Settings.FontName, Settings.ActiveColor, coordinateX, Settings.PropertiesCoordinateY + (Settings.FontSize * 6) + 1, Settings.FontSize);
            graphics.DrawString($"{CountFiles}", Settings.FontName, Settings.ActiveColor, coordinateX + Settings.PropertiesInfoCoordinateX, Settings.PropertiesCoordinateY + (Settings.FontSize * 6) + 1, Settings.FontSize);
            graphics.DrawString("Folders:", Settings.FontName, Settings.ActiveColor, coordinateX, Settings.PropertiesCoordinateY + (Settings.FontSize * 7) + 1, Settings.FontSize);
            graphics.DrawString($"{CountFolders}", Settings.FontName, Settings.ActiveColor, coordinateX + Settings.PropertiesInfoCoordinateX, Settings.PropertiesCoordinateY + (Settings.FontSize * 7) + 1, Settings.FontSize);
        }

        public override void Rename(string newName)
        {
            Directory.Move(FullName, $@"{ParentDirectory.FullName}\{newName}");
        }
    }
}

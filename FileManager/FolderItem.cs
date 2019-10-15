using System.IO;
using System.Linq;
using NConsoleGraphics;

namespace FileManager
{
    public class FolderItem : SystemItem
    {
        public int CountFolders { get; private set; }
        public int CountFiles { get; private set; }

        public FolderItem(DirectoryInfo directory) :
            base((directory.Name.Length > 45) ? string.Join("", directory.Name.Take(45)) + "..." : directory.Name, directory.FullName, (directory.Parent.Name == string.Empty) ? new FolderItem(directory.Root) : new FolderItem(directory.Parent), directory.Root.Name, "<dir>", directory.LastAccessTime, directory.LastWriteTime)
        {
        }

        public override void ShowInfo(ConsoleGraphics graphics, uint color, int coordinateX, int coordinateY)
        {
            graphics.DrawString(Name, Settings.FontName, color, coordinateX, coordinateY, Settings.FontSize);
            graphics.DrawString(Extension, Settings.FontName, color, coordinateX + Settings.ExtensionCoodrinateX, coordinateY, Settings.FontSize);
        }

        public override void ShowProperties(ConsoleGraphics graphics)
        {
            base.ShowProperties(graphics);
            GetPropertiesInfo(FullName);

            graphics.DrawString("Size:", Settings.FontName, Settings.BlackColor, Settings.PropertiesCoordinateX, Settings.PropertiesCoordinateY + (Settings.FontSize * 5) + 1, Settings.FontSize);
            graphics.DrawString(ConvertByte(Size), Settings.FontName, Settings.BlackColor, Settings.PropertiesCoordinateX + Settings.PropertiesInfoCoordinateX, Settings.PropertiesCoordinateY + (Settings.FontSize * 5) + 1, Settings.FontSize);
            graphics.DrawString("Files:", Settings.FontName, Settings.BlackColor, Settings.PropertiesCoordinateX, Settings.PropertiesCoordinateY + (Settings.FontSize * 6) + 1, Settings.FontSize);
            graphics.DrawString($"{CountFiles}", Settings.FontName, Settings.BlackColor, Settings.PropertiesCoordinateX + Settings.PropertiesInfoCoordinateX, Settings.PropertiesCoordinateY + (Settings.FontSize * 6) + 1, Settings.FontSize);
            graphics.DrawString("Folders:", Settings.FontName, Settings.BlackColor, Settings.PropertiesCoordinateX, Settings.PropertiesCoordinateY + (Settings.FontSize * 7) + 1, Settings.FontSize);
            graphics.DrawString($"{CountFolders}", Settings.FontName, Settings.BlackColor, Settings.PropertiesCoordinateX + Settings.PropertiesInfoCoordinateX, Settings.PropertiesCoordinateY + (Settings.FontSize * 7) + 1, Settings.FontSize);
            graphics.FlipPages();
            Message.CloseMessage();
        }

        public override void Rename(string newName)
        {
            Directory.Move(FullName, $@"{ParentDirectory.FullName}\{newName}");
        }

        private void GetPropertiesInfo(string path)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(path);
            var files = directoryInfo.EnumerateFiles().Where(file => !file.Attributes.HasFlag(FileAttributes.Hidden));
            var directories = directoryInfo.EnumerateDirectories().Where(dir => !dir.Attributes.HasFlag(FileAttributes.Hidden) && HasFolderPermission(dir));

            Size += files.Sum(f => f.Length);
            CountFiles += files.Count();
            CountFolders += directories.Count();

            foreach (var d in directories)
            {
                GetPropertiesInfo(d.FullName);
            }
        }
    }
}

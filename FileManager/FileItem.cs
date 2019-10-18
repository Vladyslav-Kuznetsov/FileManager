using System.IO;
using System.Linq;
using NConsoleGraphics;

namespace FileManager
{
    public class FileItem : SystemItem
    {
        public bool IsReadOnly { get; private set; }

        public FileItem(FileInfo file) :
            base((file.Name.Length > 45) ? string.Join("", file.Name.Take(45)) + "..." : file.Name, file.FullName, file.Directory, file.Directory.Root.Name, file.Extension, file.LastAccessTime, file.LastWriteTime)
        {
            Size = file.Length;
            IsReadOnly = file.IsReadOnly;
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

        public override void ShowProperties(ConsoleGraphics graphics)
        {
            base.ShowProperties(graphics);

            graphics.DrawString("Size:", Settings.FontName, Settings.BlackColor, Settings.PropertiesCoordinateX, Settings.PropertiesCoordinateY + (Settings.FontSize * 5) + 1, Settings.FontSize);
            graphics.DrawString(ConvertByte(Size), Settings.FontName, Settings.BlackColor, Settings.PropertiesCoordinateX + Settings.PropertiesInfoCoordinateX, Settings.PropertiesCoordinateY + (Settings.FontSize * 5) + 1, Settings.FontSize);
            graphics.DrawString("Is read only:", Settings.FontName, Settings.BlackColor, Settings.PropertiesCoordinateX, Settings.PropertiesCoordinateY + (Settings.FontSize * 6) + 1, Settings.FontSize);
            graphics.DrawString($"{IsReadOnly}", Settings.FontName, Settings.BlackColor, Settings.PropertiesCoordinateX + Settings.PropertiesInfoCoordinateX, Settings.PropertiesCoordinateY + (Settings.FontSize * 6) + 1, Settings.FontSize);
            graphics.FlipPages();
            //Message.CloseMessage();
        }
    }
}

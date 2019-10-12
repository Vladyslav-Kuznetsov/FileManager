﻿using System;
using System.IO;
using System.Linq;
using NConsoleGraphics;

namespace FileManager
{
    public class FileItem : SystemItem
    {
        public FileItem(FileInfo file)
        {
            Name = (file.Name.Length > 45) ? string.Join("", file.Name.Take(45)) + "..." : file.Name;
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

        public override void ShowProperties(ConsoleGraphics graphics)
        {
            Message.ShowMessage("Property definition process in progress", "Please, wait", graphics);
            FileInfo file = new FileInfo(FullName);
            graphics.FillRectangle(Settings.ActiveColor, Settings.PropertiesCoordinateX, Settings.PropertiesCoordinateY, Settings.PropertiesWidth, Settings.PropertiesHeight);
            graphics.DrawString("Name:", Settings.FontName, Settings.BlackColor, Settings.PropertiesCoordinateX, Settings.PropertiesCoordinateY, Settings.FontSize);
            graphics.DrawString(Name, Settings.FontName, Settings.BlackColor, Settings.PropertiesCoordinateX + Settings.PropertiesInfoCoordinateX, Settings.PropertiesCoordinateY, Settings.FontSize);
            graphics.DrawString("Parent directory:", Settings.FontName, Settings.BlackColor, Settings.PropertiesCoordinateX, Settings.PropertiesCoordinateY + Settings.FontSize + 1, Settings.FontSize);
            graphics.DrawString(ParentDirectory.Name, Settings.FontName, Settings.BlackColor, Settings.PropertiesCoordinateX + Settings.PropertiesInfoCoordinateX, Settings.PropertiesCoordinateY + Settings.FontSize + 1, Settings.FontSize);
            graphics.DrawString("Root directory:", Settings.FontName, Settings.BlackColor, Settings.PropertiesCoordinateX, Settings.PropertiesCoordinateY + (Settings.FontSize * 2) + 1, Settings.FontSize);
            graphics.DrawString(Root, Settings.FontName, Settings.BlackColor, Settings.PropertiesCoordinateX + Settings.PropertiesInfoCoordinateX, Settings.PropertiesCoordinateY + (Settings.FontSize * 2) + 1, Settings.FontSize);
            graphics.DrawString("Is read only:", Settings.FontName, Settings.BlackColor, Settings.PropertiesCoordinateX, Settings.PropertiesCoordinateY + (Settings.FontSize * 3) + 1, Settings.FontSize);
            graphics.DrawString($"{file.IsReadOnly}", Settings.FontName, Settings.BlackColor, Settings.PropertiesCoordinateX + Settings.PropertiesInfoCoordinateX, Settings.PropertiesCoordinateY + (Settings.FontSize * 3) + 1, Settings.FontSize);
            graphics.DrawString("Last read time:", Settings.FontName, Settings.BlackColor, Settings.PropertiesCoordinateX, Settings.PropertiesCoordinateY + (Settings.FontSize * 4) + 1, Settings.FontSize);
            graphics.DrawString($"{file.LastAccessTime}", Settings.FontName, Settings.BlackColor, Settings.PropertiesCoordinateX + Settings.PropertiesInfoCoordinateX, Settings.PropertiesCoordinateY + (Settings.FontSize * 4) + 1, Settings.FontSize);
            graphics.DrawString("Last write time:", Settings.FontName, Settings.BlackColor, Settings.PropertiesCoordinateX, Settings.PropertiesCoordinateY + (Settings.FontSize * 5) + 1, Settings.FontSize);
            graphics.DrawString($"{file.LastWriteTime}", Settings.FontName, Settings.BlackColor, Settings.PropertiesCoordinateX + Settings.PropertiesInfoCoordinateX, Settings.PropertiesCoordinateY + (Settings.FontSize * 5) + 1, Settings.FontSize);
            graphics.DrawString("Size:", Settings.FontName, Settings.BlackColor, Settings.PropertiesCoordinateX, Settings.PropertiesCoordinateY + (Settings.FontSize * 6) + 1, Settings.FontSize);
            graphics.DrawString(ConvertByte(Size), Settings.FontName, Settings.BlackColor, Settings.PropertiesCoordinateX + Settings.PropertiesInfoCoordinateX, Settings.PropertiesCoordinateY + (Settings.FontSize * 6) + 1, Settings.FontSize);
            graphics.DrawString("Press Enter to continue :", Settings.FontName, Settings.BlackColor, Settings.PropertiesCoordinateX, Settings.PropertiesCoordinateY + (Settings.FontSize * 9) + 1, Settings.FontSize + 3);
            graphics.FlipPages();

            Message.CloseMessage();
        }
    }
}
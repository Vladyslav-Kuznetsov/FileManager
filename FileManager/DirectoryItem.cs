using NConsoleGraphics;
using System;
using System.IO;

namespace FileManager
{
    public abstract class DirectoryItem
    {
        public string Name { get; protected set; }
        public string FullName { get; protected set; }
        public DirectoryInfo ParentDirectory { get; protected set; }
        public string Root { get; protected set; }
        public string Extension { get; protected set; }
        public long Size { get; protected set; }

        public abstract void ShowProperties(ConsoleGraphics graphics, int coordinateX);
        public abstract void ShowInfo(ConsoleGraphics graphics, uint color, int coordinateX, int coordinateY);
        public abstract void Rename(string newName);

        public static void Paste(Engine engine, string currentPath)
        {
            if (engine.TempItem is FileItem file)
            {
                if (engine.IsCut)
                {
                    File.Move(file.FullName, $@"{ currentPath}\\{file.Name}");
                    engine.TempItem = null;
                }
                else
                {
                    File.Copy(file.FullName, $@"{ currentPath}\\{file.Name}");
                }
            }

            if (engine.TempItem is FolderItem directory)
            {
                if (engine.IsCut)
                {
                    CopyFolder(directory.FullName, $@"{currentPath}\\{directory.Name}");
                    Directory.Delete(directory.FullName, true);
                    engine.TempItem = null;
                }
                else
                {
                    CopyFolder(directory.FullName, $@"{currentPath}\\{directory.Name}");
                }
            }
        }

        public static void Copy(Engine engine, DirectoryItem item)
        {
            engine.TempItem = item;
            engine.IsCut = false;
        }

        public static void Cut(Engine engine, DirectoryItem item)
        {
            engine.TempItem = item;
            engine.IsCut = true;
        }

        protected string ConvertByte(long bytes)
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

        private static void CopyFolder(string sourcePath, string destPath)
        {
            Directory.CreateDirectory(destPath);

            foreach (string currentFilePath in Directory.GetFiles(sourcePath))
            {
                string resultFilePath = destPath + "\\" + Path.GetFileName(currentFilePath);
                File.Copy(currentFilePath, resultFilePath);
            }
            foreach (string folderPath in Directory.GetDirectories(sourcePath))
            {
                CopyFolder(folderPath, destPath + "\\" + Path.GetFileName(folderPath));
            }
        }
    }
}

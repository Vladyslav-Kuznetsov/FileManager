using NConsoleGraphics;
using System;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;

namespace FileManager
{
    public abstract class SystemItem
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

        public static void Paste(Engine engine, string currentPath, ConsoleGraphics graphics)
        {
            string message = (engine.IsCut) ? "Moving to:" : "Copy to:";
            Message.ShowMessage(message, currentPath, graphics);

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

        public static void Copy(Engine engine, SystemItem item)
        {
            engine.TempItem = item;
            engine.IsCut = false;
        }

        public static void Cut(Engine engine, SystemItem item)
        {
            engine.TempItem = item;
            engine.IsCut = true;
        }

        public static bool HasFolderPermission(string destDir)
        {
            if (string.IsNullOrEmpty(destDir) || !Directory.Exists(destDir)) return false;
            try
            {
                DirectorySecurity security = Directory.GetAccessControl(destDir);
                SecurityIdentifier users = new SecurityIdentifier(WellKnownSidType.BuiltinUsersSid, null);
                foreach (AuthorizationRule rule in security.GetAccessRules(true, true, typeof(SecurityIdentifier)))
                {
                    if (rule.IdentityReference == users)
                    {
                        FileSystemAccessRule rights = ((FileSystemAccessRule)rule);
                        if (rights.AccessControlType == AccessControlType.Allow)
                        {
                            if (rights.FileSystemRights == (rights.FileSystemRights | FileSystemRights.ReadData)) return true;
                        }
                    }
                }
                return false;
            }
            catch
            {
                return false;
            }
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

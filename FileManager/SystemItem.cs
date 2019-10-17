using NConsoleGraphics;
using System;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;

namespace FileManager
{
    public abstract class SystemItem
    {
        public string Name { get; }
        public string FullName { get; }
        public DirectoryInfo ParentDirectory { get; }
        public string Root { get; }
        public string Extension { get; }
        public DateTime LastAccessTime { get; }
        public DateTime LastWriteTime { get; }
        public long Size { get; protected set; }

        public SystemItem(string name, string fullName, DirectoryInfo parentDirectory, string root, string extension, DateTime lastAccessTime, DateTime lastWriteTime)
        {
            Name = name;
            FullName = fullName;
            ParentDirectory = parentDirectory;
            Root = root;
            Extension = extension;
            LastAccessTime = lastAccessTime;
            LastWriteTime = lastWriteTime;
        }

        public abstract void ShowInfo(ConsoleGraphics graphics, uint color, int coordinateX, int coordinateY);
        public abstract void Rename(string newName);

        public virtual void ShowProperties(ConsoleGraphics graphics)
        {
            Message.ShowMessage("Property definition process in progress", "Please, wait", graphics);

            graphics.FillRectangle(Settings.ActiveColor, Settings.PropertiesCoordinateX, Settings.PropertiesCoordinateY, Settings.PropertiesWidth, Settings.PropertiesHeight);
            graphics.DrawString("Name:", Settings.FontName, Settings.BlackColor, Settings.PropertiesCoordinateX, Settings.PropertiesCoordinateY, Settings.FontSize);
            graphics.DrawString(Name, Settings.FontName, Settings.BlackColor, Settings.PropertiesCoordinateX + Settings.PropertiesInfoCoordinateX, Settings.PropertiesCoordinateY, Settings.FontSize);
            graphics.DrawString("Parent directory:", Settings.FontName, Settings.BlackColor, Settings.PropertiesCoordinateX, Settings.PropertiesCoordinateY + Settings.FontSize + 1, Settings.FontSize);
            graphics.DrawString(ParentDirectory.Name, Settings.FontName, Settings.BlackColor, Settings.PropertiesCoordinateX + Settings.PropertiesInfoCoordinateX, Settings.PropertiesCoordinateY + Settings.FontSize + 1, Settings.FontSize);
            graphics.DrawString("Root directory:", Settings.FontName, Settings.BlackColor, Settings.PropertiesCoordinateX, Settings.PropertiesCoordinateY + (Settings.FontSize * 2) + 1, Settings.FontSize);
            graphics.DrawString(Root, Settings.FontName, Settings.BlackColor, Settings.PropertiesCoordinateX + Settings.PropertiesInfoCoordinateX, Settings.PropertiesCoordinateY + (Settings.FontSize * 2) + 1, Settings.FontSize);
            graphics.DrawString("Last read time:", Settings.FontName, Settings.BlackColor, Settings.PropertiesCoordinateX, Settings.PropertiesCoordinateY + (Settings.FontSize * 3) + 1, Settings.FontSize);
            graphics.DrawString($"{LastAccessTime}", Settings.FontName, Settings.BlackColor, Settings.PropertiesCoordinateX + Settings.PropertiesInfoCoordinateX, Settings.PropertiesCoordinateY + (Settings.FontSize * 3) + 1, Settings.FontSize);
            graphics.DrawString("Last write time:", Settings.FontName, Settings.BlackColor, Settings.PropertiesCoordinateX, Settings.PropertiesCoordinateY + (Settings.FontSize * 4) + 1, Settings.FontSize);
            graphics.DrawString($"{LastWriteTime}", Settings.FontName, Settings.BlackColor, Settings.PropertiesCoordinateX + Settings.PropertiesInfoCoordinateX, Settings.PropertiesCoordinateY + (Settings.FontSize * 4) + 1, Settings.FontSize);
            graphics.DrawString("Press Enter to continue :", Settings.FontName, Settings.BlackColor, Settings.PropertiesCoordinateX, Settings.PropertiesCoordinateY + (Settings.FontSize * 9) + 1, Settings.FontSize + 3);
        }

        //public static void Paste(Engine engine, string currentPath, ConsoleGraphics graphics)
        //{
        //    string message = (engine.IsCut) ? "Moving to:" : "Copy to:";
        //    Message.ShowMessage(message, currentPath, graphics);

        //    if (engine.TempItem is FileItem file)
        //    {
        //        try
        //        {
        //            if (engine.IsCut)
        //            {
        //                File.Move(file.FullName, $@"{ currentPath}\{file.Name}");
        //                engine.TempItem = null;
        //            }
        //            else
        //            {
        //                File.Copy(file.FullName, $@"{ currentPath}\{file.Name}");
        //            }
        //        }
        //        catch (IOException)
        //        {
        //            Message.ShowMessage("A file with the same name already exists.", "Press Enter to continue", graphics);
        //            Message.CloseMessage();
        //        }
        //    }

        //    if (engine.TempItem is FolderItem directory)
        //    {
        //        try
        //        {
        //            if (engine.IsCut)
        //            {
        //                CopyFolder(directory.FullName, $@"{currentPath}\{directory.Name}");
        //                Directory.Delete(directory.FullName, true);
        //                engine.TempItem = null;
        //            }
        //            else
        //            {
        //                CopyFolder(directory.FullName, $@"{currentPath}\{directory.Name}");
        //            }
        //        }
        //        catch (IOException)
        //        {
        //            Message.ShowMessage("A folder with the same name already exists.", "Press Enter to continue", graphics);
        //            Message.CloseMessage();
        //        }
        //    }
        //}

        //public static void Copy(Engine engine, SystemItem item)
        //{
        //    engine.TempItem = item;
        //    engine.IsCut = false;
        //}

        //public static void Cut(Engine engine, SystemItem item)
        //{
        //    engine.TempItem = item;
        //    engine.IsCut = true;
        //}

        public static bool HasFolderPermission(DirectoryInfo directoryInfo)
        {
            try
            {
                DirectorySecurity security = directoryInfo.GetAccessControl();
                SecurityIdentifier users = new SecurityIdentifier(WellKnownSidType.BuiltinUsersSid, null);

                foreach (AuthorizationRule rule in security.GetAccessRules(true, true, typeof(SecurityIdentifier)))
                {
                    if (rule.IdentityReference == users)
                    {
                        FileSystemAccessRule rights = (FileSystemAccessRule)rule;

                        if (rights.AccessControlType == AccessControlType.Allow)
                        {
                            if (rights.FileSystemRights == (rights.FileSystemRights | FileSystemRights.ReadData))
                            {
                                return true;
                            }
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

        //private static void CopyFolder(string sourcePath, string destPath)
        //{
        //    Directory.CreateDirectory(destPath);

        //    foreach (string currentFilePath in Directory.GetFiles(sourcePath))
        //    {
        //        string resultFilePath = destPath + "\\" + Path.GetFileName(currentFilePath);
        //        File.Copy(currentFilePath, resultFilePath);
        //    }

        //    foreach (string folderPath in Directory.GetDirectories(sourcePath))
        //    {
        //        CopyFolder(folderPath, destPath + "\\" + Path.GetFileName(folderPath));
        //    }
        //}
    }
}

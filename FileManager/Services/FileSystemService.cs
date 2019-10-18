using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace FileManager.Services
{
    public class FileSystemService
    {
        //public void Paste(string currentPath)
        //{
        //    if (_clipboard.TempItem is FileItem file)
        //    {
        //        try
        //        {
        //            if (_clipboard.IsCut)
        //            {
        //                File.Move(file.FullName, $@"{ currentPath}\{file.Name}");
        //                _clipboard.TempItem = null;
        //            }
        //            else
        //            {
        //                File.Copy(file.FullName, $@"{ currentPath}\{file.Name}");
        //            }
        //        }
        //        catch (IOException)
        //        {

        //        }
        //    }

        //    if (_clipboard.TempItem is FolderItem directory)
        //    {
        //        try
        //        {
        //            if (_clipboard.IsCut)
        //            {
        //                CopyFolder(directory.FullName, $@"{currentPath}\{directory.Name}");
        //                Directory.Delete(directory.FullName, true);
        //                _clipboard.TempItem = null;
        //            }
        //            else
        //            {
        //                CopyFolder(directory.FullName, $@"{currentPath}\{directory.Name}");
        //            }
        //        }
        //        catch (IOException)
        //        {

        //        }
        //    }

        public IEnumerable<SystemItem> GetFolderContent(string path)
        {
            DirectoryInfo directory = new DirectoryInfo(path);

            var folders = directory.EnumerateDirectories().Where(dir => !dir.Attributes.HasFlag(FileAttributes.Hidden) && HasFolderPermission(dir)).Select(dir => new FolderItem(dir)).Cast<SystemItem>();
            var files = directory.EnumerateFiles().Where(file => !file.Attributes.HasFlag(FileAttributes.Hidden)).Select(file => new FileItem(file)).Cast<SystemItem>();

            return folders.Concat(files);
        }

        public void Copy(string sourcePath, string destPath)
        {
            if (sourcePath.Last() == '\\')
            {
                CopyFolder(sourcePath, destPath);
            }
            else
            {
                File.Copy(sourcePath, destPath);
            }
        }

        public void Move(string sourcePath, string destPath)
        {
            if (sourcePath.Last() == '\\')
            {
                CopyFolder(sourcePath, destPath);
                Directory.Delete(sourcePath, true);
            }
            else
            {
                File.Move(sourcePath, destPath);
            }
        }

        public void Rename(string path, string newName)
        {
            string[] arrayStr = path.Split('\\');
            string destPath = string.Empty;

            if (path.Last() == '\\')
            {
                arrayStr[arrayStr.Length - 2] = newName;
                destPath = string.Join("\\", arrayStr);
                Directory.Move(path, destPath);
            }
            else
            {
                arrayStr[arrayStr.Length - 1] = newName;
                destPath = string.Join("\\", arrayStr);
                File.Move(path, destPath);
            }
        }

        public void CreateNewFolder(string path,string nameFolder)
        {
            Directory.CreateDirectory($@"{path}\{nameFolder}");
        }

        private void CopyFolder(string sourcePath, string destPath)
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

        private static bool HasFolderPermission(DirectoryInfo directoryInfo)
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
    }
}

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

        public IEnumerable<SystemItem> GetFolderContent(string path)
        {
            return GetFolders(path).Cast<SystemItem>().Concat(GetFiles(path).Cast<SystemItem>());
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

        public void CreateNewFolder(string path, string nameFolder)
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

        public (long size, int countFolder, int countFiles) GetFolderProperties(string path)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(path);
            var files = directoryInfo.EnumerateFiles().Where(file => !file.Attributes.HasFlag(FileAttributes.Hidden));
            var directories = directoryInfo.EnumerateDirectories().Where(dir => !dir.Attributes.HasFlag(FileAttributes.Hidden) && HasFolderPermission(dir));

            long size = files.Sum(f => f.Length);
            int countFiles = files.Count();
            int countFolders = directories.Count();

            foreach (var d in directories)
            {
                var result = GetFolderProperties(d.FullName);
                size += result.size;
                countFiles += result.countFiles;
                countFolders += result.countFolder;
            }

            return (size, countFolders, countFiles);
        }

        public (string path, int position) FindFileByName(string name, string currentPath)
        {
            DirectoryInfo directory = new DirectoryInfo(currentPath);
            


            if (HasFolderPermission(directory))
            {
                foreach (var file in GetFiles(directory.FullName))
                {
                    if (file.Name.ToLower().Contains(name))
                    {
                        var folderContent = GetFolderContent(directory.FullName).ToList();
                        return (directory.FullName, folderContent.FindIndex(s => s.FullName == file.FullName));
                    }
                }

                foreach (var dir in GetFolders(directory.FullName))
                {
                    var result = FindFileByName(name, dir.FullName);

                    if (result.position != -1)
                    {
                        return result;
                    }
                }
            }

            return (string.Empty, -1);
        }

        private IEnumerable<FileItem> GetFiles(string path)
        {
            DirectoryInfo directory = new DirectoryInfo(path);
            return directory.EnumerateFiles().Where(file => !file.Attributes.HasFlag(FileAttributes.Hidden)).Select(file => new FileItem(file));
        }
        private IEnumerable<FolderItem> GetFolders(string path)
        {
            DirectoryInfo directory = new DirectoryInfo(path);
            return directory.EnumerateDirectories().Where(dir => !dir.Attributes.HasFlag(FileAttributes.Hidden) && HasFolderPermission(dir)).Select(dir => new FolderItem(dir));

        }

        private bool HasFolderPermission(DirectoryInfo directoryInfo)
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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FileManager.Services
{
    public class FileSystemService
    {
        public event Action<int, string, int> FileFound;
        public event Action AccessErrorOccurred;

        public IEnumerable<SystemItem> GetFolderContent(string path)
        {
            try
            {
                return GetFolders(path).Cast<SystemItem>().Concat(GetFiles(path).Cast<SystemItem>());
            }
            catch (ArgumentNullException)
            {
                Console.Beep();

                AccessErrorOccurred?.Invoke();
                return null;
            }
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
            string destPath;

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

        public void CreateNewFolder(string path)
        {
            Directory.CreateDirectory(path);
        }

        public (long size, int countFolder, int countFiles) GetFolderProperties(string path)
        {
            var files = GetFiles(path);
            var directories = GetFolders(path);
            long size = 0;
            int countFiles = 0;
            int countFolders = 0;

            if (files != null & directories != null)
            {
                size = files.Sum(f => f.Size);
                countFiles = files.Count();
                countFolders = directories.Count();


                foreach (var d in directories)
                {
                    var result = GetFolderProperties(d.FullName);
                    size += result.size;
                    countFiles += result.countFiles;
                    countFolders += result.countFolder;
                }
            }

            return (size, countFolders, countFiles);
        }

        public bool FindFileByName(string name, string currentPath)
        {
            var files = GetFiles(currentPath);
            
            if(files != null)
            {
                foreach (var file in files)
                {
                    if (file.Name.ToLower().Contains(name))
                    {
                        var folderContent = GetFolderContent(currentPath).ToList();
                        FileFound?.Invoke(folderContent.FindIndex(s => s.FullName.ToLower().Contains(name.ToLower())), currentPath, folderContent.Count);
                        return true;
                    }
                }
            }

            var folders = GetFolders(currentPath);

            if (folders != null)
            {
                foreach (var folder in folders)
                {
                    var result = FindFileByName(name, folder.FullName);

                    if (result)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool Exists(string path)
        {
            if (path.Last() == '\\')
            {
                return Directory.Exists(path);
            }
            else
            {
                return File.Exists(path);
            }
        }

        private IEnumerable<FileItem> GetFiles(string path)
        {
            DirectoryInfo directory = new DirectoryInfo(path);

            try
            {
                return directory.EnumerateFiles().Select(file => new FileItem(file));
            }
            catch (UnauthorizedAccessException)
            {
                return null;
            }
        }
        private IEnumerable<FolderItem> GetFolders(string path)
        {
            DirectoryInfo directory = new DirectoryInfo(path);

            try
            {
                return directory.EnumerateDirectories().Select(dir => new FolderItem(dir));
            }
            catch (UnauthorizedAccessException)
            {
                return null;
            }
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
    }
}

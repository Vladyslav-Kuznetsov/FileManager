using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManager
{
    public class SystemService
    {
        private Buffer _buffer = new Buffer();

        public void Paste(string currentPath)
        {
            if (_buffer.TempItem is FileItem file)
            {
                try
                {
                    if (_buffer.IsCut)
                    {
                        File.Move(file.FullName, $@"{ currentPath}\{file.Name}");
                        _buffer.TempItem = null;
                    }
                    else
                    {
                        File.Copy(file.FullName, $@"{ currentPath}\{file.Name}");
                    }
                }
                catch (IOException)
                {

                }
            }

            if (_buffer.TempItem is FolderItem directory)
            {
                try
                {
                    if (_buffer.IsCut)
                    {
                        CopyFolder(directory.FullName, $@"{currentPath}\{directory.Name}");
                        Directory.Delete(directory.FullName, true);
                        _buffer.TempItem = null;
                    }
                    else
                    {
                        CopyFolder(directory.FullName, $@"{currentPath}\{directory.Name}");
                    }
                }
                catch (IOException)
                {

                }
            }
        }

        public void Copy(SystemItem item)
        {
            _buffer.TempItem = item;
            _buffer.IsCut = false;
        }

        public void Cut(SystemItem item)
        {
            _buffer.TempItem = item;
            _buffer.IsCut = true;
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

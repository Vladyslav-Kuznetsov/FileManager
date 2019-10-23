using System.IO;
using System.Linq;
using NConsoleGraphics;

namespace FileManager
{
    public class FileItem : SystemItem
    {
        public FileItem(FileInfo file) :
            base(file.Name, file.FullName,  file.Directory.Name, file.Directory.Root.Name, file.LastAccessTime, file.LastWriteTime)
        {
            Extension = file.Extension;
            Size = file.Length;
            IsReadOnly = file.IsReadOnly;
        }

        public string Extension { get; private set; }
        public bool IsReadOnly { get; private set; }
    }
}

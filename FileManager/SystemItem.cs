using System;
using System.IO;
using System.Linq;

namespace FileManager
{
    public abstract class SystemItem
    {
        public SystemItem(string name, string fullName, string parentDirectory, string root, DateTime lastAccessTime, DateTime lastWriteTime)
        {
            Name = (name.Length > 45) ? string.Join("", name.Take(45)) + "..." : name;
            FullName = fullName;
            ParentDirectory = parentDirectory;
            Root = root;
            LastAccessTime = lastAccessTime;
            LastWriteTime = lastWriteTime;
        }

        public string Name { get; }
        public string FullName { get; }
        //public DirectoryInfo ParentDirectory { get; }
        public string ParentDirectory { get; }
        public string Root { get; }
        public DateTime LastAccessTime { get; }
        public DateTime LastWriteTime { get; }
        public long Size { get; protected set; }
    }
}

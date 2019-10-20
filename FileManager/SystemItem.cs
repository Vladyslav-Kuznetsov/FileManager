using System;
using System.IO;

namespace FileManager
{
    public abstract class SystemItem
    {
        public SystemItem(string name, string fullName, DirectoryInfo parentDirectory, string root, DateTime lastAccessTime, DateTime lastWriteTime)
        {
            Name = name;
            FullName = fullName;
            ParentDirectory = parentDirectory;
            Root = root;
            LastAccessTime = lastAccessTime;
            LastWriteTime = lastWriteTime;
        }

        public string Name { get; }
        public string FullName { get; }
        public DirectoryInfo ParentDirectory { get; }
        public string Root { get; }
        public DateTime LastAccessTime { get; }
        public DateTime LastWriteTime { get; }
        public long Size { get; protected set; }
    }
}

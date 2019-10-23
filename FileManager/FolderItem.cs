using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FileManager
{
    public class FolderItem : SystemItem
    {
        public FolderItem(DirectoryInfo directory) :
            base(directory.Name, directory.FullName, (directory.Parent.Name == string.Empty) ? directory.Root.Name : directory.Parent.Name, directory.Root.Name, directory.LastAccessTime, directory.LastWriteTime)
        {

        }

        public FolderItem(FolderItem folder, long size, int countFolders, int countFiles) :
            base(folder.Name, folder.FullName, folder.ParentDirectory, folder.Root, folder.LastAccessTime, folder.LastWriteTime)
        {
            Size = size;
            CountFolders = countFolders;
            CountFiles = countFiles;
        }

        public int CountFolders { get; private set; }
        public int CountFiles { get; private set; }
    }
}

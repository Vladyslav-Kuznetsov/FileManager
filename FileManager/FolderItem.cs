using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FileManager
{
    public class FolderItem : SystemItem
    {
        public FolderItem(DirectoryInfo directory) :
            base((directory.Name.Length > 45) ? string.Join("", directory.Name.Take(45)) + "..." : directory.Name, directory.FullName, (directory.Parent.Name == string.Empty) ? directory.Root : directory.Parent, directory.Root.Name, directory.LastAccessTime, directory.LastWriteTime)
        {
            
        }

        public int CountFolders { get; private set; }
        public int CountFiles { get; private set; }
    }
}

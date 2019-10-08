using NConsoleGraphics;

namespace FileManager
{
    public interface IDirectoryItem
    {
        string Name { get; }
        string FullName { get; }
        string ParentDirectory { get; }
        string Root { get; }
        string Extension { get; }
        long Size { get; }

        void ShowProperties(ConsoleGraphics graphics, int coordinateX);
        void ShowInfo(ConsoleGraphics graphics, uint color, int coordinateX, int coordinateY);
        string ConvertByte(long bytes);
    }
}

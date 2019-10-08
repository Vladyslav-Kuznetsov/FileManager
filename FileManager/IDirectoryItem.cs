using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NConsoleGraphics;

namespace FileManager
{
    interface IDirectoryItem
    {
        string Name { get; }
        string FullName { get; }
        string Parent { get; }
        string Root { get; }
        void ShowProperties(ConsoleGraphics graphics, int coordinateX);
        void ShowInfo(ConsoleGraphics graphics, uint color, int coordinateX);
    }
}

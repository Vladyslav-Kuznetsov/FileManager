using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using NConsoleGraphics;


namespace FileManager
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.CursorVisible = false;
            Console.WindowHeight = Console.LargestWindowHeight;
            Console.WindowWidth = Console.LargestWindowWidth;
            //ConsoleGraphics g = new ConsoleGraphics();
            //while (true)
            //{
            //    g.FillRectangle(0xFF000000, 0, 0, g.ClientWidth, g.ClientHeight);
            //    g.DrawRectangle(0xFFFFFFFF, 10, 50, 900, 700);
            //    g.DrawString("Hello", "ISOCPEUR", 0xFF756d6c, 10, 50, 12);
            //    g.DrawString("Hello", "ISOCPEUR", 0xFFFFFFFF, 10, 62, 12);
            //    g.DrawRectangle(0xFFFFFFFF, 960, 50, 900, 700);
            //    g.FlipPages();
            //}
            Window window = new Window();
            window.Explorer();
            
            //FolderView f = new FolderView();
            //f.Explorer();
            Console.ReadLine();
        }
    }
}

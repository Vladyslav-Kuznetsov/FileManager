using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;


namespace FileManager
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.CursorVisible = false;
            FolderView f = new FolderView();
            f.Explorer();
            
            
            //List<string> str = new List<string>()
            //{
            //    "Name",
            //    "Type",
            //    "Extention",
            //    "Size"
            //};

            //int left = 0;
            //int top = 0;

            //for (int i = 0; i < 4; i++, top++)
            //{
            //    for (int j = 0; j < str.Count; j++, left+= str[j-1].Length+1)
            //    {
            //        Console.SetCursorPosition(left, top);
            //        Console.WriteLine(str[j]);
            //    }
            //    left = 0;
            //}

            Console.ReadLine();
        }
    }
}

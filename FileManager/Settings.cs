using System;

namespace FileManager
{
    public static class Settings
    {
        public const int ExtensionCoodrinateX = 350;
        public const int SizeCoodrinateX = 500;
        public const int LeftWindowCoordinateX = 10;
        public const int RigthWindowCoordinateX = 700;
        public const int WindowCoordinateY = 50;
        public const int PropertiesCoordinateY = 660;
        public const int PropertiesInfoCoordinateX = 300;
        public const int PropertiesHeight = 120;
        public const int WindowWidth = 620;
        public const int WindowHeight = 600;
        public const uint ActiveColor = 0xFFFFFFFF;
        public const uint InactiveColor = 0xFF756d6c;
        public const uint BlackColor = 0xFF000000;
        public const int FontSize = 12;
        public const string FontName = "ISOCPEUR";

        public static void SetupDefaultConsoleSettings()
        {
            Console.CursorVisible = false;
            Console.WindowHeight = 55;
            Console.WindowWidth = 170;
        }
    }
}

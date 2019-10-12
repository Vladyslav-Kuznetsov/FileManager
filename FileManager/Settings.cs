﻿using System;

namespace FileManager
{
    public static class Settings
    {
        public const int ExtensionCoodrinateX = 450;
        public const int SizeCoodrinateX = 520;
        public const int LeftWindowCoordinateX = 10;
        public const int RigthWindowCoordinateX = LeftWindowCoordinateX + WindowWidth + 10;
        public const int WindowCoordinateY = 10;
        public const int WindowWidth = 620;
        public const int WindowHeight = 526;
        public const int PropertiesCoordinateX = WindowWidth + 5 - PropertiesWidth / 2;
        public const int PropertiesCoordinateY = WindowHeight / 2 - PropertiesHeight / 2;
        public const int PropertiesInfoCoordinateX = 220;
        public const int PropertiesWidth = 600;
        public const int PropertiesHeight = 140;
        public const int MessageWindowCoordinateX = WindowWidth + 5 - MessageWindowWidth / 2;
        public const int MessageWindowCoordinateY = WindowHeight / 2 - MessageWindowHeiht / 2;
        public const int MessageWindowWidth = 600;
        public const int MessageWindowHeiht = 80;
        public const uint ActiveColor = 0xFFFFFFFF;
        public const uint InactiveColor = 0xFF756d6c;
        public const uint BlackColor = 0xFF000000;
        public const uint HintsColor = 0xFF9fa818;
        public const int HintsCoordinateY = WindowCoordinateY + WindowHeight + 10;
        public const int HintsWidth = WindowWidth * 2 + 10;
        public const int HintsHeight = 35;
        public const int FontSize = 12;
        public const string FontName = "ISOCPEUR";
        public const string Hints = "F1 - copy | F2 - cut | F3 - paste | F4 - root | F5 - list of disks | F6 - properties | F7 - rename | F8 - find | F9 - new folder";

        public static void SetupDefaultConsoleSettings()
        {
            Console.CursorVisible = false;
            Console.WindowHeight = 37;
            Console.WindowWidth = 157;
            Console.SetBufferSize(Console.WindowWidth, Console.WindowHeight);
        }
    }
}

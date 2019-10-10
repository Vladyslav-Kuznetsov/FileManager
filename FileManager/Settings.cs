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
        public const int PropertiesCoordinateY = WindowCoordinateY + WindowHeight + 10;
        public const int PropertiesInfoCoordinateX = 300;
        public const int PropertiesHeight = 120;
        public const int InputWindowCoordinateX = WindowWidth + 5 - InputWindowWidth / 2;
        public const int InputWindowCoordinateY = WindowHeight / 2 - InputWindowHeiht / 2;
        public const int InputWindowWidth = 400;
        public const int InputWindowHeiht = 80;
        public const int InputFieldWidth = InputWindowWidth - 25;
        public const int InputFieldHeiht = 30;

        public const int WindowWidth = 620;
        public const int WindowHeight = 526;
        public const uint ActiveColor = 0xFFFFFFFF;
        public const uint InactiveColor = 0xFF756d6c;
        public const uint BlackColor = 0xFF000000;
        public const uint HintsColor = 0xFF9fa818;
        public const int HintsCoordinateY = PropertiesCoordinateY + PropertiesHeight + 10;
        public const int HintsWidth = WindowWidth * 2 + 10;
        public const int HintsHeight = 35;
        public const int FontSize = 12;
        public const string FontName = "ISOCPEUR";
        public const string Hints = "F1 - copy | F2 - cut | F3 - paste | F4 - root | F5 - list of disks | F6 - properties | F7 - rename | F8 - find | F9 - new folder";

        public static void SetupDefaultConsoleSettings()
        {
            Console.CursorVisible = false;
            Console.WindowHeight = 45;
            Console.WindowWidth = 157;
            Console.SetBufferSize(Console.WindowWidth, Console.WindowHeight);
        }
    }
}

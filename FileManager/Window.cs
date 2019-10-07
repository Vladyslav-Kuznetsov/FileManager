﻿using NConsoleGraphics;

namespace FileManager
{
    public class Window
    {
        private readonly FolderView _left;
        private readonly FolderView _rigth;
        private readonly ConsoleGraphics _graphics;
        public bool IsLeftActive { get; set; }
        public bool IsRightActive { get; set; }
        public bool Exit { get; set; }

        public Window()
        {
            _left = new FolderView(Settings.LeftWindowCoordinateX);
            _rigth = new FolderView(Settings.RigthWindowCoordinateX);
            IsLeftActive = true;
            IsRightActive = false;
            _graphics = new ConsoleGraphics();
            Exit = false;
        }

        public void Explorer()
        {
            while (!Exit)
            {
                _graphics.FillRectangle(Settings.BlackColor, 0, 0, _graphics.ClientWidth, _graphics.ClientHeight);
                _left.Show(_graphics, IsLeftActive);
                _rigth.Show(_graphics, IsRightActive);
                _graphics.FlipPages();

                if (IsLeftActive)
                {
                    _left.Navigate(this);
                }
                else
                {
                    _rigth.Navigate(this);
                }
            }
        }
    }
}

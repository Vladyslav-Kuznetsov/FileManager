using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using NConsoleGraphics;

namespace FileManager
{
    public class Window
    {
        private FolderView _left;
        private FolderView _rigth;
        private bool _isLeftActive;
        private bool _isRightActive;
        private bool _exit;
        private readonly ConsoleGraphics _graphics;

        public Window()
        {
            _left = new FolderView(Settings.LeftWindowCoordinateX);
            _rigth = new FolderView(Settings.RigthWindowCoordinateX);
            _isLeftActive = true;
            _isRightActive = false;
            _graphics = new ConsoleGraphics();
            _exit = false;
        }

        public void Explorer()
        {
            do
            {
                _graphics.FillRectangle(Settings.BlackColor, 0, 0, _graphics.ClientWidth, _graphics.ClientHeight);
                _left.Show(_graphics,_isLeftActive );
                _rigth.Show(_graphics, _isRightActive);
                _graphics.FlipPages();

                if (_isLeftActive)
                {
                    _left.Navigate(ref _isLeftActive,ref _isRightActive);
                }
                else
                {
                    _rigth.Navigate(ref _isLeftActive, ref _isRightActive);
                }

            }
            while (!_exit);
        }
    }
}

using NConsoleGraphics;
using System.Collections.Generic;

namespace FileManager.Views
{
    public class TabView
    {
        private readonly ConsoleGraphics _graphics;
        private readonly IEnumerable<Tab> _tabs;
        private readonly SystemItemView _systemItemView;

        public TabView(IEnumerable<Tab> tabs, ConsoleGraphics graphics, SystemItemView systemItemView)
        {
            _tabs = tabs;
            _graphics = graphics;
            _systemItemView = systemItemView;
        }

        public void Show()
        {
            foreach(var tab in _tabs)
            {
                ShowTab(tab);
            }
        }

        private void ShowTab(Tab tab)
        {
            uint color = (tab.IsActive) ? Settings.ActiveColor : Settings.InactiveColor;
            _graphics.DrawRectangle(color, tab.CoordinateX, tab.CoordinateY, Settings.TabWidth, Settings.TabHeight);

            if (tab.CurrentPath == string.Empty)
            {
                DisplayDrives(color, tab);
            }
            else
            {
                tab.InitCurrentDirectory();
                tab.CheckPosition();
                DisplayFolderContent(color,tab);
            }
        }

        private void DisplayDrives(uint color, Tab tab)
        {
            int textCoordinateY = tab.CoordinateY;
            var listItem = tab.GetListDrives();

            for (int i = 0; i < listItem.Count; i++)
            {
                if (i == tab.Position)
                {
                    _graphics.FillRectangle(color, tab.CoordinateX, textCoordinateY + 5, Settings.TabWidth, Settings.FontSize);
                    _graphics.DrawString($"{listItem[i].Name}", Settings.FontName, Settings.BlackColor, tab.CoordinateX, textCoordinateY, Settings.FontSize);
                }
                else
                {
                    _graphics.DrawString($"{listItem[i].Name}", Settings.FontName, color, tab.CoordinateX, textCoordinateY, Settings.FontSize);
                }

                textCoordinateY += Settings.FontSize + 1;
            }
        }

        private void DisplayFolderContent(uint color, Tab tab)
        {
            int textCoordinateY = tab.CoordinateY;
            var listItem = tab.GetFolderContent();

            for (int i = tab.StartPosition; i < tab.EndPosition; i++)
            {
                if (i == tab.Position)
                {
                    _graphics.FillRectangle(color, tab.CoordinateX, textCoordinateY + 5, Settings.TabWidth, Settings.FontSize);
                    _systemItemView.ShowInfo(listItem[i], Settings.BlackColor, tab.CoordinateX, textCoordinateY);
                }
                else
                {
                    _systemItemView.ShowInfo(listItem[i], color, tab.CoordinateX, textCoordinateY);
                }

                textCoordinateY += Settings.FontSize + 1;
            }
        }
    }
}

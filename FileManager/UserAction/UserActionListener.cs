using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NConsoleGraphics;

namespace FileManager.UserAction
{
    public class UserActionListener
    {
        public event EventHandler<NavigateEventArgs> Navigated;
        public void ReadInput()
        {
            if (Input.IsKeyDown(Keys.UP))
            {
                Navigated?.Invoke(this, new NavigateEventArgs() { Type = NavigateType.Up });
            }
            else if (Input.IsKeyDown(Keys.DOWN))
            {
                Navigated?.Invoke(this, new NavigateEventArgs() { Type = NavigateType.Down });
            }
            if (Input.IsKeyDown(Keys.RETURN))
            {
                Navigated?.Invoke(this, new NavigateEventArgs() { Type = NavigateType.Enter });
            }
            if (Input.IsKeyDown(Keys.BACK))
            {
                Navigated?.Invoke(this, new NavigateEventArgs() { Type = NavigateType.Back });
            }
        }
    }
}

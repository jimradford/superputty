using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using log4net;

namespace SuperPutty.Utils
{
    /// <summary>
    /// Utility class to save/restore form locations accounting multiple monitors, etc.
    /// 
    /// http://stackoverflow.com/questions/937298/restoring-window-size-position-with-multiple-monitors
    /// </summary>
    public class FormUtils
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(FormUtils));

        public static void RestoreFormPositionAndState(Form form, Rectangle winPos, FormWindowState winState)
        {

            // check if the saved bounds are nonzero and visible on any screen
            if (!winPos.IsEmpty && IsVisibleOnAnyScreen(winPos))
            {
                // first set the bounds
                form.StartPosition = FormStartPosition.Manual;
                form.DesktopBounds = winPos;

                // afterwards set the window state to the saved value (which could be Maximized)
                form.WindowState = winState;
                
                Log.InfoFormat("Restoring form position and state.  position={0}, state={1}", winPos, winState);
            }
            else if (!winPos.Size.IsEmpty)
            {
                // this resets the upper left corner of the window to windows standards
                form.StartPosition = FormStartPosition.WindowsDefaultLocation;

                // we can still apply the saved size
                form.Size = winPos.Size;
                Log.InfoFormat("Restoring form size only.  position.size={0}", winPos.Size);
            }


        }

        /// <summary>
        /// Returns true if rectangle is visible on the screens (key)
        /// </summary>
        /// <param name="rect"></param>
        /// <returns></returns>
        public static bool IsVisibleOnAnyScreen(Rectangle rect)
        {
            foreach (Screen screen in Screen.AllScreens)
            {
                if (screen.WorkingArea.IntersectsWith(rect))
                {
                    return true;
                }
            }

            return false;
        }

        public static T SafeParseEnum<T>(string val, bool ignoreCase, T defaultVal) 
        {
            T enumVal = defaultVal;
            try
            {
                enumVal = (T) Enum.Parse(typeof(T), val, ignoreCase);
            }
            catch(Exception ex)
            {
                Log.Error(string.Format("Could not parse ({0}) of type ({1})", val, typeof(T).Name), ex);
            }
            return enumVal;
        }

    }
}

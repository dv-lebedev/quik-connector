/*
The MIT License (MIT)

Copyright (c) 2015 Denis Lebedev

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
 */

using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace QuikConnector.API
{
    public class Terminal
    {
        #region optional functions

        private static string _exportMenuKey = "Экспорт данных";
        private static string _stopExportMenuItem = "Остановить экспорт таблиц по &DDE";
        private static string _startExportByDDEMenuItem = "Начать экспорт таблиц по &DDE";

        [DllImport("user32.dll")]
        static extern IntPtr GetMenu(IntPtr hWnd);

        [DllImport("user32.dll")]
        static extern IntPtr GetSubMenu(IntPtr hMenu, int nPos);

        [DllImport("user32.dll")]
        static extern int GetMenuItemCount(IntPtr hMenu);

        [DllImport("user32.dll")]
        static extern int GetMenuString(IntPtr hMenu, uint uIDItem, StringBuilder lpString, int nMaxCount, uint uFlag);

        [DllImport("user32.dll")]
        static extern uint GetMenuItemID(IntPtr hMenu, int nPos);

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll", SetLastError = true)]
        static extern bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        private const uint MF_BYPOSITION = 0x00000400;

        public static bool StartDDE()
        {
            IntPtr[] quikWindows = FindQuikWindow();

            if (quikWindows.Length == 0)
                return false;
            
            foreach (var quikWindow in quikWindows)
            {
                IntPtr mainMenu = GetMenu(quikWindow);
                int exportMenuIndex = (int)FindMenuItemByPart(mainMenu, _exportMenuKey);
                IntPtr exportMenu = GetSubMenu(mainMenu, exportMenuIndex);

                uint exportIndex = FindMenuItemByPart(exportMenu, _startExportByDDEMenuItem);
                uint menuItem = GetMenuItemID(exportMenu, (int)exportIndex);
                PostMessage(quikWindow, 0x111, (IntPtr)menuItem, (IntPtr)0);
            }

            return true;
        }

        public static bool StopDDE()
        {
            IntPtr[] quikWindows = FindQuikWindow();

            if (quikWindows.Length == 0)
                return false;        

            foreach (var quikWindow in quikWindows)
            {
                IntPtr mainMenu = GetMenu(quikWindow);
                int exportMenuIndex = (int)FindMenuItemByPart(mainMenu, _exportMenuKey);
                IntPtr exportMenu = GetSubMenu(mainMenu, exportMenuIndex);

                uint exportIndex = FindMenuItemByPart(exportMenu, _stopExportMenuItem);
                uint menuItem = GetMenuItemID(exportMenu, (int)exportIndex);
                PostMessage(quikWindow, 0x111, (IntPtr)menuItem, (IntPtr)0);
            }
            return true;
        }


        private static IntPtr[] FindQuikWindow()
        {
            Process[] processes = Process.GetProcessesByName("info");
            IntPtr[] result = new IntPtr[processes.Length];

            for (int i = 0; i < processes.Length; i++)
            {
                result[i] = processes[i].MainWindowHandle;
            }

            return result;
        }

        private static uint FindMenuItemByPart(IntPtr menu, string name)
        {
            int menuItemsCount = GetMenuItemCount(menu);

            for (uint menuIndex = 0; menuIndex < menuItemsCount; menuIndex++)
            {
                StringBuilder result = new StringBuilder(1024);
                GetMenuString(menu, menuIndex, result, 1024, MF_BYPOSITION);
                string buffer = result.ToString();
                if (buffer.Contains(name))
                {
                    return menuIndex;
                }
            }

            return 0;
        }


        public static string GetPathToActiveQuik()
        {
            return Process.GetProcessesByName("info")
                .FirstOrDefault()
                .MainModule
                .FileName
                .Replace("info.exe", "");
        }

        #endregion
    }
}

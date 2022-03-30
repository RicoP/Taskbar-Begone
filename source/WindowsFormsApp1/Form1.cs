using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1 {
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();
        }

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder strText, int maxCount);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

        [DllImport("user32.dll")]
        private static extern bool EnumWindows(EnumWindowsProc enumProc, IntPtr lParam);

        // Delegate to filter which windows to include 
        public delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        /// <summary> Get the text for the window pointed to by hWnd </summary>
        public static string GetWindowText(IntPtr hWnd) {
            int size = GetWindowTextLength(hWnd);
            if (size > 0) {
                var builder = new StringBuilder(size + 1);
                GetWindowText(hWnd, builder, builder.Capacity);
                return builder.ToString();
            }

            return String.Empty;
        }
        public static string GetWindowClassName(IntPtr hWnd) {
            int size = 256;
            if (size > 0) {
                var builder = new StringBuilder(size + 1);
                GetClassName(hWnd, builder, builder.Capacity);
                return builder.ToString();
            }

            return String.Empty;
        }

        /// <summary> Find all windows that match the given filter </summary>
        /// <param name="filter"> A delegate that returns true for windows
        ///    that should be returned and false for windows that should
        ///    not be returned </param>
        public static IEnumerable<IntPtr> FindWindows(EnumWindowsProc filter) {
            IntPtr found = IntPtr.Zero;
            List<IntPtr> windows = new List<IntPtr>();

            EnumWindows(delegate (IntPtr wnd, IntPtr param) {
                if (filter(wnd, param)) {
                    // only add the windows that pass the filter
                    windows.Add(wnd);
                }

                // but return true here so that we iterate all windows
                return true;
            }, IntPtr.Zero);

            return windows;
        }

        /// <summary> Find all windows that contain the given title text </summary>
        /// <param name="titleText"> The text that the window title must contain. </param>
        public static IEnumerable<IntPtr> FindWindowsWithText(string titleText) {
            return FindWindows(delegate (IntPtr wnd, IntPtr param) {
                return GetWindowText(wnd).Contains(titleText);
            });
        }


        [DllImport("user32.dll")]
        private static extern int FindWindow(string className, string windowText);
        [DllImport("user32.dll")]
        private static extern int ShowWindow(int hwnd, int command);

        private const int SW_HIDE = 0;
        private const int SW_SHOW = 1;

        protected static int Handle {
            get {
                return FindWindow("Shell_TrayWnd", "");
            }
        }

        public static void Show() {
            ShowWindow(Handle, SW_SHOW);
        }

        public static void Hide() {
            ShowWindow(Handle, SW_HIDE);
        }



        private void Form1_Load(object sender, EventArgs e) {

        }

        private void btnShow_Click(object sender, EventArgs e) {
            Show();
        }

        private void btnHide_Click(object sender, EventArgs e) {
            Hide();
        }

        private void btnList_Click(object sender, EventArgs e) {
            var allWindows = FindWindows((wh, rh) => true);
            foreach (var handle in allWindows) {
                Console.WriteLine(GetWindowClassName(handle));
            }
        }
    }
}

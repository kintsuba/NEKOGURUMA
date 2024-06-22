using System;
using System.Diagnostics;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.Web.WebView2.Core;
using Windows.UI.ViewManagement;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace NEKOGURUMA
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        private const int defaultWidth = 1136;
        private const int defaultHeight = 640;

        public MainWindow()
        {
            this.InitializeComponent();

            Title = "NEKOGURUMA";

            IntPtr hwnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            Microsoft.UI.WindowId windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hwnd);
            Microsoft.UI.Windowing.AppWindow appWindow =
                Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowId);
            appWindow.SetIcon("Assets/TitleLogo.ico");
            appWindow.Resize(new Windows.Graphics.SizeInt32(defaultWidth + 72, defaultHeight + 44));

            var us = new UISettings();
            appWindow.TitleBar.BackgroundColor = us.GetColorValue(UIColorType.Background);
        }
    }
}
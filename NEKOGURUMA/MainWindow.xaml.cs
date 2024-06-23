using System;
using System.Diagnostics;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Windows.UI.ViewManagement;
using Windows.UI.WindowManagement;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace NEKOGURUMA
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : WinUIEx.WindowEx
    {
        public MainWindow()
        {
            InitializeComponent();

            Title = "NEKOGURUMA";

            var theme = MainGrid.ActualTheme;

            AppWindow.TitleBar.ButtonBackgroundColor = (theme == ElementTheme.Light) ? Colors.Transparent : Colors.Black;
            AppWindow.TitleBar.ButtonInactiveBackgroundColor = (theme == ElementTheme.Light) ? Colors.Transparent : Colors.Black;
            AppWindow.TitleBar.ButtonForegroundColor = (theme == ElementTheme.Light) ? Colors.DarkSlateGray : Colors.White;

            AppWindow.SetIcon("Assets/TitleLogo.ico");
        }

        private void SideBar_PinButtonClicked(object sender, RoutedEventArgs e)
        {
            if (AppWindow.Presenter is OverlappedPresenter)
            {
                var presenter = AppWindow.Presenter as OverlappedPresenter;
                presenter.IsAlwaysOnTop = !presenter.IsAlwaysOnTop;
            }
        }

        private void SideBar_ScreenshotButtonClicked(object sender, RoutedEventArgs e)
        {
            OleWebview.TakeScreenshot();
        }
    }
}
using System;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Windows.Storage;
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
        private NekogurumaConfig Config { get; set; }

        public MainWindow()
        {
            Config = new NekogurumaConfig();

            InitializeComponent();
            InitializeLocalSetting();

            Title = "NEKOGURUMA";

            var theme = MainGrid.ActualTheme;

            AppWindow.TitleBar.ButtonBackgroundColor = (theme == ElementTheme.Light) ? Colors.Transparent : Colors.Black;
            AppWindow.TitleBar.ButtonInactiveBackgroundColor = (theme == ElementTheme.Light) ? Colors.Transparent : Colors.Black;
            AppWindow.TitleBar.ButtonForegroundColor = (theme == ElementTheme.Light) ? Colors.DarkSlateGray : Colors.White;

            AppWindow.SetIcon("Assets/TitleLogo.ico");
        }

        private async void InitializeLocalSetting()
        {
            try
            {
                var localFolder = await StorageFolder.GetFolderFromPathAsync(AppContext.BaseDirectory);
                var configFile = await localFolder.GetFileAsync("config.json");
                var configJson = await FileIO.ReadTextAsync(configFile);
                var config = JsonSerializer.Deserialize<NekogurumaConfig>(configJson);

                Config.ScreenshotFolder = config.ScreenshotFolder;
            }
            catch (FileNotFoundException)
            {
                var localFolder = await StorageFolder.GetFolderFromPathAsync(AppContext.BaseDirectory);
                StorageFile configFile = await localFolder.CreateFileAsync("config.json", CreationCollisionOption.ReplaceExisting);
                await FileIO.WriteTextAsync(configFile, JsonSerializer.Serialize(Config));
            }
        }

        private void SideBar_PinButtonClicked(object sender, RoutedEventArgs e)
        {
            if (AppWindow.Presenter is OverlappedPresenter)
            {
                var presenter = AppWindow.Presenter as OverlappedPresenter;
                presenter.IsAlwaysOnTop = !presenter.IsAlwaysOnTop;
            }
        }

        private void SideBar_VolumeButtonClicked(object sender, RoutedEventArgs e)
        {
            OleWebview.ToggleMute();
        }

        private void SideBar_ScreenshotButtonClicked(object sender, RoutedEventArgs e)
        {
            OleWebview.TakeScreenshot(Config);
        }

        private void SideBar_SettingButtonClicked(object sender, RoutedEventArgs e)
        {
            var settingsWindow = new SettingsWindow(Config);
            settingsWindow.Activate();
        }
    }

    public class NekogurumaConfig 
    {
        public string ScreenshotFolder { get; set; }

        public NekogurumaConfig()
        {
            Init();
        }

        private async void Init()
        {
            var picturesFolder = KnownFolders.PicturesLibrary;
            var screenshotFolder = await picturesFolder.CreateFolderAsync("screenshot", CreationCollisionOption.OpenIfExists);
            var nekogurumaFolder = await screenshotFolder.CreateFolderAsync("NEKOGURUMA", CreationCollisionOption.OpenIfExists);

            ScreenshotFolder = nekogurumaFolder.Path;
        }
    }
}
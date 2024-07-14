using System;
using System.Text.Json;
using System.Diagnostics;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.Storage.Pickers;
using Windows.Storage;
using Microsoft.UI;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace NEKOGURUMA
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsWindow : WinUIEx.WindowEx
    {
        private NekogurumaConfig Config { get; set; }

        public SettingsWindow(NekogurumaConfig Config)
        {
            this.Config = Config;

            this.InitializeComponent();

            Title = "ê›íË";

            var theme = MainGrid.ActualTheme;

            AppWindow.TitleBar.ButtonBackgroundColor = (theme == ElementTheme.Light) ? Colors.Transparent : Colors.Black;
            AppWindow.TitleBar.ButtonInactiveBackgroundColor = (theme == ElementTheme.Light) ? Colors.Transparent : Colors.Black;
            AppWindow.TitleBar.ButtonForegroundColor = (theme == ElementTheme.Light) ? Colors.DarkSlateGray : Colors.White;

            AppWindow.SetIcon("Assets/TitleLogo.ico");

            ScreenshotSettingCard.Description = Config.ScreenshotFolder;
        }

        private async void PickFolderButton_Click(object sender, RoutedEventArgs e)
        {
            var openPicker = new FolderPicker();

            var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            WinRT.Interop.InitializeWithWindow.Initialize(openPicker, hWnd);

            openPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            openPicker.FileTypeFilter.Add("*");

            var folder = await openPicker.PickSingleFolderAsync();
            if (folder == null) return;
            
            ScreenshotSettingCard.Description = folder.Path;
            Config.ScreenshotFolder = folder.Path;

            try
            {
                var localFolder = await StorageFolder.GetFolderFromPathAsync(AppContext.BaseDirectory);
                var configFile = await localFolder.CreateFileAsync("config.json", CreationCollisionOption.ReplaceExisting);
                await FileIO.WriteTextAsync(configFile, JsonSerializer.Serialize(Config));
            }
            catch (Exception ex)
            {
                var errorDialog = new ContentDialog
                {
                    Title = "Error",
                    Content = ex.Message + "\n" + ex.StackTrace,
                    CloseButtonText = "ï¬Ç∂ÇÈ",
                    XamlRoot = this.Content.XamlRoot
                };
                await errorDialog.ShowAsync();
            }
        }
    }
}

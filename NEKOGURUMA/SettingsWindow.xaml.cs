using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.Storage.AccessCache;
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
        public SettingsWindow()
        {
            this.InitializeComponent();

            Title = "ê›íË";

            var theme = MainGrid.ActualTheme;

            AppWindow.TitleBar.ButtonBackgroundColor = (theme == ElementTheme.Light) ? Colors.Transparent : Colors.Black;
            AppWindow.TitleBar.ButtonInactiveBackgroundColor = (theme == ElementTheme.Light) ? Colors.Transparent : Colors.Black;
            AppWindow.TitleBar.ButtonForegroundColor = (theme == ElementTheme.Light) ? Colors.DarkSlateGray : Colors.White;

            AppWindow.SetIcon("Assets/TitleLogo.ico");

            var localSettings = ApplicationData.Current.LocalSettings;
            var path = localSettings.Values["screenshotFolder"] as string;
            ScreenshotSettingCard.Description = path;
        }

        private async void PickFolderButton_Click(object sender, RoutedEventArgs e)
        {
            var openPicker = new FolderPicker();

            var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            WinRT.Interop.InitializeWithWindow.Initialize(openPicker, hWnd);

            openPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            openPicker.FileTypeFilter.Add("*");

            var folder = await openPicker.PickSingleFolderAsync();
            if (folder != null)
            {
                StorageApplicationPermissions.FutureAccessList.AddOrReplace("PickedFolderToken", folder);
                ScreenshotSettingCard.Description = folder.Path;

                var localSettings = ApplicationData.Current.LocalSettings;
                localSettings.Values["screenshotFolder"] = folder.Path;
            }
        }
    }
}

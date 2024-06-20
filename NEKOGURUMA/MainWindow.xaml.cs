using System;
using System.Diagnostics;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
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

            appWindow.Resize(new Windows.Graphics.SizeInt32(defaultWidth, defaultHeight));

            this.SizeChanged += MainWindow_SizeChanged;

            UISettings us = new UISettings();
            appWindow.TitleBar.BackgroundColor = us.GetColorValue(UIColorType.Background);

            OLE.CoreWebView2Initialized += WebView_CoreWebView2Initialized;
        }

        private void WebView_CoreWebView2Initialized(
            WebView2 sender,
            CoreWebView2InitializedEventArgs args
        )
        {
            // ナビゲーション完了イベントを監視
            OLE.CoreWebView2.NavigationCompleted += CoreWebView2_NavigationCompleted;

            OLE.CoreWebView2.FrameCreated += CoreWebView_CoreWebView2FrameCreated;
        }

        private async void CoreWebView2_NavigationCompleted(
            CoreWebView2 sender,
            CoreWebView2NavigationCompletedEventArgs args
        )
        {
            if (args.IsSuccess)
            {
                var currentUri = sender.Source;

                if (currentUri == "https://pc-play.games.dmm.com/play/olesma/")
                {
                    // こつタワーのページに遷移した場合
                    string js =
                        @" 
                            const styleTag = document.createElement('style');
                            styleTag.textContent = `
                                *, *::before, *::after, body {
                                    margin: 0;
                                    padding: 0;
                                    box-sizing:border-box;
                                    border: none;
                                }

                                body {
                                    overflow: hidden;
                                    text-overflow: clip;
                                    white-space: nowrap;
                                }

                                #foot, .dmm-ntgnavi, .area-naviapp {
                                    display: none;
                                }
                            `;
                            document.head.appendChild(styleTag);
                        ";
                    var result = await sender.ExecuteScriptAsync(js);
                }
            }
        }

        private void CoreWebView_CoreWebView2FrameCreated(
            CoreWebView2 sender,
            CoreWebView2FrameCreatedEventArgs args
        )
        {
            args.Frame.NavigationStarting += Frame_NavigationStarting;
        }

        private void Frame_NavigationStarting(
            CoreWebView2Frame sender,
            CoreWebView2NavigationStartingEventArgs args
        )
        {
            if (args.Uri.Contains("osapi.dmm.com"))
            {
                sender.NavigationCompleted += Frame_NavigationCompleted;
            }
        }

        private async void Frame_NavigationCompleted(
            CoreWebView2Frame sender,
            CoreWebView2NavigationCompletedEventArgs args
        )
        {
            var result = await sender.ExecuteScriptAsync(
                "document.getElementById('olet').style.height = '647px'"
            );
            Debug.WriteLine(result);
        }

        private void MainWindow_SizeChanged(object sender, WindowSizeChangedEventArgs e)
        {
            if (OLE.CoreWebView2 != null)
            {
                float zoom = (float)(MainGrid.ActualHeight / defaultHeight);
                OLE.Scale = new System.Numerics.Vector3(zoom, zoom, 1.0f);

                var cp = new System.Numerics.Vector3(
                    (float)(MainGrid.ActualWidth / 2.0f),
                    (float)(MainGrid.ActualHeight / 2.0f),
                    1.0f
                );
                OLE.CenterPoint = cp;

                Debug.WriteLine(
                    $"Zoom: {zoom}, BoundsHeight: {MainGrid.ActualHeight}, CenterPoint: {cp}"
                );
            }
        }
    }
}
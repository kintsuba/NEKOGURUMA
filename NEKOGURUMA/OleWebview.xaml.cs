using System;
using System.Diagnostics;
using CommunityToolkit.WinUI.Notifications;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Web.WebView2.Core;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.Storage.Streams;

namespace NEKOGURUMA
{
    public sealed partial class OleWebview : UserControl
    {
        public OleWebview()
        {
            this.InitializeComponent();

            OLE.CoreWebView2Initialized += WebView_CoreWebView2Initialized;
        }

        private void WebView_CoreWebView2Initialized(
            WebView2 sender,
            CoreWebView2InitializedEventArgs args
        )
        {
            // �i�r�Q�[�V���������C�x���g���Ď�
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
                    // ���^���[�̃y�[�W�ɑJ�ڂ����ꍇ
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
                    await sender.ExecuteScriptAsync(js);
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
                // �悭�킩��Ȃ������ˑ��H��iframe�̒��g���͂ݏo�Ă���̂ŁA����ɖ�����荇�킹��
                // ���̍����͏㉺�����ɑ����邽�߂ɂ����g���Ȃ��̂ŁA�Q�[����ʂ��`�悳���canvas�͑��ς�炸640px�̂܂�
                "document.getElementById('olet').style.height = '647px'"
            );
        }

        public async void TakeScreenshot(NekogurumaConfig Config)
        {
            if (OLE.CoreWebView2 != null)
            {
                var path = Config.ScreenshotFolder;

                var now = DateTime.Now;
                string fileName = "OleTower-" + now.ToString("yyMMdd-HHmmssff") + ".png";

                try
                {
                    var folder = await StorageFolder.GetFolderFromPathAsync(path);
                    var file = await folder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);

                    using var stream = await file.OpenAsync(FileAccessMode.ReadWrite);
                    await OLE.CoreWebView2.CapturePreviewAsync(CoreWebView2CapturePreviewImageFormat.Png, stream);
                    await stream.FlushAsync();

                    DataPackage dataPackage = new DataPackage();
                    dataPackage.SetBitmap(RandomAccessStreamReference.CreateFromStream(stream));
                    Clipboard.SetContent(dataPackage);
                    Clipboard.Flush();

                    new ToastContentBuilder()                        
                        .AddArgument("action", "viewConversation")
                        .AddArgument("conversationId", 9813)
                        .AddArgument("filePath", file.Path)
                        .AddHeroImage(new Uri(file.Path))
                        .AddText("�X�N���[���V���b�g���N���b�v�{�[�h�ɃR�s�[���ĕۑ����܂���")
                        .Show();
                }
                catch (Exception ex)
                {
                    var errorDialog = new ContentDialog
                    {
                        Title = "Error",
                        Content = ex.Message + "\n" + ex.StackTrace,
                        CloseButtonText = "����",
                        XamlRoot = this.Content.XamlRoot
                    };
                    await errorDialog.ShowAsync();
                }
            }
        }

        public void ToggleMute()
        {
            if (OLE.CoreWebView2 != null)
            {
                OLE.CoreWebView2.IsMuted = !OLE.CoreWebView2.IsMuted;
            }
        }
    }
}
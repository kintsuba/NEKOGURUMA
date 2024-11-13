using System;
using Microsoft.Windows.AppNotifications.Builder;
using Microsoft.Windows.AppNotifications;
using Windows.Storage;
using Windows.System;
using System.Diagnostics;

namespace NEKOGURUMA.Notifications
{
    internal class ToastWithScreenshot
    {
        public const int ScenarioId = 1;
        public const string ScenarioName = "Local Toast with Screenshot";

        public static bool SendToast(StorageFile file)
        {
            var appNotification = new AppNotificationBuilder()
                .AddArgument(Common.scenarioTag, ScenarioId.ToString())
                .AddArgument(Common.actionTag, "viewScreenshot")
                .AddArgument("conversationId", "9813")
                .AddArgument(Common.filePathTag, file.Path)
                .SetHeroImage(new Uri(file.Path))
                .AddText("スクリーンショットをクリップボードにコピーして保存しました")
                .BuildNotification();

            appNotification.ExpiresOnReboot = false;
            AppNotificationManager.Default.Show(appNotification);

            return appNotification.Id != 0; // return true (indicating success) if the toast was sent (if it has an Id)
        }

        public static async void NotificationReceived(AppNotificationActivatedEventArgs notificationActivatedEventArgs)
        {
            string filePath = notificationActivatedEventArgs.Arguments[Common.filePathTag];
            await Launcher.LaunchUriAsync(new Uri(filePath));
        }
    }
}

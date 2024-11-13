using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Windows.AppNotifications;
using Windows.System;


namespace NEKOGURUMA.Notifications
{
    internal class NotificationManager
    {
        private bool m_isRegistered;

        private readonly Dictionary<int, Action<AppNotificationActivatedEventArgs>> c_notificationHandlers;

        public NotificationManager()
        {
            m_isRegistered = false;

            // When adding new a scenario, be sure to add its notification handler here.
            c_notificationHandlers = new Dictionary<int, Action<AppNotificationActivatedEventArgs>>
            {
                { ToastWithScreenshot.ScenarioId, ToastWithScreenshot.NotificationReceived }
            };
        }

        ~NotificationManager()
        {
            Unregister();
        }

        public void Init()
        {
            // To ensure all Notification handling happens in this process instance, register for
            // NotificationInvoked before calling Register(). Without this a new process will
            // be launched to handle the notification.
            AppNotificationManager notificationManager = AppNotificationManager.Default;

            notificationManager.NotificationInvoked += OnNotificationInvoked;

            notificationManager.Register();
            m_isRegistered = true;
        }

        public void Unregister()
        {
            if (m_isRegistered)
            {
                AppNotificationManager.Default.Unregister();
                m_isRegistered = false;
            }
        }

        public void ProcessLaunchActivationArgs(AppNotificationActivatedEventArgs notificationActivatedEventArgs)
        {
            DispatchNotification(notificationActivatedEventArgs);
        }

        public bool DispatchNotification(AppNotificationActivatedEventArgs notificationActivatedEventArgs)
        {
            var scenarioId = notificationActivatedEventArgs.Arguments[Common.scenarioTag];
            if (scenarioId.Length != 0)
            {
                try
                {
                    c_notificationHandlers[int.Parse(scenarioId)](notificationActivatedEventArgs);
                    return true;
                }
                catch
                {
                    return false; // Couldn't find a NotificationHandler for scenarioId.
                }
            } else
            {
                return false; // No scenario specified in the notification
            }
        }

        void OnNotificationInvoked(object sender, AppNotificationActivatedEventArgs notificationActivatedEventArgs)
        {
            ProcessLaunchActivationArgs(notificationActivatedEventArgs);
        }
    }
}

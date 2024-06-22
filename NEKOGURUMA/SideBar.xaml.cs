using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace NEKOGURUMA
{
    public sealed partial class SideBar : UserControl
    {
        public SideBar()
        {
            this.InitializeComponent();
        }

        public event EventHandler<RoutedEventArgs> PinButtonClicked;
        public event EventHandler<RoutedEventArgs> VolumeButtonClicked;
        public event EventHandler<RoutedEventArgs> ScreenshotButtonClicked;

        private void PinButton_Click(object sender, RoutedEventArgs e)
        {
            PinButtonClicked?.Invoke(this, e);
        }

        private void VolumeButton_Click(object sender, RoutedEventArgs e)
        {
            VolumeButtonClicked?.Invoke(this, e);
        }

        private void ScreenShotButton_Click(object sender, RoutedEventArgs e)
        {
            ScreenshotButtonClicked?.Invoke(this, e);
        }
    }
}
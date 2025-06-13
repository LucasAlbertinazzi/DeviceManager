using DeviceManager.Mobile.Views;

namespace DeviceManager.Mobile
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(DeviceFormPage), typeof(DeviceFormPage));
        }
    }
}

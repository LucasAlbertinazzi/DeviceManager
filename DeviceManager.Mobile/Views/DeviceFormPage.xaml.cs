using DeviceManager.Mobile.ViewModels;

namespace DeviceManager.Mobile.Views;

public partial class DeviceFormPage : ContentPage
{
    public DeviceFormPage(DeviceFormViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
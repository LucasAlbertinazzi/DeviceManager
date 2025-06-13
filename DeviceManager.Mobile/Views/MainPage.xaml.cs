using DeviceManager.Mobile.ViewModels;

namespace DeviceManager.Mobile.Views
{
    public partial class MainPage : ContentPage
    {
        private readonly DeviceViewModel _viewModel;

        public MainPage(DeviceViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = _viewModel = viewModel;
        }

        private void OnAddClicked(object sender, EventArgs e)
        {
            _viewModel.AdicionarDispositivoCommand.Execute(null);
        }

        private void FecharPopup(object sender, EventArgs e)
        {
            if (BindingContext is DeviceViewModel vm)
                vm.ExibirLogs = false;
        }


        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (BindingContext is DeviceViewModel vm)
            {
                vm.Recarregar();
            }
        }

    }
}

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DeviceManager.Mobile.Data;
using DeviceManager.Mobile.Models;
using System.Threading.Tasks;

namespace DeviceManager.Mobile.ViewModels
{
    public partial class DeviceFormViewModel : ObservableObject
    {
        private readonly RealmDbContext _realmDb;

        private DispositivoRealm? _modelo;

        [ObservableProperty]
        private string descricao;

        partial void OnDescricaoChanged(string value)
        {
            OnPropertyChanged(nameof(DescricaoInvalida));
        }

        [ObservableProperty]
        private string codigoReferencia;

        partial void OnCodigoReferenciaChanged(string value)
        {
            OnPropertyChanged(nameof(CodigoReferenciaInvalido));
        }

        public bool DescricaoInvalida => string.IsNullOrWhiteSpace(Descricao);
        public bool CodigoReferenciaInvalido => string.IsNullOrWhiteSpace(CodigoReferencia);

        [ObservableProperty]
        private bool isEditando;


        public DeviceFormViewModel(RealmDbContext realmDb)
        {
            _realmDb = realmDb;
        }

        public void IniciarNovo()
        {
            _modelo = new DispositivoRealm
            {
                Descricao = string.Empty,
                CodigoReferencia = Guid.NewGuid().ToString(),
                DataCriacao = DateTimeOffset.UtcNow,
                IsSynced = false,
                IsDeleted = false
            };

            Descricao = _modelo.Descricao;
            CodigoReferencia = _modelo.CodigoReferencia;
            IsEditando = false;
        }

        public void Editar(DispositivoRealm existente)
        {
            _modelo = existente;

            // Salva os valores originais
            var descricaoOriginal = existente.Descricao;
            var codigoReferenciaOriginal = existente.CodigoReferencia;

            Descricao = existente.Descricao;
            CodigoReferencia = existente.CodigoReferencia;
            IsEditando = true;

            // Só marca como editado se houve alteração
            if (Descricao != descricaoOriginal || CodigoReferencia != codigoReferenciaOriginal)
            {
                _realmDb.Update(() =>
                {
                    _modelo.IsSynced = false;
                    _modelo.DataAtualizacao = DateTimeOffset.UtcNow;
                });
            }
        }


        [RelayCommand]
        private async Task Salvar()
        {
            if (_modelo == null)
                return;

            // Validação de campos obrigatórios
            if (string.IsNullOrWhiteSpace(Descricao) || string.IsNullOrWhiteSpace(CodigoReferencia))
            {
                await Application.Current.MainPage.DisplayAlert("Erro", "Preencha todos os campos obrigatórios.", "OK");
                return;
            }

            // Validação de unicidade
            var codigoJaExiste = _realmDb.GetAll<DispositivoRealm>()
                .Any(d => d.CodigoReferencia == CodigoReferencia && d.Id != _modelo.Id && !d.IsDeleted);

            if (codigoJaExiste)
            {
                await Application.Current.MainPage.DisplayAlert("Erro", "Já existe um dispositivo com esse Código de Referência.", "OK");
                return;
            }

            if (IsEditando)
            {
                _realmDb.Update(() =>
                {
                    _modelo.Descricao = Descricao;
                    _modelo.CodigoReferencia = CodigoReferencia;
                    _modelo.IsSynced = false;
                    _modelo.DataAtualizacao = DateTimeOffset.UtcNow;
                });
            }
            else
            {
                _modelo.Descricao = Descricao;
                _modelo.CodigoReferencia = CodigoReferencia;
                _modelo.IsSynced = false;
                _modelo.DataCriacao = DateTimeOffset.UtcNow;

                _realmDb.Add(_modelo);
            }

            await Shell.Current.Navigation.PopAsync();
        }


    }
}

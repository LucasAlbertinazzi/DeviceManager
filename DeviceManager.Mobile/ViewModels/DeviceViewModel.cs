using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DeviceManager.Mobile.Data;
using DeviceManager.Mobile.Models;
using DeviceManager.Mobile.Views;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace DeviceManager.Mobile.ViewModels
{
    public partial class DeviceViewModel : ObservableObject
    {
        private readonly RealmDbContext _realmDb;
        private readonly IServiceProvider _serviceProvider;
        private readonly HttpClient _httpClient;

        private const string ApiBaseUrl = "http://192.168.3.4:5000/api/dispositivos";

        private bool _estaSincronizando;
        public bool EstaSincronizando
        {
            get => _estaSincronizando;
            set => SetProperty(ref _estaSincronizando, value);
        }

        [ObservableProperty]
        private string logSincronizacao;

        [ObservableProperty]
        private bool exibirLogs;

        [ObservableProperty]
        private ObservableCollection<DispositivoRealm> dispositivos = new();

        public DeviceViewModel(RealmDbContext realmDb, IServiceProvider serviceProvider)
        {
            _realmDb = realmDb;
            _serviceProvider = serviceProvider;
            _httpClient = new HttpClient();

            CarregarDispositivos();
        }

        private void CarregarDispositivos()
        {
            var lista = _realmDb.GetAll<DispositivoRealm>()
                                .Where(d => !d.IsDeleted)
                                .OrderByDescending(d => d.DataCriacao)
                                .ToList();

            Dispositivos = new ObservableCollection<DispositivoRealm>(lista);
        }

        public void Recarregar() => CarregarDispositivos();

        [RelayCommand]
        private async Task AdicionarDispositivo()
        {
            var vm = _serviceProvider.GetService<DeviceFormViewModel>();
            vm!.IniciarNovo();

            var page = _serviceProvider.GetService<DeviceFormPage>();
            page!.BindingContext = vm;

            await Shell.Current.Navigation.PushAsync(page);
        }

        [RelayCommand]
        private async Task EditarDispositivo(DispositivoRealm dispositivo)
        {
            var vm = _serviceProvider.GetService<DeviceFormViewModel>();
            vm!.Editar(dispositivo);

            var page = _serviceProvider.GetService<DeviceFormPage>();
            page!.BindingContext = vm;

            await Shell.Current.Navigation.PushAsync(page);
        }

        [RelayCommand]
        private void ExcluirDispositivo(DispositivoRealm dispositivo)
        {
            if (dispositivo is null) return;

            _realmDb.Update(() =>
            {
                dispositivo.IsDeleted = true;
                dispositivo.IsSynced = false;
                dispositivo.DataAtualizacao = DateTimeOffset.UtcNow;
            });

            CarregarDispositivos();
        }

        [RelayCommand]
        private async Task SincronizarAsync()
        {
            EstaSincronizando = true;
            LogSincronizacao = string.Empty;

            var pendentes = _realmDb.GetAll<DispositivoRealm>()
                .Where(d => !d.IsSynced || (d.IsDeleted && d.IsSynced))
                .ToList();

            if (!pendentes.Any())
            {
                await Application.Current.MainPage.DisplayAlert("Aviso", "Nenhum item pendente de sincronização!", "OK");
                EstaSincronizando = false;
                return;
            }

            if (Connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Sem conexão",
                    "Você está sem acesso à internet. Tente sincronizar novamente quando estiver conectado.",
                    "OK"
                );
                EstaSincronizando = false;
                return;
            }

            try
            {
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
                var pingResponse = await _httpClient.GetAsync($"{ApiBaseUrl}/ping", cts.Token);

                if (!pingResponse.IsSuccessStatusCode)
                {
                    await Application.Current.MainPage.DisplayAlert(
                        "Servidor indisponível",
                        "Conectado à internet, mas não foi possível acessar a API. Tente novamente mais tarde.",
                        "OK"
                    );
                    EstaSincronizando = false;
                    return;
                }
            }
            catch (OperationCanceledException)
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Conexão lenta",
                    "A API demorou muito para responder. Tente novamente em uma conexão mais rápida.",
                    "OK"
                );
                EstaSincronizando = false;
                return;
            }
            catch
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Erro de rede",
                    "Não foi possível se conectar à API. Verifique se o servidor está disponível.",
                    "OK"
                );
                EstaSincronizando = false;
                return;
            }

            LogSincronizacao = "🔄 Iniciando sincronização...\n";
            ExibirLogs = true;

            try
            {
                var json = JsonSerializer.Serialize(pendentes);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"{ApiBaseUrl}/sync", content);
                response.EnsureSuccessStatusCode();

                var resposta = await response.Content.ReadAsStringAsync();
                var resultado = JsonSerializer.Deserialize<SincronizacaoResposta>(resposta);

                if (resultado is null)
                {
                    LogSincronizacao += "⚠️ Erro ao interpretar resposta da API.\n";
                    EstaSincronizando = false;
                    return;
                }

                var itensParaRemover = new List<DispositivoRealm>();

                _realmDb.Update(() =>
                {
                    foreach (var item in pendentes)
                    {
                        if (item.IsDeleted)
                        {
                            itensParaRemover.Add(item);
                        }
                        else
                        {
                            item.IsSynced = true;
                            item.DataAtualizacao = DateTimeOffset.UtcNow;
                        }
                    }
                });

                foreach (var item in itensParaRemover)
                {
                    _realmDb.Remove(item);
                }

                foreach (var log in resultado.Logs)
                {
                    LogSincronizacao += $"{log}\n";
                    await Task.Delay(50);
                }

                LogSincronizacao += $"📦 Total enviados: {resultado.TotalRecebidos}\n";
                await Task.Delay(1);
            }
            catch (HttpRequestException)
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Erro de conexão",
                    "Não foi possível se conectar com o servidor. Verifique sua conexão e tente novamente mais tarde.",
                    "OK"
                );
            }
            catch (Exception ex)
            {
                LogSincronizacao += $"❌ Erro: {ex.Message}\n";
            }

            CarregarDispositivos();

            EstaSincronizando = false;
        }
    }
}

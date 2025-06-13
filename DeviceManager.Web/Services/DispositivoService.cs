using DeviceManager.Web.Models;
using System.Net.Http.Json;

namespace DeviceManager.Web.Services
{
    public class DispositivoService
    {
        private readonly HttpClient _http;

        public DispositivoService(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<Dispositivo>> GetAllAsync()
        {
            return await _http.GetFromJsonAsync<List<Dispositivo>>("api/dispositivos") ?? new List<Dispositivo>();
        }

        public async Task<Dispositivo?> GetByIdAsync(string id)
        {
            try
            {
                return await _http.GetFromJsonAsync<Dispositivo>($"api/dispositivos/{id}");
            }
            catch (HttpRequestException)
            {
                return null;
            }
        }

        public async Task<Dispositivo?> GetByCodigoReferenciaAsync(string codigo)
        {
            try
            {
                var codigoSeguro = Uri.EscapeDataString(codigo);
                return await _http.GetFromJsonAsync<Dispositivo?>($"api/dispositivos/por-referencia/{codigoSeguro}");
            }
            catch (HttpRequestException)
            {
                return null;
            }
        }

        public async Task CreateAsync(Dispositivo dispositivo)
        {
            var response = await _http.PostAsJsonAsync("api/dispositivos", dispositivo);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new ApplicationException($"Erro ao criar: {error}");
            }
        }

        public async Task UpdateAsync(string id, Dispositivo dispositivo)
        {
            var response = await _http.PutAsJsonAsync($"api/dispositivos/{id}", dispositivo);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new ApplicationException($"Erro ao atualizar: {error}");
            }
        }

        public async Task DeleteAsync(string id)
        {
            var response = await _http.DeleteAsync($"api/dispositivos/{id}");
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new ApplicationException($"Erro ao excluir: {error}");
            }
        }
    }
}

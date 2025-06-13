using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManager.Mobile.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;

        public ApiService()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("http://192.168.3.4:5000/api/dispositivos")
            };
        }

        public async Task<DispositivoDto?> GetByCodigoReferenciaAsync(string codigoRef)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<DispositivoDto>($"Devices/bycodigo/{codigoRef}");
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> PostAsync(DispositivoDto dispositivo)
        {
            var response = await _httpClient.PostAsJsonAsync("Devices", dispositivo);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> PutAsync(string id, DispositivoDto dispositivo)
        {
            var response = await _httpClient.PutAsJsonAsync($"Devices/{id}", dispositivo);
            return response.IsSuccessStatusCode;
        }
    }

    public class DispositivoDto
    {
        public string Id { get; set; }
        public string Descricao { get; set; }
        public string CodigoReferencia { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime? DataAtualizacao { get; set; }
    }
}

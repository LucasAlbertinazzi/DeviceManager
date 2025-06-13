using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DeviceManager.Mobile.Models
{
    public class SincronizacaoResposta
    {
        [JsonPropertyName("sucesso")]
        public bool Sucesso { get; set; }

        [JsonPropertyName("totalRecebidos")]
        public int TotalRecebidos { get; set; }

        [JsonPropertyName("logs")]
        public List<string> Logs { get; set; }
    }

}

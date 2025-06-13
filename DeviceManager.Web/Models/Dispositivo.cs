using System.ComponentModel.DataAnnotations;

namespace DeviceManager.Web.Models
{
    public class Dispositivo
    {
        public string Id { get; set; }

        public string Descricao { get; set; } = string.Empty;

        public string CodigoReferencia { get; set; } = string.Empty;

        public DateTimeOffset DataCriacao { get; set; }

        public DateTimeOffset? DataAtualizacao { get; set; }

        public bool IsDeleted { get; set; }
    }
}

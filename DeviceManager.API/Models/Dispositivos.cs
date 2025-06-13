using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace DeviceManager.API.Models
{
    public class Dispositivo
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        public string Descricao { get; set; }

        public string CodigoReferencia { get; set; } // unico

        public DateTimeOffset DataCriacao { get; set; }

        public DateTimeOffset? DataAtualizacao { get; set; }

        public bool IsDeleted { get; set; }
    }
}

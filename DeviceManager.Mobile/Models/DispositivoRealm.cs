using Realms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManager.Mobile.Models
{
    public partial class DispositivoRealm : RealmObject
    {
        [PrimaryKey]
        [MapTo("_id")]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string Descricao { get; set; }

        public string CodigoReferencia { get; set; }

        public DateTimeOffset DataCriacao { get; set; } = DateTimeOffset.UtcNow;

        public DateTimeOffset? DataAtualizacao { get; set; }

        public bool IsDeleted { get; set; } = false;

        public bool IsSynced { get; set; } = false;
    }
}

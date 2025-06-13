using DeviceManager.API.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace DeviceManager.API.Services
{
    public class DispositivoService
    {
        private readonly IMongoCollection<Dispositivo> _collection;

        public DispositivoService(IOptions<MongoDbSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            var database = client.GetDatabase(settings.Value.DatabaseName);
            _collection = database.GetCollection<Dispositivo>(settings.Value.CollectionName);
        }

        public async Task<List<Dispositivo>> GetAllAsync() =>
            await _collection.Find(_ => true).ToListAsync();

        public async Task<Dispositivo> GetByIdAsync(string id) =>
            await _collection.Find(x => x.Id == id).FirstOrDefaultAsync();

        public async Task<Dispositivo> CreateAsync(Dispositivo dispositivo)
        {
            dispositivo.DataCriacao = DateTime.UtcNow;
            await _collection.InsertOneAsync(dispositivo);
            return dispositivo;
        }

        public async Task UpdateAsync(string id, Dispositivo dispositivo)
        {
            dispositivo.DataAtualizacao = DateTime.UtcNow;
            await _collection.ReplaceOneAsync(x => x.Id == id, dispositivo);
        }

        public async Task DeleteAsync(string id) =>
            await _collection.DeleteOneAsync(x => x.Id == id);

        public async Task<Dispositivo?> GetByCodigoReferenciaAsync(string codigoReferencia)
        {
            return await _collection.Find(d => d.CodigoReferencia == codigoReferencia).FirstOrDefaultAsync();
        }
    }
}

using DeviceManager.Mobile.Models;
using Realms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManager.Mobile.Data
{
    public class RealmDbContext
    {
        private readonly Realm _realm;

        public RealmDbContext()
        {
            var config = new RealmConfiguration(Path.Combine(FileSystem.AppDataDirectory, "devices.realm"))
            {
                SchemaVersion = 2,
                ShouldDeleteIfMigrationNeeded = true
            };

            _realm = Realm.GetInstance(config);
        }

        public IQueryable<T> GetAll<T>() where T : RealmObject
        {
            return _realm.All<T>();
        }

        public void Add<T>(T item) where T : RealmObject
        {
            _realm.Write(() => _realm.Add(item));
        }

        public void Update(Action action)
        {
            _realm.Write(action);
        }

        public void Remove<T>(T item) where T : RealmObject
        {
            _realm.Write(() => _realm.Remove(item));
        }
    }

}

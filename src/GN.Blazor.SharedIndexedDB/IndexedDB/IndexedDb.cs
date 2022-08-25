using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Reflection;
using System.Linq;
using GN.Blazor.SharedIndexedDB.Messaging;
using GN.Blazor.SharedIndexedDB.Messaging.Commands;

namespace GN.Blazor.SharedIndexedDB.IndexedDB
{
    class IndexedDb : IIndexedDb
    {
        public const int DEFAULT_TIMEOUT = 5000;
        private readonly string dbName;
        private readonly ILogger<IndexedDb> logger;
        private readonly IMessageBus bus;
        private DatabaseSchema _schema;

        public IndexedDb(string dbName, ILogger<IndexedDb> logger, IMessageBus bus)
        {
            if (string.IsNullOrEmpty(dbName))
            {
                throw new ArgumentException($"'{nameof(dbName)}' cannot be null or empty.", nameof(dbName));
            }

            if (bus is null)
            {
                throw new ArgumentNullException(nameof(bus));
            }
            this.dbName = dbName;
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.bus = bus;
        }
        public async Task<IIndexedDb> EnsureDatabase()
        {
            await Task.CompletedTask;
            await bus.CreateContext(new CreateDatabase(dbName))
                .Request();
            return this;
        }
        public async Task<DatabaseSchema> GetSchema(bool refersh = false)
        {
            if (_schema == null || refersh)
            {
                await EnsureDatabase();
                var req = await bus.CreateContext(new GetDatabaseSchema(dbName))
                    .Request();
                _schema = req.GetPayload<DatabaseSchema>();
            }
            return _schema;
        }

        public async Task<IIndexedDbStore<T>> GetStore<T>(StoreSchema schema) where T : class
        {
            var result = new IndexedDbStore<T>(dbName, schema, logger, bus);
            await result.EnsureStore();
            return result;
        }

        public Task<IIndexedDbStore<object>> GetStore(StoreSchema schema)
        {
            return GetStore<object>(schema);
        }

        private StoreSchema CreateSchema(Type type, string name = null)
        {
            var keyProp = type.GetProperties().FirstOrDefault(x => x.GetCustomAttribute<KeyAttribute>() != null) ??
                type.GetProperty("Id");
            if (keyProp == null)
            {
                throw new Exception("Key not found");
            }

            return new StoreSchema
            {
                StoreName = string.IsNullOrWhiteSpace(name) ? type.GetCustomAttribute<TableAttribute>()?.Name ?? type.Name : name,
                PrimaryKey = new IndexData(
                                            keyProp.Name.ToCamel(),
                                            keyProp.GetCustomAttribute<KeyAttribute>()?.Name?.ToLowerInvariant() ?? keyProp.Name?.ToLowerInvariant(),
                                            true),
                Indexes = type.GetProperties()
                    .Where(x => x.GetCustomAttribute<IndexAttribute>() != null)
                    .Select(x => new IndexData(x.Name.ToCamel(),
                            x.GetCustomAttribute<IndexAttribute>()?.Name?.ToLowerInvariant() ?? x.Name?.ToLowerInvariant(),
                            x.GetCustomAttribute<IndexAttribute>().Unique))
                    .ToArray()

            };


        }
        public Task<IIndexedDbStore<T>> GetStore<T>(string storeName = null, Expression<Func<T, object>> idSelector = null) where T : class
        {

            return GetStore<T>(CreateSchema(typeof(T), storeName));
        }
    }
}

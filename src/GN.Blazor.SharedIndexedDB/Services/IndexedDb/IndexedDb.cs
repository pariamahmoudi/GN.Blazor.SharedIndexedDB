using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using GN.Blazor.SharedIndexedDB.Models.Messages;
using GN.Blazor.SharedIndexedDB.Models;
using System.Linq.Expressions;
using System.Reflection;
using System.Linq;

namespace GN.Blazor.SharedIndexedDB.Services
{
    class IndexedDb : IIndexedDb
    {
        public const int DEFAULT_TIMEOUT = 5000;
        private readonly string dbName;
        private readonly ILogger<IndexedDb> logger;
        private readonly IMessageBus bus;
        private int _timeout = 5 * 1000;
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
            await this.bus.CreateContext(new CreateDatabaseMessage(this.dbName))
                .Request();
            return this;
        }
        public async Task<DatabaseSchema> GetSchema(bool refersh = false)
        {
            if (this._schema == null || refersh)
            {
                await EnsureDatabase();
                var req = await bus.CreateContext(new GetDatabaseSchema(this.dbName))
                    .Request();
                this._schema = req.GetPayload<DatabaseSchema>();
            }
            return this._schema;
        }

        public async Task<IIndexedDbStore<T>> GetStore<T>(StoreSchema schema) where T : class
        {
            var result = new IndexedDbStore<T>(this.dbName, schema, this.logger, this.bus);
            await result.EnsureStore();
            return result;
        }

        public Task<IIndexedDbStore<object>> GetStore(StoreSchema schema)
        {
            return this.GetStore<object>(schema);
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
                StoreName = string.IsNullOrWhiteSpace(name) ? (type.GetCustomAttribute<TableAttribute>()?.Name ?? type.Name) : name,
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

            return this.GetStore<T>(CreateSchema(typeof(T), storeName));
        }
    }
}

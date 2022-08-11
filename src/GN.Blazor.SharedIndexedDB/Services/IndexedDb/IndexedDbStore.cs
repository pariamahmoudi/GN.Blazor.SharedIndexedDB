using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using GN.Blazor.SharedIndexedDB.Models.Messages;
using GN.Blazor.SharedIndexedDB.Models;
using System.Collections.Generic;
using System.Linq;
using GN.Blazor.SharedIndexedDB.Services.LinqQuery;
using System.Reflection;

namespace GN.Blazor.SharedIndexedDB.Services
{
    public class QueryBuilder<T> : IQueryBuilder<T>
    {
        public QueryBuilder(StoreFetchPayload Payload)
        {
            this.Payload = Payload;
        }

        public StoreFetchPayload Payload { get; }

        public IQueryBuilder<T> Filter(Filter filter)
        {
            this.Payload.Filter = filter;
            return this;
        }

        public IQueryBuilder<T> OrderBy(string field, bool descending = false)
        {
            this.Payload.OrderBy = field?.ToLowerInvariant();
            this.Payload.Descending = descending ? true : null;
            return this;
        }

        public IQueryBuilder<T> SelectKeys()
        {
            this.Payload.Select = "key";
            return this;
        }

        public IQueryBuilder<T> Skip(int? skip)
        {
            this.Payload.Skip = skip;
            return this;
        }

        public IQueryBuilder<T> Take(int? skip)
        {
            this.Payload.Take = skip;
            return this;
        }
    }
    class IndexedDbStore<T> : IIndexedDbStore<T> where T : class
    {
        private readonly string dbName;
        private readonly StoreSchema schema;
        private readonly ILogger logger;
        private readonly IMessageBus bus;

        public IndexedDbStore(string dbName, StoreSchema schema, ILogger logger, IMessageBus bus)
        {
            this.dbName = dbName;
            this.schema = schema;
            this.logger = logger;
            this.bus = bus;
        }

        public string Name => this.schema?.StoreName;

        public StoreSchema Schema => this.schema;

        public async Task<long> Count()
        {
            return (await this.bus
                .CreateContext(new StoreCountMessage(this.dbName, this.schema.StoreName))
                .Request())
                .GetPayload<long>();
        }

        public async Task EnsureStore()
        {

            var res = await this.bus.CreateContext(new CreateStoreMessage(this.dbName, this.schema))
                .Request();

        }

        public async Task<IEnumerable<T>> FetchAll(Action<IQueryBuilder<T>> query = null)
        {
            var msg = new StoreFetchMessage(this.dbName, this.schema);
            var q = new QueryBuilder<T>(msg.Payload);
            query?.Invoke(q);
            var res = await bus.CreateContext(msg)
                .Request();
            PropertyInfo keyProp = ShilaFeaturesExtensions.KeyProp(typeof(T));
            return res.GetPayload<StoreFetchResult>()
                .Items
                .Select(x => q.Payload?.Select == "key" ? ShilaFeaturesExtensions.SafeCastWithKey<T>(x, keyProp) :  ShilaFeaturesExtensions.SafeCast<T>(x));
        }

        public IAsyncQueryable<T> GetQueryable()
        {
            var result = new LinqQuery.Queryable<T>(new QueryExecutor<T>(this));

            return result;
        }

        public async Task Put(T[] items)
        {
            await this.bus.CreateContext(new StorePutMessage(this.dbName, this.schema, items))
                .Request();
        }
    }
}

using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Linq.Expressions;
using GN.Blazor.SharedIndexedDB.Messaging;
using GN.Blazor.SharedIndexedDB.Messaging.Commands;
using static GN.Blazor.SharedIndexedDB.Messaging.Commands.Fetch;
using GN.Blazor.SharedIndexedDB.IndexedDB.LinqQuery;
using static GN.Blazor.SharedIndexedDB.Messaging.Commands.DeleteRecordByID;
using GN.Blazor.SharedIndexedDB.IndexedDB.LinqQuery.Vistitors;

namespace GN.Blazor.SharedIndexedDB.IndexedDB
{
    public class QueryBuilder<T> : IQueryBuilder<T>
    {
        public QueryBuilder(FetchPayload Payload)
        {
            this.Payload = Payload;
        }

        public FetchPayload Payload { get; }

        public IQueryBuilder<T> Filter(Filter filter)
        {
            Payload.Filter = filter;
            return this;
        }

        public IQueryBuilder<T> OrderBy(string field, bool descending = false)
        {
            Payload.OrderBy = field?.ToLowerInvariant();
            Payload.Descending = descending ? true : null;
            return this;
        }

        public IQueryBuilder<T> SelectKeys()
        {
            Payload.Select = "key";
            return this;
        }

        public IQueryBuilder<T> Skip(int? skip)
        {
            Payload.Skip = skip;
            return this;
        }

        public IQueryBuilder<T> Take(int? skip)
        {
            Payload.Take = skip;
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

        public string Name => schema?.StoreName;

        public StoreSchema Schema => schema;

        public async Task<long> Count()
        {
            return (await bus
                .CreateContext(new Count(dbName, schema.StoreName))
                .Request())
                .GetPayload<long>();
        }

        public async Task EnsureStore()
        {

            var res = await bus.CreateContext(new CreateStore(dbName, schema))
                .Request();

        }

        public async Task<T> GetByID(string id)
        {
            try
            {
                var res = await bus.CreateContext(new GetRecordByID(id, dbName, Schema)).Request();
                var payload = res.GetPayload<T>();
                return payload;
            }
            catch (Exception err)
            {
                throw new Exception($"an error occured while geting the record {id} from {dbName}", err.InnerException);
            }


        }

        public async Task<IEnumerable<T>> FetchAll(Action<IQueryBuilder<T>> query = null)
        {
            var msg = new Fetch(dbName, schema);
            var q = new QueryBuilder<T>(msg.Payload);
            query?.Invoke(q);
            var res = await bus.CreateContext(msg)
                .Request();
            PropertyInfo keyProp = ShilaFeaturesExtensions.KeyProp(typeof(T));
            return res.GetPayload<StoreFetchResult>()
                .Items
                .Select(x => q.Payload?.Select == "key" ? ShilaFeaturesExtensions.SafeCastWithKey<T>(x, keyProp) : ShilaFeaturesExtensions.SafeCast<T>(x));
        }

        public IAsyncQueryable<T> GetQueryable()
        {
            var result = new LinqQuery.Queryable<T>(new QueryExecutor<T>(this));

            return result;
        }

        public async Task Put(T[] items)
        {
            await bus.CreateContext(new Put(dbName, schema, items))
                .Request();
        }

        public async Task<bool> DeleteByID(string id)
        {
            var m = new DeleteRecordByID(id, dbName, schema);
            var res = await bus.CreateContext(m).Request();
            var payload = res?.GetPayload<DeleteResult>();
            return payload.Success;
        }

        public async Task<bool> DeleteWhere(Expression<Predicate<T>> expression)
        {
            var evaluator = new FilterEvaluator();
            var filter = evaluator.Evaluate(expression);
            var message = new DeleteRecordWithFilter(filter, dbName, schema);
            var res = await bus.CreateContext(message).Request();
            var payload = res?.GetPayload<DeleteResult>();
            return payload.Success;
        }
    }
}

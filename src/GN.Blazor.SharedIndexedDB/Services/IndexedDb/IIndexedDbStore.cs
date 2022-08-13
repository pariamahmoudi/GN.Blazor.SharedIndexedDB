using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GN.Blazor.SharedIndexedDB.Models;
using GN.Blazor.SharedIndexedDB.Models.Messages;
using GN.Blazor.SharedIndexedDB.Services.LinqQuery;

namespace GN.Blazor.SharedIndexedDB.Services
{
    
    public interface IQueryBuilder<T>
    {
        IQueryBuilder<T> Skip(int? skip);
        IQueryBuilder<T> Take(int? take);
        IQueryBuilder<T> Filter(Filter filter);
        IQueryBuilder<T> OrderBy(string filed, bool descending = false);
        IQueryBuilder<T> SelectKeys();
    }
    public interface IIndexedDbStore<T> where T : class
    {
        public string Name { get; }
        public StoreSchema Schema { get; }
        Task Put(T[] items);
        Task<long> Count();
        Task<IEnumerable<T>> FetchAll(Action<IQueryBuilder<T>> query=null);
        IAsyncQueryable<T> GetQueryable();
        Task<T> GetByID(string id);

    }
}

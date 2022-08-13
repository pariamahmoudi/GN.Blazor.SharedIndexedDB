using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using GN.Blazor.SharedIndexedDB.Models;

namespace GN.Blazor.SharedIndexedDB.Services
{
    public interface IIndexedDb
    {
        Task<IIndexedDb> EnsureDatabase();
        Task<DatabaseSchema> GetSchema(bool refersh = false);
        Task<IIndexedDbStore<T>> GetStore<T>(StoreSchema schema) where T : class;
        Task<IIndexedDbStore<T>> GetStore<T>(string storeName = null, Expression<Func<T, object>> idSelector = null) where T : class;
        Task<IIndexedDbStore<object>> GetStore(StoreSchema schema);

    }
}

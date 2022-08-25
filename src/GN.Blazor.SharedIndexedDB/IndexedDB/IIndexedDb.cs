using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace GN.Blazor.SharedIndexedDB.IndexedDB
{
    /// <summary>
    /// the main database interface
    /// </summary>
    public interface IIndexedDb
    {
        /// <summary>
        /// makes sure database is available. will create a database otherwise
        /// </summary>
        Task<IIndexedDb> EnsureDatabase();
        /// <summary>
        /// gets the database's structure
        /// </summary>
        /// <param name="refersh">set to true if you wish to make sure you're getting the last update</param>
        Task<DatabaseSchema> GetSchema(bool refersh = false);
        /// <summary>
        /// will fetch or create a store(table) in the database with the provided schema
        /// </summary>
        /// <typeparam name="T">type of your object model</typeparam>
        /// <param name="schema">structure of the store</param>
        /// <returns>a store object for your model to start CRUD operations </returns>
        Task<IIndexedDbStore<T>> GetStore<T>(StoreSchema schema) where T : class;
        /// <summary>
        /// will fetch or create the store. will try to make schema automatically with the tags on your object
        /// model. see Attributes.cs
        /// </summary>
        /// <typeparam name="T">your object model</typeparam>
        /// <param name="storeName">the store name</param>
        /// <param name="idSelector"></param>
        /// <returns></returns>
        Task<IIndexedDbStore<T>> GetStore<T>(string storeName = null, Expression<Func<T, object>> idSelector = null) where T : class;
        /// <summary>
        /// will fetch or create a store for and unknown object type
        /// </summary>
        /// <param name="schema">structure ot the store</param>
        /// <returns></returns>
        Task<IIndexedDbStore<object>> GetStore(StoreSchema schema);

    }
}

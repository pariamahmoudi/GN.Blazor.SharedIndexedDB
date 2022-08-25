using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;


namespace GN.Blazor.SharedIndexedDB.IndexedDB
{
    /// <summary>
    /// To manage query support of the store. 
    /// </summary>
    /// <typeparam name="T">store's model type</typeparam>
    public interface IQueryBuilder<T>
    {
        /// <summary>
        /// skip X records
        /// </summary>
        /// <param name="skip">number of records to skip</param>
        /// <returns>the queryable object</returns>
        IQueryBuilder<T> Skip(int? skip);
        /// <summary>
        /// take X records
        /// </summary>
        /// <param name="take">number of records to take</param>
        /// <returns>the queryable object</returns>
        IQueryBuilder<T> Take(int? take);
        /// <summary>
        /// filter records
        /// </summary>
        /// <param name="filter">the filter that should be applied to the records</param>
        /// <returns>the queryable object</returns>
        IQueryBuilder<T> Filter(Filter filter);
        /// <summary>
        /// Order by a field 
        /// </summary>
        /// <param name="field">name of the field(column) in store</param>
        /// <param name="descending">set to true if the order should be descending</param>
        /// <returns>the queryable object</returns>
        IQueryBuilder<T> OrderBy(string field, bool descending = false);
        /// <summary>
        /// Select keys
        /// </summary>
        /// <returns>the queryable object</returns>
        IQueryBuilder<T> SelectKeys();
    }
    /// <summary>
    /// The main store interface
    /// </summary>
    /// <typeparam name="T">The model of your store</typeparam>
    public interface IIndexedDbStore<T> where T : class
    {
        /// <summary>
        /// the name of your store
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// store schema. see StoreSchema for more info
        /// </summary>
        public StoreSchema Schema { get; }
        /// <summary>
        /// Add record(s) to store
        /// </summary>
        /// <param name="items">Array of records to be added</param>
        Task Put(T[] items);
        /// <summary>
        /// Counts records in store
        /// </summary>
        /// <returns>The number of record in store</returns>
        Task<long> Count();
        /// <summary>
        /// Fetch records which match the condition
        /// </summary>
        /// <param name="query">the condition of yout fetch</param>
        /// <returns>an enumerable of your model based on the condition</returns>
        Task<IEnumerable<T>> FetchAll(Action<IQueryBuilder<T>> query = null);
        /// <summary>
        /// Gets a queryable of the store
        /// </summary>
        /// <returns>An AsyncQueryable object for all the records</returns>
        IAsyncQueryable<T> GetQueryable();
        /// <summary>
        /// Gets the record as your object model from store using it's key 
        /// </summary>
        /// <param name="id">Record's key</param>
        /// <returns>The record as your model</returns>
        Task<T> GetByID(string id);
        /// <summary>
        /// Deletes the object with provided id as it's key
        /// </summary>
        /// <param name="id">key value of the object</param>
        /// <returns>true or false if the deletion was successful</returns>
        Task<bool> DeleteByID(string id);
        /// <summary>
        /// deletes record which match the expression
        /// </summary>
        /// <param name="expression">deletion's rule</param>
        /// <returns>true or false if the deletion was successful</returns>
        Task<bool> DeleteWhere(Expression<Predicate<T>> expression);
    }
}

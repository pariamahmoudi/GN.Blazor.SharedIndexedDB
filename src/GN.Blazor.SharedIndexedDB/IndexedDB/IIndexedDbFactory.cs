using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using GN.Blazor.SharedIndexedDB.Messaging;
using GN.Blazor.SharedIndexedDB.Messaging.Commands;

namespace GN.Blazor.SharedIndexedDB.IndexedDB
{
    /// <summary>
    /// DataBase factory
    /// </summary>
    public interface IIndexedDbFactory
    {
        /// <summary>
        /// get a fresh new database
        /// </summary>
        /// <param name="dbName">name of your database</param>
        Task<IIndexedDb> GetDatabase(string dbName);
        /// <summary>
        /// delete an existing database
        /// </summary>
        /// <param name="dbName">database name</param>
        Task DeleteDatabase(string dbName);
        /// <summary>
        /// checks if a database exists
        /// </summary>
        /// <param name="dbName">database name</param>
        /// <returns></returns>
        Task<bool> DatabaseExists(string dbName);
    }
    class IndexedDbFactory : IIndexedDbFactory
    {
        private readonly IServiceProvider serviceProvider;

        public IndexedDbFactory(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }
        public async Task<IIndexedDb> GetDatabase(string dbName)
        {
            await Task.CompletedTask;
            return new IndexedDb(dbName, serviceProvider.GetService<ILogger<IndexedDb>>(), serviceProvider.GetService<IMessageBus>());
        }

        public async Task<bool> DatabaseExists(string dbName)
        {
            return (await serviceProvider.GetService<IMessageBus>()
               .CreateContext(new DatabaseExsists(dbName))
               .Request())
               .GetPayload<bool>();

        }

        public Task DeleteDatabase(string dbName)
        {
            return serviceProvider.GetService<IMessageBus>()
                .CreateContext(new DeleteDatabase(dbName))
                .Request();
        }
    }
}

using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using GN.Blazor.SharedIndexedDB.Models.Messages;

namespace GN.Blazor.SharedIndexedDB.Services
{
    public interface IIndexedDbFactory
    {
        Task<IIndexedDb> GetDatabase(string dbName);
        Task DeleteDatabase(string dbName);
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
                .CreateContext(new DeleteDatabaseMessage(dbName))
                .Request();
        }
    }
}

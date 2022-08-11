using Microsoft.AspNetCore.Components;
using GN.Blazor.SharedIndexedDB;
using GN.Blazor.SharedIndexedDB.Models;
using GN.Blazor.SharedIndexedDB.Models.Messages;
using GN.Blazor.SharedIndexedDB.Services;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GN.Blazor.SharedIndexedDB.Tests.Pages
{
    public partial class MessageBusTests
    {
        private StringBuilder _log = new StringBuilder();
        [Inject]
        private IMessageBus Bus { get; set; }
        public void ClearLog()
        {
            this._log = new StringBuilder();
            this.StateHasChanged();

        }
        public void Log(string fmt)
        {
            //_log.AppendFormat(fmt, args);
            _log.AppendLine(fmt);
            this.StateHasChanged();
        }
        public void Assert(bool check, string comment)
        {
            if (!check)
                throw new Exception(comment);
        }
        public async Task Publish()
        {
            var topic = "test";
            var received = new List<Message>();
            await this.Bus.Subscribe(topic, ctx =>
            {
                var m = ctx.Message;
                Log($"Item received. {m.GetPayload<string>()}");
                received.Add(m);
            });
            await this.Bus.CreateContext(topic, "hi there")
                .Publish(MessageScope.AllScopes.Only(Transports.BusSubscriptions));
            Assert(received.Count == 1, "");
        }
        public async Task Request()
        {
            var topic = "test";
            var received = new List<Message>();
            var res = await this.Bus.CreateContext(new CreateDatabaseMessage("babak"))
                .WithScope(s => s.All().Only(Transports.SharedWorker))
                .Request();
            Log($"Database created. {res}");
        }
        public async Task CreateStore()
        {
            try
            {
                var dbName = "mydatabase2";
                var response = await this.Bus.CreateContext(new CreateStoreMessage(dbName, new StoreSchema
                {
                    //DbName = "mydatabase2",
                    StoreName = "mystore",
                    PrimaryKey = new IndexData("id"),
                    Indexes = new IndexData[] { new IndexData("name", "name", false) },
                }))
                    .Request()
                    .TimeoutAfter(5000);
                Log($"Store Created: {response}");


                var response2 = await this.Bus.CreateContext(new GetDatabaseSchema(dbName))
                   .Request()
                   .TimeoutAfter(5000);
                Log($"Schema Created: {response2}");
            }
            catch (TimeoutException)
            {
                Log($"Failed: Timeout. ");
            }
        }
        public async Task StoreCRUD()
        {
            var dbName = "ContactsDB";
            var schema = new StoreSchema
            {
                //DbName = "ContactsDB1",
                StoreName = "Contats",
                PrimaryKey = new IndexData("id"),
                Indexes = new IndexData[] { new IndexData("age","age",false) }
            };
            try
            {
                var store = await this.Bus
                    .CreateContext(new CreateStoreMessage(dbName,schema))
                    .Request()
                    .TimeoutAfter(5000);

                Log("Store Created!");
                var items = Enumerable.Range(0, 10000)
                    .Select(idx => new PersonModel
                    {
                        Name = $"name {new Random().Next(1, 10000)} ",
                        Id = idx.ToString(),
                        Age = new Random().Next(10, 70)
                    })
                    .ToArray();
                Log($"Store Successfully Created {store}");
                var put_response = await this.Bus
                    .CreateContext(new StorePutMessage(dbName, schema, items))
                    .Request()
                    .TimeoutAfter(10000);
                Log($"Put Succeeded: {put_response}");
            }
            catch (Exception err)
            {
                Log($"Failed {err}");
            }
        }

    }
}

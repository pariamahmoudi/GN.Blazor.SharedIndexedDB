using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GN.Blazor.SharedIndexedDB.Models.Messages
{
    public class CreateStoreMessagePayload
    {
        public string DbName { get; set; }
        public StoreSchema Schema { get; set; }

    }
    public class CreateStoreMessage : Message<CreateStoreMessagePayload>
    {
        public CreateStoreMessage(string dbName, StoreSchema schema) : base(Subjects.CreateStore, new CreateStoreMessagePayload
        {
            DbName = dbName,
            Schema = schema
        })
        {

        }
    }
}

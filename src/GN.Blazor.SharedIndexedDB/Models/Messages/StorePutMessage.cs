using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GN.Blazor.SharedIndexedDB.Models.Messages
{
    public class StoreSchemaWithDbName
    {
        public StoreSchema Schema { get; set; }
        public string DbName { get; set; }

    }
    public class StorePutPayload : StoreSchemaWithDbName
    {

        public object[] Items { get; set; }
    }
    public class StorePutMessage : Message<StorePutPayload>
    {
        public StorePutMessage(string dbName, StoreSchema schema, object[] items) :
            base(Subjects.StorePut, new StorePutPayload { DbName = dbName, Schema = schema, Items = items })
        {

        }
    }
}

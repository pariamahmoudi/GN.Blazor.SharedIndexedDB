using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GN.Blazor.SharedIndexedDB.Models.Messages
{
    public class StoreCountPayload
    {
        public String DbName { get; set; }
        public StoreSchema Schema { get; set; }
    }
    public class StoreCountMessage : Message<StoreCountPayload>
    {
        public StoreCountMessage(string dbName, string store) : base(Subjects.StoreCount, new StoreCountPayload
        {
            DbName = dbName,
            Schema = new StoreSchema { StoreName = store }
        })
        { }
    }
}

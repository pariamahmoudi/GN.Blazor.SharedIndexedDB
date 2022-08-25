using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GN.Blazor.SharedIndexedDB.IndexedDB;

namespace GN.Blazor.SharedIndexedDB.Messaging.Commands
{
    /// <summary>
    /// Count the records in store message
    /// </summary>
    public class Count : Message<Count.CountPayload>
    {
        /// <summary>
        /// count the records in a store command
        /// </summary>
        /// <param name="dbName">database name</param>
        /// <param name="store">store name</param>
        public Count(string dbName, string store) : base(Subjects.Count, new CountPayload
        {
            DbName = dbName,
            Schema = new StoreSchema { StoreName = store }
        })
        { }
        /// <summary>
        /// count command payload
        /// </summary>
        public class CountPayload
        {
            /// <summary>
            /// name of the database
            /// </summary>
            public string DbName { get; set; }
            /// <summary>
            /// schema of the store
            /// </summary>
            public StoreSchema Schema { get; set; }
        }
    }
}

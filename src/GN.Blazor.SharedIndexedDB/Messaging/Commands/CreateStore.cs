using GN.Blazor.SharedIndexedDB.IndexedDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GN.Blazor.SharedIndexedDB.Messaging.Commands
{

    /// <summary>
    /// create store message
    /// </summary>
    public class CreateStore : Message<CreateStore.CreateStorePayload>
    {
        /// <summary>
        /// request a new store to be created
        /// </summary>
        /// <param name="dbName">the name of the database to add the store to</param>
        /// <param name="schema">the schema of the store</param>
        public CreateStore(string dbName, StoreSchema schema) : base(Subjects.CreateStore, new CreateStorePayload
        {
            DbName = dbName,
            Schema = schema
        })
        { }
        /// <summary>
        /// Create store command payload
        /// </summary>
        public class CreateStorePayload
        {
            /// <summary>
            /// name of the database you want to add the store to
            /// </summary>
            public string DbName { get; set; }
            /// <summary>
            /// store schema
            /// </summary>
            public StoreSchema Schema { get; set; }

        }
    }
}

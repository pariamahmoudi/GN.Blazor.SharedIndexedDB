using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GN.Blazor.SharedIndexedDB.IndexedDB;

namespace GN.Blazor.SharedIndexedDB.Messaging.Commands
{
    /// <summary>
    /// add record(s) message
    /// </summary>
    public class Put : Message<Put.PutPayload>
    {
        /// <summary>
        /// request to adds record(s) to the store command
        /// </summary>
        /// <param name="dbName">database name</param>
        /// <param name="schema">store schema</param>
        /// <param name="items">array of record(s) to be added</param>
        public Put(string dbName, StoreSchema schema, object[] items) :
            base(Subjects.Put, new PutPayload { DbName = dbName, Schema = schema, Items = items })
        {

        }
        /// <summary>
        /// add record(s) to store command peyload
        /// </summary>
        public class PutPayload : StoreSchemaWithDbNamePayload
        {
            /// <summary>
            /// array of record(s)
            /// </summary>
            public object[] Items { get; set; }
        }
    }
}

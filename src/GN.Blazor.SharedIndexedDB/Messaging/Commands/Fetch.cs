using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GN.Blazor.SharedIndexedDB.IndexedDB;

namespace GN.Blazor.SharedIndexedDB.Messaging.Commands
{

    /// <summary>
    /// Fetch from store message
    /// </summary>
    public class Fetch : Message<Fetch.FetchPayload>
    {
        /// <summary>
        /// request to fetch redord(s) from a store with condition command
        /// </summary>
        /// <param name="dbName">database name</param>
        /// <param name="schema">store schema</param>
        /// <param name="query">the condition of fetch</param>
        public Fetch(string dbName, StoreSchema schema, Action<FetchPayload> query = null) : base(Subjects.Fetch, new FetchPayload
        {
            DbName = dbName,
            Schema = schema,
        })
        {
            query?.Invoke(Payload);
        }
        /// <summary>
        /// fetch command payload 
        /// </summary>
        public class FetchPayload : StoreSchemaWithDbNamePayload
        {
            /// <summary>
            /// Skip x records()
            /// </summary>
            public int? Skip { get; set; }
            /// <summary>
            /// Take x record(s)
            /// </summary>
            public int? Take { get; set; }
            /// <summary>
            /// the condition as a filter object
            /// </summary>
            public Filter Filter { get; set; }
            /// <summary>
            /// order by a field 
            /// </summary>
            public string OrderBy { get; set; }
            /// <summary>
            /// sort records in descending order
            /// </summary>
            public bool? Descending { get; set; }
            /// <summary>
            /// select a field
            /// </summary>
            public string Select { get; set; }
        }
        /// <summary>
        /// result of the fetch command. an array of record(s)
        /// </summary>
        public class StoreFetchResult
        {
            /// <summary>
            /// array of fetched records 
            /// </summary>
            public object[] Items { get; set; }

        }
    }
}

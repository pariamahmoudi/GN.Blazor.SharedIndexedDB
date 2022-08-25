using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GN.Blazor.SharedIndexedDB.IndexedDB;

namespace GN.Blazor.SharedIndexedDB.Messaging.Commands
{
    /// <summary>
    /// Delete record(s) with condition message
    /// </summary>
    public class DeleteRecordWithFilter : Message<DeleteRecordWithFilter.DeleteRecordWithFilterPayload>
    {
        /// <summary>
        /// request to delete record(s) with condition command
        /// </summary>
        /// <param name="filter">the condition as a filter object</param>
        /// <param name="dbName">database name</param>
        /// <param name="schema">store schema</param>
        public DeleteRecordWithFilter(Filter filter, string dbName, StoreSchema schema) : base(Subjects.DeleteWithExpression, new DeleteRecordWithFilterPayload
        {
            Filter = filter,
            DbName = dbName,
            Schema = schema
        })
        { }
        /// <summary>
        /// Delete record(s) with condition payload
        /// </summary>
        public class DeleteRecordWithFilterPayload
        {
            /// <summary>
            /// the condition as a filter object
            /// </summary>
            public Filter Filter { get; set; }
            /// <summary>
            /// database name
            /// </summary>
            public string DbName { get; set; }
            /// <summary>
            /// store schema
            /// </summary>
            public StoreSchema Schema { get; set; }

        }
    }
}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GN.Blazor.SharedIndexedDB.IndexedDB;

namespace GN.Blazor.SharedIndexedDB.Messaging.Commands
{
    /// <summary>
    /// delete record by it's primary key message
    /// </summary>
    public class DeleteRecordByID : Message<DeleteRecordByID.DeleteRecordByIDPayload>
    {
        /// <summary>
        /// request a record to be deleted by it's primary key command
        /// </summary>
        /// <param name="ID">primary key value</param>
        /// <param name="dbName">database name </param>
        /// <param name="schema">store schema</param>
        public DeleteRecordByID(string ID, string dbName, StoreSchema schema) : base(Subjects.DeleteById, new DeleteRecordByIDPayload
        {
            ID = ID,
            DBName = dbName,
            Schema = schema
        })
        { }
        /// <summary>
        /// delete record by it's id payload
        /// </summary>
        public class DeleteRecordByIDPayload
        {
            /// <summary>
            /// value of the primary key of the record
            /// </summary>
            public string ID { get; set; }
            /// <summary>
            /// database name
            /// </summary>
            public string DBName { get; set; }
            /// <summary>
            /// store schema
            /// </summary>
            public StoreSchema Schema { get; set; }
        }
        /// <summary>
        /// deletation result payload
        /// </summary>
        public class DeleteResult
        {
            /// <summary>
            /// success of the deletation
            /// </summary>
            public bool Success { get; set; }
        }
    }





}

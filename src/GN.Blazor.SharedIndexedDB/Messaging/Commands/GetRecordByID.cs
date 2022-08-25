using GN.Blazor.SharedIndexedDB.IndexedDB;

namespace GN.Blazor.SharedIndexedDB.Messaging.Commands
{
    /// <summary>
    /// Get record by its primary key message
    /// </summary>
    public class GetRecordByID : Message<GetRecordByID.GetRecordByIdPayload>
    {
        /// <summary>
        /// requet a record using its primary key value command
        /// </summary>
        /// <param name="iD">primary key value of the record</param>
        /// <param name="dbName">database name</param>
        /// <param name="schema">store schema</param>
        public GetRecordByID(string iD, string dbName, StoreSchema schema) : base(Subjects.GetRecordByID, new GetRecordByIdPayload
        {
            DBName = dbName,
            ID = iD,
            Schema = schema
        })
        { }
        /// <summary>
        /// get a record by its primary key command payload
        /// </summary>
        public class GetRecordByIdPayload
        {
            /// <summary>
            /// primary key value
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
    }

}

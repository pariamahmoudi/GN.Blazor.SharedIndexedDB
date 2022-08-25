namespace GN.Blazor.SharedIndexedDB.Messaging.Commands
{
    /// <summary>
    /// retrieve database message
    /// </summary>
    public class GetDatabaseSchema : Message<GetDatabaseSchema.GetDatabaseSchemaPayload>
    {
        /// <summary>
        /// retrive a database command
        /// </summary>
        /// <param name="dbName">database name </param>
        public GetDatabaseSchema(string dbName) : base(Subjects.GetDatabaseSchema, new GetDatabaseSchemaPayload { DbName = dbName })
        {
        }
        /// <summary>
        /// retrieve database command payload
        /// </summary>
        public class GetDatabaseSchemaPayload
        {
            /// <summary>
            /// database name
            /// </summary>
            public string DbName { get; set; }
        }
    }
}

namespace GN.Blazor.SharedIndexedDB.Models.Messages
{
    public class GetDatabaseSchema : Message<GetDatabaseSchemaPayload>
    {
        public GetDatabaseSchema(string dbName) : base(Subjects.GetDatabaseSchema, new GetDatabaseSchemaPayload { DbName = dbName })
        {
        }
    }
}

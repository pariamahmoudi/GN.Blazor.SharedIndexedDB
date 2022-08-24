namespace GN.Blazor.SharedIndexedDB.Models.Messages
{
    public class GetRecordByIdPayload
    {
        public string ID { get; set; }
        public string DBName { get; set; }
        public StoreSchema Schema { get; set; }
    }
    public class GetRecordByID : Message<GetRecordByIdPayload>
    {
        public GetRecordByID(string iD, string dbName, StoreSchema schema) : base(Subjects.GetRecordByID, new GetRecordByIdPayload
        {
            DBName = dbName,
            ID = iD,
            Schema = schema
        })
        { }
    }

}

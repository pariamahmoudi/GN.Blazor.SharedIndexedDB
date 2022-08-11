namespace GN.Blazor.SharedIndexedDB.Models
{
    public class StoreSchema
    {
        //public string DbName { get; set; }
        public string StoreName { get; set; }
        public IndexData PrimaryKey { get; set; }
        
        public IndexData[] Indexes { get; set; }
        public static StoreSchema GetShilaDefault(string channelName)
        {
            return new StoreSchema
            {
                StoreName = channelName,
                PrimaryKey = new IndexData("primary", "uniqueId", true)
            };
        }
    }
}

namespace GN.Blazor.SharedIndexedDB.Models
{
    public class DatabaseSchema
    {
        public string DbName { get; set; }
        public StoreSchema[] Stores { get; set; }
    }
}

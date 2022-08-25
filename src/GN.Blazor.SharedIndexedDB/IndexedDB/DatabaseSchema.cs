namespace GN.Blazor.SharedIndexedDB.IndexedDB
{
    /// <summary>
    /// DataBase structure
    /// </summary>
    public class DatabaseSchema
    {
        /// <summary>
        /// name of the database
        /// </summary>
        public string DbName { get; set; }
        /// <summary>
        /// list of it's store(table) schemas.
        /// </summary>
        public StoreSchema[] Stores { get; set; }
    }
}

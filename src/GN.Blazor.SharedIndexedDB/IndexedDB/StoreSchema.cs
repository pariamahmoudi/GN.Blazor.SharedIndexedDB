namespace GN.Blazor.SharedIndexedDB.IndexedDB
{
    /// <summary>
    /// structure of store(table)
    /// </summary>
    public class StoreSchema
    {
        //public string DbName { get; set; }
        /// <summary>
        /// Name of the store
        /// </summary>
        public string StoreName { get; set; }
        /// <summary>
        /// set the public key of the store.it must be unqiue
        /// </summary>
        public IndexData PrimaryKey { get; set; }
        /// <summary>
        /// the field in store that should be indexed
        /// </summary>
        public IndexData[] Indexes { get; set; }
        /// <summary>
        /// Test StoreSchema
        /// </summary>
        /// <param name="channelName">store name</param>
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

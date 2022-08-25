using GN.Blazor.SharedIndexedDB.IndexedDB;

namespace GN.Blazor.SharedIndexedDB.Messaging.Commands
{
    /// <summary>
    /// a class which can contain the payload every request to indexedDB needs
    /// </summary>
    public class StoreSchemaWithDbNamePayload
    {
        /// <summary>
        /// store schema
        /// </summary>
        public StoreSchema Schema { get; set; }
        /// <summary>
        /// database name
        /// </summary>
        public string DbName { get; set; }

    }
}

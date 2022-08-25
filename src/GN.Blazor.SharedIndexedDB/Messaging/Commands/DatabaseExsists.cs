namespace GN.Blazor.SharedIndexedDB.Messaging.Commands
{
    /// <summary>
    /// check the existance of the database message
    /// </summary>
    public class DatabaseExsists : Message<string>
    {
        /// <summary>
        /// request to check if the database exists command
        /// </summary>
        /// <param name="dbName">database name </param>
        public DatabaseExsists(string dbName) : base(Subjects.DatabaseExists, dbName)
        {

        }
    }
}

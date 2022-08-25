using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GN.Blazor.SharedIndexedDB.Messaging.Commands
{
    /// <summary>
    /// Create databse message
    /// </summary>
    public class CreateDatabase : Message<CreateDatabase.CreateDatabasePayload>
    {
        /// <summary>
        /// request a new database to be created command
        /// </summary>
        /// <param name="dataBaseName">the name of the database</param>
        public CreateDatabase(string dataBaseName)
        {
            Subject = Subjects.CreateDatabase;
            Payload = new CreateDatabasePayload()
            {
                DatabaseName = dataBaseName
            };
        }
        /// <summary>
        /// create database command peyload
        /// </summary>
        public class CreateDatabasePayload
        {
            /// <summary>
            /// name of the database
            /// </summary>
            public string DatabaseName { get; set; }

        }
    }
}

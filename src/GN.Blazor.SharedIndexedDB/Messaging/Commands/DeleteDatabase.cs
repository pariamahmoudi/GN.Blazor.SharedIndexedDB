using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GN.Blazor.SharedIndexedDB.Messaging.Commands
{
    /// <summary>
    /// delete database message
    /// </summary>
    public class DeleteDatabase : Message<string>
    {
        /// <summary>
        /// request a database to be deleted
        /// </summary>
        /// <param name="dbName">database name </param>
        public DeleteDatabase(string dbName) : base(Subjects.DeleteDatabase, dbName)
        {

        }
    }
}

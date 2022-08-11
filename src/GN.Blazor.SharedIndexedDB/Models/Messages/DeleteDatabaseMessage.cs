using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GN.Blazor.SharedIndexedDB.Models.Messages
{
    public class DeleteDatabaseMessage:Message<string>
    {
        public DeleteDatabaseMessage(string dbName) : base(Subjects.DeleteDatabase, dbName)
        {

        }
    }
    public class DatabaseExsists : Message<string>
    {
        public DatabaseExsists(string dbName) : base(Subjects.DatabaseExists, dbName)
        {

        }
    }
}

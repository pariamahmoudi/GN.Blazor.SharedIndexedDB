using GN.Blazor.SharedIndexedDB.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GN.Blazor.SharedIndexedDB.Models.Messages
{
    public class CreateDatabaseMessage : Message<CreateDatabaseMessage.CreateDatabaseMessagePayload>
    {
        public CreateDatabaseMessage(string dataBaseName)
        {
            Subject = Subjects.CreateDatabase;
            Payload = new CreateDatabaseMessagePayload()
            {
                DatabaseName = dataBaseName
            };
        }
        public class CreateDatabaseMessagePayload
        {
            public string DatabaseName { get; set; }

        }
    }
}

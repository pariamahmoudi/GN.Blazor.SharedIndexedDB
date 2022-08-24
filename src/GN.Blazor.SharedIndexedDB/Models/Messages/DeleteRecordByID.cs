using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GN.Blazor.SharedIndexedDB.Models.Messages
{
    public class DeleteRecordByID : Message<DeleteRecordByIDPayload>
    {
        public DeleteRecordByID(string ID, string dbName, StoreSchema schema) : base(Subjects.DeleteById, new DeleteRecordByIDPayload
        {
            ID = ID,
            DBName = dbName,
            Schema = schema
        })
        { }
    }

    public class DeleteRecordByIDPayload
    {
        public string ID { get; set; }
        public string DBName { get; set; }
        public StoreSchema Schema { get; set; }
    }

    public class DeleteResult
    {
        public bool Success { get; set; }
    }


  

}

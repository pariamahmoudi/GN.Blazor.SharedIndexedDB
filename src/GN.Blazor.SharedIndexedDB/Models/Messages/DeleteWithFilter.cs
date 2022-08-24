using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GN.Blazor.SharedIndexedDB.Models.Messages
{
    public class DeleteRecordWithFilterPayload
    {
        public Filter Filter { get; set; }
        public string DbName { get;set; }
        public StoreSchema schema { get; set; }

    }
    public class DeleteRecordWithFilter : Message<DeleteRecordWithFilterPayload>
    {
        public DeleteRecordWithFilter(Filter filter, string dbName, StoreSchema schema) : base(Subjects.DeleteWithExpression, new DeleteRecordWithFilterPayload
        {
            Filter = filter,
            DbName = dbName,
            schema = schema
        })
        { }
    }
}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GN.Blazor.SharedIndexedDB.Models.Messages
{
    public class DeleteRecordByID : Message<string>
    {
        public DeleteRecordByID(string ID) : base(Subjects.DeleteById, ID)
        {

        }
    }
    public class GetRecordByIdPayload
    {
        public string ID { get; set; }
        public string DBName { get; set; }
    }
    public class GetRecordByID : Message<GetRecordByIdPayload>
    {
        public GetRecordByID(string ID, string DBName) : base(Subjects.GetRecordByID, new GetRecordByIdPayload
        {
            DBName = DBName,
            ID = ID
        })
        { }
    }

}

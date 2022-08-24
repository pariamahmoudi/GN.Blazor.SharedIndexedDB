using GN.Blazor.SharedIndexedDB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GN.Blazor.SharedIndexedDB.Tests.Models
{
    [Table("TestModel")]

    public class TestModel
    {
        [Key]
        public Guid GuID { get; set; }
        public int Number { get; set; }
        public static TestModel[] GetSampleData(int count = 5)
        {
            var data = new List<TestModel>();
            for (var i = 0; i < count; i++)
            {
                data.Add(new TestModel
                {
                    GuID = Guid.NewGuid(),
                    Number = i,
                });
            }
            return data.ToArray();
        }
    }

}

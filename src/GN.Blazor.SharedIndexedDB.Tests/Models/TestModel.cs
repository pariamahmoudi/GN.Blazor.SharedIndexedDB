using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GN.Blazor.SharedIndexedDB.Tests.Models
{
    [Table("TestModel")]

    public class TestModel
    {
        [Key()]
        public int Guid { get; set; }
        public int Number { get; set; }
        public static TestModel[] GetSampleData()
        {
            var data = new List<TestModel>();
            for (var i = 0; i < 10; i++)
            {
                data.Add(new TestModel
                {
                    Guid = i,
                    Number = i,
                });
            }
            return data.ToArray();
        }
    }

}

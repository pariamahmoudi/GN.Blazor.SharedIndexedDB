using GN.Blazor.SharedIndexedDB.Models;
//using System.ComponentModel.DataAnnotations.Schema;

namespace GN.Blazor.SharedIndexedDB.Tests.Pages
{
    [Table("Persons")]
    public class PersonModel
    {
        [Key()]
        public string Id { get; set; }
        
        [Index()]
        public string Name { get; set; }
        
        [Index()]
        public int Age { get; set; }
    }
}

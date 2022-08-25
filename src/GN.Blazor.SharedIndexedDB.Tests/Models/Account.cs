using GN.Blazor.SharedIndexedDB.IndexedDB;
using System;
using System.Linq;

namespace GN.Blazor.SharedIndexedDB.Tests.Models
{
    [Table("Accounts")]
    public class Account
    {
        [Key]
        public int Id { get; set; }

        [Index]
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string City { get; set; }

        private static Random random = new Random();
        internal static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        public static Account[] Seed(int count)
        {
            return Enumerable.Range(1, count)
                .Select(x =>
                     new Account
                     {
                         Id = x,
                         Name = RandomString(20)
                     }
                ).ToArray();
        }
    }
}

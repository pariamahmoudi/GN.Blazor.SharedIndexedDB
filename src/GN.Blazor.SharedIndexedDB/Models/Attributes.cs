using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GN.Blazor.SharedIndexedDB.Models
{
    [AttributeUsage(AttributeTargets.Class)]
    public class TableAttribute :Attribute
    {
        public string Name { get; }
        public TableAttribute(string name)
        {
            this.Name = name;
        }
    }
    [AttributeUsage(AttributeTargets.Property)]
    public class KeyAttribute : Attribute
    {
        public string Name { get; }
        public KeyAttribute(string name = null)
        {
            this.Name = name;
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class IndexAttribute : Attribute
    {
        public string Name { get; }
        public bool Unique { get; }
        public IndexAttribute(string name=null, bool unique = false)
        {
            this.Name = name;
            this.Unique = unique;
        }
    }



}

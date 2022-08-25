using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GN.Blazor.SharedIndexedDB.IndexedDB
{
    /// <summary>
    /// Table Name in DataBase
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class TableAttribute : Attribute
    {
        /// <summary>
        /// sets name of database
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// sets table name of the model in database
        /// </summary>
        /// <param name="name">name of the table in database</param>
        public TableAttribute(string name)
        {
            Name = name;
        }
    }

    /// <summary>
    /// sets public key tag. you can use it with a constructor to use a different name.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class KeyAttribute : Attribute
    {
        /// <summary>
        /// name of the field which has the key
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// sets the name of the public key field in database
        /// </summary>
        /// <param name="name">name of the public key field</param>
        public KeyAttribute(string name = null)
        {
            Name = name;
        }
    }

    /// <summary>
    /// set the field to be indexed, use it with a constructor to have a custom name and IsUnique tag
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class IndexAttribute : Attribute
    {
        /// <summary>
        /// name of the field
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// set to true if your values are unique for every record
        /// </summary>
        public bool Unique { get; }
        /// <summary>
        /// will create and index the field
        /// </summary>
        /// <param name="name">name if the field</param>
        /// <param name="unique">are values unique</param>
        public IndexAttribute(string name = null, bool unique = false)
        {
            Name = name;
            Unique = unique;
        }
    }



}

namespace GN.Blazor.SharedIndexedDB.Models
{
    public class IndexData
    {
        public string Name { get; set; }
        public string KeyPath { get; set; }
        public bool Unique { get; set; }
        public IndexData() { }
        public IndexData(string keyPath , string name=null, bool unique=false)
        {
            this.Name = name;
            this.KeyPath = keyPath;
            this.Unique = unique;

        }

    }
}

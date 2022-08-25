namespace GN.Blazor.SharedIndexedDB.IndexedDB
{
    public class IndexData
    {
        public string Name { get; set; }
        public string KeyPath { get; set; }
        public bool Unique { get; set; }
        public IndexData() { }
        public IndexData(string keyPath, string name = null, bool unique = false)
        {
            Name = name;
            KeyPath = keyPath;
            Unique = unique;

        }

    }
}

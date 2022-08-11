namespace GN.Blazor.SharedIndexedDB.Models
{
    public class ShareWorkerAdapterOptions
    {
        public object CallbackAdapter { get; set; }
        
        public string SharedWorkerUrl { get; set; }
        public string OnMessageMethodName { get; set; }
    }
}

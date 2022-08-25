namespace GN.Blazor.SharedIndexedDB.SharedWorker
{
    public class ShareWorkerAdapterOptions
    {
        public object CallbackAdapter { get; set; }

        public string SharedWorkerUrl { get; set; }
        public string OnMessageMethodName { get; set; }
    }
}

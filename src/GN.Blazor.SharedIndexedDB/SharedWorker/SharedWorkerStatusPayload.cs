namespace GN.Blazor.SharedIndexedDB.SharedWorker
{
    public class SharedWorkerStatusPayload
    {
        public SubscriptionData[] Subscriptions { get; set; }
        public ShareWorkerAdapterOptions Options { get; set; }

    }
}

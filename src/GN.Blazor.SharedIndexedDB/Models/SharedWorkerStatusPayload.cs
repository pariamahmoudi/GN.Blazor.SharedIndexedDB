namespace GN.Blazor.SharedIndexedDB.Models
{
    public class SharedWorkerStatusPayload
    {
        public SubscriptionData[] Subscriptions { get; set; }
        public ShareWorkerAdapterOptions Options { get; set; }

    }
}

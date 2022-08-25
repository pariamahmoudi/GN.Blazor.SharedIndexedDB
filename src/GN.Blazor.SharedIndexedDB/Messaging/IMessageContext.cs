using System;
using System.Threading.Tasks;

namespace GN.Blazor.SharedIndexedDB.Messaging
{
    public interface IMessageContext
    {
        Task Publish(MessageScope scope = null);
        Task<Message> Request(MessageScope scope = null);
        IMessageContext WithScope(Action<MessageScope> action);
        Transports Transport { get; }
        Message Message { get; }



    }
}

using System;
using System.Threading.Tasks;

namespace GN.Blazor.SharedIndexedDB.Messaging
{
    class MessageContext : IMessageContext
    {
        private readonly MessageBus bus;
        private readonly Message message;
        private MessageScope scope;
        public Message Message => message;
        public Transports Transport { get; private set; }
        public MessageContext(MessageBus bus, Message message, Transports transport = Transports.Unknown)
        {
            this.bus = bus;
            this.message = message;
            scope = MessageScope.AllScopes;
            Transport = transport;
        }

        public Task Publish(MessageScope scope = null)
        {
            return bus.Publish(this, scope ?? this.scope);
        }

        public Task<Message> Request(MessageScope scope = null)
        {
            return bus.Request(this, scope ?? this.scope);
        }
        public IMessageContext WithScope(Action<MessageScope> action)
        {
            action?.Invoke(scope);
            return this;
        }
    }
}

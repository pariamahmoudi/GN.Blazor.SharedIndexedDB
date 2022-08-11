using System;
using System.Threading.Tasks;

namespace GN.Blazor.SharedIndexedDB.Services
{
    class MessageContext : IMessageContext
    {
        private readonly MessageBus bus;
        private readonly Message message;
        private MessageScope scope;
        public Message Message => this.message;
        public Transports Transport { get; private set; }
        public MessageContext(MessageBus bus, Message message, Transports transport=Transports.Unknown)
        {
            this.bus = bus;
            this.message = message;
            this.scope = MessageScope.AllScopes;
            this.Transport = transport;
        }

        public Task Publish(MessageScope scope = null)
        {
            return this.bus.Publish(this, scope ?? this.scope);
        }

        public Task<Message> Request(MessageScope scope = null)
        {
            return this.bus.Request(this, scope ?? this.scope);
        }
        public IMessageContext WithScope(Action<MessageScope> action)
        {
            action?.Invoke(this.scope);
            return this;
        }
    }
}

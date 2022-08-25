using System;
using System.Collections.Generic;
using System.Linq;

namespace GN.Blazor.SharedIndexedDB.Messaging
{
    public class MessageScope
    {
        internal HashSet<Transports> transports = new HashSet<Transports>();

        MessageScope()
        {
            transports = new HashSet<Transports>(Enum.GetValues(typeof(Transports)).OfType<Transports>());
        }
        public MessageScope All()
        {
            transports = new HashSet<Transports>(Enum.GetValues(typeof(Transports)).OfType<Transports>());
            return this;
        }

        public MessageScope Except(params Transports[] transport)
        {
            transports = new HashSet<Transports>(transports.Except(transport));
            return this;
        }
        public MessageScope Only(params Transports[] transports)
        {
            this.transports = new HashSet<Transports>(transports);
            return this;
        }
        public bool Contains(Transports transport) => transports.Contains(transport);
        public static MessageScope AllScopes => new MessageScope().All();


    }
}

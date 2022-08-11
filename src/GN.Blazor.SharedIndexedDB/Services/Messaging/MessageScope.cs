using System;
using System.Collections.Generic;
using System.Linq;

namespace GN.Blazor.SharedIndexedDB.Services
{
    public class MessageScope
    {
        internal HashSet<Transports> transports = new HashSet<Transports>();

        MessageScope()
        {
            this.transports = new HashSet<Transports>(Enum.GetValues(typeof(Transports)).OfType<Transports>());
        }
        public MessageScope All()
        {
            this.transports = new HashSet<Transports>(Enum.GetValues(typeof(Transports)).OfType<Transports>());
            return this;
        }

        public MessageScope Except(params Transports[] transport)
        {
            this.transports = new HashSet<Transports>(this.transports.Except(transport));
            return this;
        }
        public MessageScope Only(params Transports[] transports)
        {
            this.transports = new HashSet<Transports>(transports);
            return this;
        }
        public bool Contains(Transports transport) => this.transports.Contains(transport);
        public static MessageScope AllScopes => new MessageScope().All();
        

    }
}

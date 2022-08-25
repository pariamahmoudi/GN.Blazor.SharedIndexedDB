using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GN.Blazor.SharedIndexedDB.Messaging
{
    class BusSubscription
    {
        public string SubjetPattern { get; set; }
        Action<IMessageContext> Handler { get; set; }
        public BusSubscription(string pattern, Action<IMessageContext> handler)
        {
            SubjetPattern = pattern;
            Handler = handler;
        }
        public void Handle(IMessageContext context)
        {
            Handler?.Invoke(context);
        }
        public bool Matches(IMessageContext context)
        {
            return ShilaFeaturesExtensions.PatternMatchesSubject(SubjetPattern, context.Message.Subject);
        }
    }
}

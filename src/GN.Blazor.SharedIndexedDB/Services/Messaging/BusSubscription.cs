using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GN.Blazor.SharedIndexedDB.Services.Internals
{
    class BusSubscription
    {
        public string SubjetPattern { get; set; }
        Action<IMessageContext> Handler { get; set; }
        public BusSubscription(string pattern, Action<IMessageContext> handler)
        {
            this.SubjetPattern = pattern;
            this.Handler = handler;
        }
        public void Handle(IMessageContext context)
        {
            this.Handler?.Invoke(context);
        }
        public bool Matches(IMessageContext context)
        {
            return ShilaFeaturesExtensions.PatternMatchesSubject(this.SubjetPattern, context.Message.Subject);
        }
    }
}

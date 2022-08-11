using System;
using System.Text;
using System.Threading.Tasks;

namespace GN.Blazor.SharedIndexedDB.Services
{
    public interface IMessageBus
    {
        IMessageContext CreateContext(string subject, object message);
        IMessageContext CreateContext(Message message);
        Task Subscribe(string subject, Action<IMessageContext> handler, bool external=false);
        
    }
}

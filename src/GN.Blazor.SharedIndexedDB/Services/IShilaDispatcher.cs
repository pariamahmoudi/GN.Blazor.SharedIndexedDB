using Fluxor;
using MediatR;
using GN.Blazor.SharedIndexedDB.SharedWorker.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GN.Blazor.SharedIndexedDB.Services
{
    public interface IShilaDispatcher
    {
        public void Dispatch(object action);
        public Task Dispatch1();
    }
    class ShilaDispatcher : IShilaDispatcher
    {
        private readonly IDispatcher dispatcher;
        private readonly IMediator mediator;
        private readonly ISharedWorkerAdapter sharedWorker;

        public ShilaDispatcher(IDispatcher dispatcher, IMediator mediator, ISharedWorkerAdapter sharedWorker)
        {
            this.dispatcher = dispatcher;
            this.mediator = mediator;
            this.sharedWorker = sharedWorker;
        }
        
        public void Dispatch(object action)
        {
            this.dispatcher.Dispatch(action);
            //mediator.Send(new StartServiceWorkerAction { });
            //throw new NotImplementedException();
        }

        public async Task Dispatch1()
        {
            var result = await this.sharedWorker.Ping("hi");
        }
    }
}

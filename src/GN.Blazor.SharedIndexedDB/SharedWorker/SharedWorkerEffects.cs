using Fluxor;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using GN.Blazor.SharedIndexedDB.SharedWorker.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GN.Blazor.SharedIndexedDB.SharedWorker
{
    public class SharedWorkerEffects : IRequestHandler<StartServiceWorkerAction>
    {
        private readonly IServiceProvider serviceProvider;
        private readonly ILogger<SharedWorkerEffects> logger;

        public SharedWorkerEffects(IServiceProvider serviceProvider, ILogger<SharedWorkerEffects> logger)
        {
            this.serviceProvider = serviceProvider;
            this.logger = logger;
        }

        public Task<Unit> Handle(StartServiceWorkerAction request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        [EffectMethod]
        public async Task StartSharedWorker(StartServiceWorkerAction action, IDispatcher dispatcher)
        {

        }
    }
}

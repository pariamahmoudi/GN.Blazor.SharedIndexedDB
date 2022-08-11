using Fluxor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GN.Blazor.SharedIndexedDB.SharedWorker
{
    public class SharedWorkerFeature : Feature<SharedWorkerState>
    {
        public override string GetName()
        {
            return "SharedWorker";
        }

        protected override SharedWorkerState GetInitialState()
        {
            return new SharedWorkerState { };
        }
    }
}

using Microsoft.AspNetCore.Components;
using GN.Blazor.SharedIndexedDB.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GN.Blazor.SharedIndexedDB.Tests.Pages
{
    public class TestFixture : ComponentBase
    {
        [Inject] 
        protected IMessageBus Bus { get; set; }
        public StringBuilder _log = new StringBuilder();
        public void ClearLog()
        {
            this._log = new StringBuilder();
            this.StateHasChanged();

        }
        public void Log(string fmt)
        {
            //_log.AppendFormat(fmt, args);
            _log.AppendLine(fmt);
            this.StateHasChanged();
        }
        public void Assert(bool check, string comment)
        {
            if (!check)
                throw new Exception(comment);
        }

    }
}

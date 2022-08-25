using System;
using System.Text;
using GN.Blazor.SharedIndexedDB.Messaging;
using Microsoft.AspNetCore.Components;

namespace GN.Blazor.SharedIndexedDB.Tests.Pages
{

    public class TestFixture : ComponentBase
    {
        [Inject]
        protected IMessageBus Bus { get; set; }

        public StringBuilder log = new StringBuilder();

        public void ClearLog()
        {
            this.log = new StringBuilder();
            this.StateHasChanged();

        }
        public void Log(string fmt)
        {
            //_log.AppendFormat(fmt, args);
            log.AppendLine(fmt);
            this.StateHasChanged();
        }
        public void Assert(bool check, string comment)
        {
            if (!check)
                throw new Exception(comment);
        }

    }
}

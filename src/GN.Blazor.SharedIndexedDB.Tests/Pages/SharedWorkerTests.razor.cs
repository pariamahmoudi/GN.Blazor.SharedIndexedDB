using Microsoft.AspNetCore.Components;
using GN.Blazor.SharedIndexedDB;
using GN.Blazor.SharedIndexedDB.Models;
using GN.Blazor.SharedIndexedDB.Models.Messages;
using GN.Blazor.SharedIndexedDB.Services;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GN.Blazor.SharedIndexedDB.Tests.Pages
{
    public partial class SharedWorkerTests
    {
        [Inject]
        public IShilaDispatcher dispatcher { get; set; }
        [Inject]
        public ISharedWorkerAdapter adapter { get; set; }
        private StringBuilder _log = new StringBuilder();
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
        public async Task LoadServiceWorker()
        {
            await Task.CompletedTask;
            //await dispatcher.Dispatch1();
            await adapter.StartServiceWorker();
            await adapter.PostMessage(new Message<string> {Payload="hi",  Subject="hi there" });
            

        }
        public async Task CreateDatabase()
        {
            await adapter.StartServiceWorker();
            try
            {
                var res = await adapter.Request(new CreateDatabaseMessage("testdb"));
                Log($"Database successfully created. {res.GetPayload<string>()}");
            }
            catch(Exception err)
            {
                
                Log( err==null?"Unkown error occured.": $"An error occured {err?.Message}");
            }
            


        }
        public async Task GetStatus()
        {
            var response = 
                await adapter.Request(new Message("sharedworker_get_status", ""));
            Log($"SharedWorker Status:{response}");
        }
        public async Task Play()
        {
            await adapter.StartServiceWorker();
            try
            {
               await adapter.PostMessage(new Message<string> { 
                   Subject="play",

                });
            }
            catch (Exception err)
            {

                Log(err == null ? "Unkown error occured." : $"An error occured {err?.Message}");
            }



        }


    }
}

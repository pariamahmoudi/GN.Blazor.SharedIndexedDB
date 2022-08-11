using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GN.Blazor.SharedIndexedDB.SharedWorker;
using GN.Blazor.SharedIndexedDB.Models;
using Microsoft.Extensions.DependencyInjection;
namespace GN.Blazor.SharedIndexedDB.Services
{
    public delegate void OnMessage(Message message);
    public interface ISharedWorkerAdapterFactory
    {
        ISharedWorkerAdapter CreateAdapter(string name, Action<ShareWorkerAdapterOptions> configure = null);
    }
    
    class SharedWorkerAdapterFactory : ISharedWorkerAdapterFactory
    {
        private readonly IServiceProvider serviceProvider;

        public SharedWorkerAdapterFactory(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }
        public ISharedWorkerAdapter CreateAdapter(string name, Action<ShareWorkerAdapterOptions> configure = null)
        {
            return new SharedWorkerAdapter(
                this.serviceProvider.GetService<IJSRuntime>(),
                this.serviceProvider.GetService<ILogger<SharedWorkerAdapter>>(),
                name, configure);
        }
    }
    public interface ISharedWorkerAdapter
    {
        Task<string> Ping(string text);
        Task<bool> StartServiceWorker();
        Task PostMessage<T>(Message<T> message);
        Task PostMessage(Message message, bool forced = false);
        Task<Message> Request(Message message);
        Task Subscribe(string subject, Action<Message> handler);
        public string Name { get; }
        public event OnMessage OnMessage;
    }
    class SharedWorkerAdapter : ISharedWorkerAdapter
    {

        private readonly IJSRuntime runtime;
        private readonly ILogger<SharedWorkerAdapter> logger;
        private IJSObjectReference _module;
        private IJSObjectReference _adapter;
        private DotNetObjectReference<SharedWorkerAdapter> _myreference;
        private SharedWorkerStatusPayload _status;
        public string Name { get; private set; }
        private bool _started;
        private Dictionary<string, TaskCompletionSource<Message>> tasks = new Dictionary<string, TaskCompletionSource<Message>>();
        
        public ShareWorkerAdapterOptions Options { get; private set; }

        public SharedWorkerAdapter(IJSRuntime runtime, ILogger<SharedWorkerAdapter> logger, string name = null, Action<ShareWorkerAdapterOptions> configure = null)
        {
            this.runtime = runtime;
            this.logger = logger;
            this.Name = !string.IsNullOrWhiteSpace(name) ? name : $"ShareWorker {new Random().Next(1, 100000)}";
            this.Options = new ShareWorkerAdapterOptions
            {
                SharedWorkerUrl = $"/_content/{this.GetType().Assembly.GetName().Name}/SharedWorker/SharedWorker.js",
                OnMessageMethodName = nameof(OnMessageReceived),
                CallbackAdapter = DotNetObjectReference.Create(this)
            };
            configure?.Invoke(this.Options);

            //this.SharedWorkerUrl = !string.IsNullOrWhiteSpace(url) ? url : $"/_content/{this.GetType().Assembly.GetName().Name}/SharedWorker/SharedWorker.js";
        }
        public event OnMessage OnMessage;
        public string SharedWorkerUrl { get; private set; }
        //$"/_content/{this.GetType().Assembly.GetName().Name}/SharedWorker/SharedWorker.js";
        public async Task<IJSObjectReference> GetAdapter(bool refersh = false, bool dont_start = false)
        {
            if (this._adapter == null || refersh)
            {
                await this._adapter.SafeDisposeAsync();
                await this._module.SafeDisposeAsync();
                this._myreference = DotNetObjectReference.Create(this);
                //var options = new ShareWorkerAdapterOptions
                //{
                //    CallbackAdapter = this._myreference,
                //    SharedWorkerUrl = SharedWorkerUrl,
                //    OnMessageMethodName = nameof(OnMessageReceived)
                //};


                var js = $"/_content/{this.GetType().Assembly.GetName().Name}/SharedWorker/SharedWorkerAdapter.js";
                this._module = await runtime.InvokeAsync<IJSObjectReference>("import", js);
                this._adapter = await this._module.InvokeAsync<IJSObjectReference>("createAdapter",
                    "", this.Options);
                await this._adapter
                   .InvokeVoidAsync("startSharedWorker", SharedWorkerUrl, this.Name, this._myreference);
                this._started = true;
                //await this.StartServiceWorker();
            }
            return this._adapter;
        }

        public async Task<string> Ping(string ping)
        {
            return await (await this.GetAdapter())
                .InvokeAsync<string>("ping", ping);
        }

        [JSInvokable]
        public async Task OnMessageReceived(Message message)
        {
            await Task.CompletedTask;
            message.From = string.IsNullOrWhiteSpace(message.From) ? this.Name : message.From;
            if (message.Subject == "sharedworker_status")
            {
                this._status = message.GetPayload<SharedWorkerStatusPayload>();
                this._status.Subscriptions = this._status.Subscriptions ?? new SubscriptionData[] { };
            }
            this.OnMessage?.Invoke(message);
            //this.subscriptions.ForEach(x => x(message));
            if (message.IsReply() && this.tasks.TryGetValue(message.ReplyTo, out var _source))
            {

                if (message.IsError())
                {
                    _source.SetException(new Exception(message.GetPayload<string>()));
                    this.tasks.Remove(message.ReplyTo);

                }
                else
                {
                    _source.SetResult(message);
                    this.tasks.Remove(message.ReplyTo);
                }
            }
        }

        public async Task<bool> StartServiceWorker()
        {
            if (!this._started)
            {
                await this.GetAdapter();

            }
            return this._started;
        }
        public async Task PostMessage<T>(Message<T> message)
        {
            await (await this.GetAdapter())
               .InvokeVoidAsync("postMessage", message);

        }

        public Task<Message> Request(Message message)
        {
            var source = new TaskCompletionSource<Message>();
            this.tasks[message.Id] = source;
            return this.PostMessage(message).ContinueWith(x => source.Task).Unwrap();

        }


        public async Task PostMessage(Message message, bool forced = false)
        {
            var subscriptionsOrEmpty = this._status?.Subscriptions ?? new SubscriptionData[] { };
            var should_send = forced ||
                subscriptionsOrEmpty.Length == 0 || subscriptionsOrEmpty.Any(x => ShilaFeaturesExtensions.PatternMatchesSubject(x.Subject, message.Subject));
            if (should_send)
            {
                await (await this.GetAdapter())
                   .InvokeVoidAsync("postMessage", message);
            }
        }

        public async Task Subscribe(string subject, Action<Message> handler)
        {

            await this.PostMessage(new Message<string[]> { Subject = "subscribe", Payload = new string[] { subject } }, true);
        }
    }
}

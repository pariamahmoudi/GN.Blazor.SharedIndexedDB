using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using GN.Blazor.SharedIndexedDB.Messaging;

namespace GN.Blazor.SharedIndexedDB.SharedWorker
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
                serviceProvider.GetService<IJSRuntime>(),
                serviceProvider.GetService<ILogger<SharedWorkerAdapter>>(),
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
            Name = !string.IsNullOrWhiteSpace(name) ? name : $"ShareWorker {new Random().Next(1, 100000)}";
            Options = new ShareWorkerAdapterOptions
            {
                SharedWorkerUrl = $"/_content/{GetType().Assembly.GetName().Name}/SharedWorker/SharedWorker.js",
                OnMessageMethodName = nameof(OnMessageReceived),
                CallbackAdapter = DotNetObjectReference.Create(this)
            };
            configure?.Invoke(Options);

            //this.SharedWorkerUrl = !string.IsNullOrWhiteSpace(url) ? url : $"/_content/{this.GetType().Assembly.GetName().Name}/SharedWorker/SharedWorker.js";
        }
        public event OnMessage OnMessage;
        public string SharedWorkerUrl { get; private set; }
        //$"/_content/{this.GetType().Assembly.GetName().Name}/SharedWorker/SharedWorker.js";
        public async Task<IJSObjectReference> GetAdapter(bool refersh = false, bool dont_start = false)
        {
            if (_adapter == null || refersh)
            {
                await _adapter.SafeDisposeAsync();
                await _module.SafeDisposeAsync();
                _myreference = DotNetObjectReference.Create(this);
                //var options = new ShareWorkerAdapterOptions
                //{
                //    CallbackAdapter = this._myreference,
                //    SharedWorkerUrl = SharedWorkerUrl,
                //    OnMessageMethodName = nameof(OnMessageReceived)
                //};


                var js = $"/_content/{GetType().Assembly.GetName().Name}/SharedWorker/SharedWorkerAdapter.js";
                _module = await runtime.InvokeAsync<IJSObjectReference>("import", js);
                _adapter = await _module.InvokeAsync<IJSObjectReference>("createAdapter",
                    "", Options);
                await _adapter
                   .InvokeVoidAsync("startSharedWorker", SharedWorkerUrl, Name, _myreference);
                _started = true;
                //await this.StartServiceWorker();
            }
            return _adapter;
        }

        public async Task<string> Ping(string ping)
        {
            return await (await GetAdapter())
                .InvokeAsync<string>("ping", ping);
        }

        [JSInvokable]
        public async Task OnMessageReceived(Message message)
        {
            await Task.CompletedTask;
            message.From = string.IsNullOrWhiteSpace(message.From) ? Name : message.From;
            if (message.Subject == "sharedworker_status")
            {
                _status = message.GetPayload<SharedWorkerStatusPayload>();
                _status.Subscriptions = _status.Subscriptions ?? new SubscriptionData[] { };
            }
            OnMessage?.Invoke(message);
            //this.subscriptions.ForEach(x => x(message));
            if (message.IsReply() && tasks.TryGetValue(message.ReplyTo, out var _source))
            {

                if (message.IsError())
                {
                    _source.SetException(new Exception(message.GetPayload<string>()));
                    tasks.Remove(message.ReplyTo);

                }
                else
                {
                    _source.SetResult(message);
                    tasks.Remove(message.ReplyTo);
                }
            }
        }

        public async Task<bool> StartServiceWorker()
        {
            if (!_started)
            {
                await GetAdapter();

            }
            return _started;
        }
        public async Task PostMessage<T>(Message<T> message)
        {
            await (await GetAdapter())
               .InvokeVoidAsync("postMessage", message);

        }

        public Task<Message> Request(Message message)
        {
            var source = new TaskCompletionSource<Message>();
            tasks[message.Id] = source;
            return PostMessage(message).ContinueWith(x => source.Task).Unwrap();

        }


        public async Task PostMessage(Message message, bool forced = false)
        {
            var subscriptionsOrEmpty = _status?.Subscriptions ?? new SubscriptionData[] { };
            var should_send = forced ||
                subscriptionsOrEmpty.Length == 0 || subscriptionsOrEmpty.Any(x => ShilaFeaturesExtensions.PatternMatchesSubject(x.Subject, message.Subject));
            if (should_send)
            {
                await (await GetAdapter())
                   .InvokeVoidAsync("postMessage", message);
            }
        }

        public async Task Subscribe(string subject, Action<Message> handler)
        {

            await PostMessage(new Message<string[]> { Subject = "subscribe", Payload = new string[] { subject } }, true);
        }
    }
}

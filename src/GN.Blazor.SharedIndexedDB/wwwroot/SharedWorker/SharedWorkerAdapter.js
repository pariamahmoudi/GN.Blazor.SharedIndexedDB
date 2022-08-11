class SharedWorkerAdapter {
    name;
    options;
    worker = null;
    constructor(name, options) {
        this.name = name;
        this.options = options;
        this.options = options || {};
        this.name = name || (Math.random() * 10000).toString();
        this.options.onMessageMethodName = this.options.onMessageMethodName || "OnMessageReceived";
    }
    async ping(ping) {
        return "pong " + ping;
    }
    async startSharedWorker(url, name, callBack) {
        this.name = name || this.name;
        this.options.sharedWorkerUrl = url || this.options.sharedWorkerUrl;
        this.options.callbackAdapter = callBack || this.options.callbackAdapter;
        if (!this.worker) {
            this.worker = new SharedWorker(this.options.sharedWorkerUrl, { name: this.name });
            this.worker.port.onmessage = this.handlePortMessage.bind(this);
            this.worker.port.postMessage({ subject: 'sharedworker_init', payload: this.options });
        }
        //this.worker.port.postMessage({ subject: 'sharedworker_init', payload: this.options });
    }
    handlePortMessage(ev) {
        this.options.callbackAdapter?.invokeMethodAsync(this.options.onMessageMethodName, ev.data);
    }
    postMessage(m) {
        if (!this.worker) {
        }
        this.worker?.port.postMessage(m);
    }
}
export function createAdapter(name, options) {
    return new SharedWorkerAdapter(name, options);
}

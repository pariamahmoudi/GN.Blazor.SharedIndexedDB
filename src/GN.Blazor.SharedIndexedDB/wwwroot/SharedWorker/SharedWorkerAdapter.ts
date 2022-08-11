interface IData {
    payload: string
}
interface IOptions {
    callbackAdapter: any | null;
    sharedWorkerUrl: string;
    onMessageMethodName: string;
}
interface ICallBack {
    OnMessage(ev: string): Promise<void>;
}

class SharedWorkerAdapter {
    private worker: SharedWorker | null = null;
    constructor(public name: string, public options: IOptions) {
        this.options = options || {};
        this.name = name || (Math.random() * 10000).toString();
        this.options.onMessageMethodName = this.options.onMessageMethodName || "OnMessageReceived";

    }

    public async ping(ping: string) {
        return "pong " + ping;
    }
    public async startSharedWorker(url: string, name: string, callBack: ICallBack) {
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
    handlePortMessage(ev: MessageEvent<any>) {

        (this.options.callbackAdapter as any)?.invokeMethodAsync(this.options.onMessageMethodName, ev.data);
    }
    postMessage(m: any) {
        if (!this.worker) {

        }
        this.worker?.port.postMessage(m);
    }


}

export function createAdapter(name: string, options: IOptions) {


    return new SharedWorkerAdapter(name, options);
}
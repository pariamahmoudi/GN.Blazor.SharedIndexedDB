const ports: MessagePort[] = [];
interface IShareWorkerOptions {

}
var options: IShareWorkerOptions = {};
// #region m

// #endregion

const subjects = {
    "delete_by_expression": "delete_by_expression",
    "get_by_id": "get_by_id",
    "delete_by_id": "delete_by_id",
    'create_database': 'create_database',
    'create_store': 'create_store',
    'put': 'put',
    'get_schema': 'get_schema',
    'delete_database': 'delete_database',
    'database_exists': 'database_exists',
    'count': 'count',
    'fetch': 'fetch',
    'sharedworker_init': 'sharedworker_init',
    'sharedworker_status': 'sharedworker_status',
    'sharedworker_get_status': 'sharedworker_get_status',
    'subscribe': 'subscribe',
    'play': 'play',
    'get': 'get',
    'reply': 'reply',
    'error': 'error',
    'ping': 'ping',
    'unknown': 'unknown',
    'ack': 'ack'
}

//#region Bus

interface IMessageHeader {
    [key: string]: string
}
interface ISubscriptionData {
    subject: string,
}
interface IShareWorkerStatus {
    subscriptions?: ISubscriptionData[];
    options?: IShareWorkerOptions;
}

interface IMessage {
    headers?: IMessageHeader
    id: string;
    subject: string;
    payload?: unknown;
    from?: string;
    replyTo?: string;
    statusCode?: number;
}

interface IMessageHandler {
    (context: MessageContext): void;
}
interface IRequestPromise {
    resolve: (a: any) => void;
    reject: (a: any) => void;
}







class Logger {
    sendLog(level: string, ...data: any[]) {
        switch (level) {
            case 'info':
                console.info(...data);
                break;
            case 'warn':
                console.warn(...data);
                break;
            case 'error':
                console.error(...data);
                break;
            default:
                console.log(...data);
                break;
        }
    }
    info(...data: any[]) {
        this.sendLog('info', ...data);
        //console.info(arguments);
    }
    error(...data: any[]) {
        this.sendLog('error', ...data);
    }
    warn(...data: any[]) {
        this.sendLog('warn', ...data);
    }
    log(...data: any[]) {
        this.sendLog('log', ...data);
    }





}
const logger = new Logger();

class Utils {
    toMessage(m: any): Message {
        return new Message(m?.subject, m?.payload, m?.id, m?.replyTo)
    }
    newId(): string {
        return (Math.random() * 100000).toFixed(0);
    }
    wildCardMatch(str: string, rule: string) {
        if (!rule) {
            return false
        }
        const escapeRegex = (str: string) =>
            //eslint-disable-next-line no-useless-escape
            str.replace(/([.*+?^=!:${}()|\[\]\/\\])/g, '\\$1');
        return new RegExp(
            '^' + rule.split('*').map(escapeRegex).join('.*') + '$'
        ).test(str);
    }

}
const utils = new Utils();



class Message implements IMessage {
    public id: string;
    public headers: { [key: string]: string } = {}

    constructor(public subject: string, public payload: unknown,
        private _id?: string, public replyTo?: string,
        public statusCode?: number, public from?: string, headers?: IMessageHeader) {
        this.subject = subject || 'unknown';
        this.id = _id || utils.newId();
        this.headers = headers || {};
    }
    getPayload<T>() {
        return this.payload as any as T;
    }
    reply(result: unknown, subject: string = 'reply'): Message {
        return new Message(subject, result, utils.newId(), this.id, 0);
    }
    error(err: unknown, subject: string = 'error', statusCode?: number): Message {
        return new Message(subject, err, utils.newId(), this.id, statusCode || -1);
    }
    isReply(): boolean {
        return this.replyTo != null && this.replyTo != undefined && (this.replyTo.length > 0);
    }
    isError(): boolean {
        return this.isReply() && (this.statusCode != undefined && this.statusCode != 0);
        return this.subject == 'error';
    }
    toString() {
        return `Subject:'${this.subject}', payload:'${JSON.stringify(this.payload)}'`;
    }

}

class MessageContext {
    constructor(public message: Message, public bus: Bus) {
    }
    postMessage(msg: IMessage) {
        this.bus.port?.postMessage(msg);
    }

    reply(payload: unknown, subject: string = 'reply') {
        bus.postMessage(this.message.reply(payload, subject));
    }
    error(payload: Error | any, subject: string = 'error') {
        if (payload && payload.message) {
            payload = payload.message;
        }
        bus.postMessage(this.message.error(payload, subject));
    }
    publish() {
        bus.publish(this.message);
    }
}

type Scope = 'auto' | 'internal' | 'external' | 'forced-external';
class BusSubscriprion {
    constructor(public handler: IMessageHandler, public subject: string, public isExtrenal = false) {

    }
    matches(subject: string) {
        return this.subject == subject || this.subject == "*";
    }


}
class Bus {
    private requests: Map<string, IRequestPromise> = new Map<string, IRequestPromise>();
    private subscriptions: BusSubscriprion[] = [];
    public port: MessagePort | null = null;
    constructor() {

    }
    getSubscriptions(): ISubscriptionData[] {
        var result: ISubscriptionData[] = [];
        this.subscriptions
            .filter(x => !x.isExtrenal)
            .forEach(x => {
                if (result.findIndex(y => y.subject == x.subject) < 0) {
                    result.push({ subject: x.subject })
                }
            });
        return result;

    }
    subscribe(subject: string, handler: IMessageHandler, isExtrenal = false) {
        this.subscriptions.push(new BusSubscriprion(handler, subject, isExtrenal));
        if (this.port) {
            const status: IShareWorkerStatus = {};
            status.subscriptions = this.getSubscriptions();
            this.postMessage(new Message(subjects.sharedworker_status, status));
        }
    }

    postMessage(message: Message) {
        message.from = message.from || self.name;
        this.port?.postMessage(message);
    }
    createMessage(subject: string, payload: unknown): MessageContext {

        return new MessageContext(new Message(subject, payload), this);
    }
    publish(message: Message, scope: Scope = 'auto') {
        if (message.isReply() && message.replyTo != null) {
            var task = this.requests.get(message.replyTo);
            if (task) {
                if (message.isError()) {
                    task.reject(message);
                } else {
                    task.resolve(message);
                }
                this.requests.delete(message.replyTo);
            }
            return;
        }
        var context = new MessageContext(message, this);
        if (scope == 'auto' || scope == 'internal') {
            this.subscriptions
                .filter(x => x.matches(message.subject))
                .filter(x => !x.isExtrenal)
                .forEach(x => x.handler(context));
        }
        if (scope == 'forced-external' ||
            (scope == 'auto' && (this.subscriptions.findIndex(x => x.matches(message.subject) && x.isExtrenal) > -1))) {
            message.from = message.from || self.name;
            this.port?.postMessage(message);
        }

    }
    request(message: Message): Promise<Message> {
        return new Promise<Message>((resolve, reject) => {
            this.requests.set(message.id, { reject: reject, resolve: resolve });
            this.postMessage(message);
        })
    }
}
var bus = new Bus();

bus.subscribe('sharedworker_init', msg => {
    self.options = msg.message.getPayload<IShareWorkerOptions>();
    var status: IShareWorkerStatus = {};
    status.subscriptions = bus.getSubscriptions();
    status.options = self.options;
    logger.info("SharedWorker Initialized. Options:", self.options);
    bus.postMessage(new Message('sharedworker_status', status));
});
bus.subscribe('sharedworker_get_status', msg => {

    var status: IShareWorkerStatus = {};
    status.subscriptions = bus.getSubscriptions();
    status.options = self.options;
    msg.reply(status);

});
bus.subscribe('subscribe', msg => {
    var subjects = msg.message.getPayload<string[]>() || [];
    subjects.forEach(x => bus.subscribe(x, x => { }, true));
});
//#endregion


//#region IndexdDb
interface IndexData {
    keyPath: string;
    name: string;
    unique: boolean;
}
interface ISchema {
    // dbName: string;
    storeName: string;
    primaryKey: IndexData;
    indexes: IndexData[];
}

interface IDatabaseSchema {
    dbName: string,
    stores: ISchema[];
}
interface IDeleteRecordWithFilter {
    filter: IFilter,
    schema: ISchema,
    dbName: string
}
interface getRecordByIDPayload {
    id: string
    dbName: string
    schema: ISchema;
}
interface IFilter {
    operator: string;
    left: IFilter;
    right: IFilter;
    value: any
}
interface fetchPayload {
    dbName: string;
    schema: ISchema;
    skip?: number;
    take?: number;
    filter: IFilter;
    orderBy: string;
    descending?: boolean;
    select: string;
}
interface fetchResultPayload {
    items: unknown[];
}
interface createDatabasePayload {
    databaseName: string;
}
interface createStore {
    dbName: string;
    schema: ISchema;
}
interface putPayload {
    schema: ISchema;
    dbName: string;
    items: unknown[]
}
interface getSchemaPayload {
    dbName: string;
}
interface countPayload {
    schema: ISchema;
    dbName: string;
}
interface deleteRecordByIDPayload {
    id: string
    dbName: string
    schema: ISchema;
}
interface deleteRecordByIDResult {
    success: boolean

}

function replay(ctx: any, next: (ctx: any) => Promise<unknown> | null) {

    return new Promise<void>((resolve, reject) => {
        var p = next(ctx);
        if (p == null) {
            resolve();
            return;
        }
        else {
            p
                .then(() => {
                    replay(ctx, next);
                })
                .catch(e => {
                    reject(e);
                })
        }
        return;
    });
}

interface IObjectStoreHolder {
    db: IDBDatabase;
    trans: IDBTransaction;
    os: IDBObjectStore
}
class ObjectStore {
    constructor(public db: IDBDatabase, public transaction: IDBTransaction, public store: IDBObjectStore) {

    }
    public close() {
        this.db.close();
    }
}
class Filter {
    constructor(public operator: string, public left: Filter, public right: Filter, public value: any) {
    }
}
class IndexedDbAdapter {

    constructor() {
        bus.subscribe(subjects.create_database, this.createDatabaseHandler.bind(this));
        bus.subscribe(subjects.delete_database, this.deleteDatabaseHandler.bind(this));
        bus.subscribe(subjects.database_exists, this.databaseExistsHandler.bind(this));
        bus.subscribe(subjects.create_store, this.createStoreHandler.bind(this));
        bus.subscribe(subjects.put, this.putHandler.bind(this));
        bus.subscribe(subjects.count, this.countHandler.bind(this));
        bus.subscribe(subjects.fetch, this.fetchHandler.bind(this));
        bus.subscribe(subjects.get_schema, this.getDatabaseSchemaHandler.bind(this));
        bus.subscribe(subjects.get_by_id, this.getRecordByIDhandler.bind(this))
        bus.subscribe(subjects.delete_by_id, this.deleteRecordByIDHandler.bind(this))
        bus.subscribe(subjects.delete_by_expression, this.deleteRecordWithFilter.bind(this))
        bus.subscribe('play', this.play.bind(this));
    }
    get_db_name_error(dbName: string) {
        if (!dbName || dbName.length == 0 || dbName.trim().length == 0) {
            return `Invalid DatabaseName. DatabaseName is blank `;
        }
    }
    get_schema_error(schema: ISchema, for_create_store = false) {
        if (!schema)
            return "Invalid Schema. Schema is NULL";
        if (!schema.storeName || schema.storeName.length == 0)
            return "Invalid Schema. Bad store name.";
        if (for_create_store) {

        }
    }
    getObjectStore(dbName: string, storeName: string, mode: IDBTransactionMode): Promise<ObjectStore> {
        return new Promise<ObjectStore>((resolve, reject) => {
            const dbrequest = self.indexedDB.open(dbName);
            dbrequest.onerror = err => reject(err);
            dbrequest.onsuccess = ev => {
                const db = dbrequest.result;
                const trans = db.transaction([storeName], mode);
                const os = trans.objectStore(storeName);
                const result = new ObjectStore(db, trans, os)
                resolve(result);
            }
        })
    }



    async withStore<T>(dbName: string, storeName: string, mode: IDBTransactionMode, action: (os: ObjectStore) => Promise<T>) {
        const os = await this.getObjectStore(dbName, storeName, mode);
        const res = await action(os);
        os.close();
        return res;

    }
    async fetch_d(query: fetchPayload): Promise<fetchResultPayload> {
        const db = await this.getDatabase(query.dbName);
        return new Promise<fetchResultPayload>((resolve, reject) => {
            const result: fetchResultPayload = {
                items: [],
            }
            const transaction = db.transaction([query.schema.storeName], "readonly");
            const objectStore = transaction.objectStore(query.schema.storeName);
            const cursor_req = objectStore.openCursor();
            cursor_req.onerror = err => reject(err.target);
            cursor_req.onsuccess = ev => {
                const cursor = cursor_req.result;
                if (cursor) {
                    result.items.push(cursor.value);
                    //cursor.advance()
                    cursor.continue();
                }
                else {
                    // No more data
                    db.close();
                    resolve(result);
                }
            }



        })

    }

    async deleteRecordWithFilter(context: MessageContext) {

        var msg = context.message.getPayload<IDeleteRecordWithFilter>();

        return this.withStore<object>(msg.dbName, msg.schema.storeName, 'readwrite', os => {
            return new Promise<object>((resolve, reject) => {
                var resultPayload: deleteRecordByIDResult = { success: false };
                const cursor_req = os.store.openCursor();
                cursor_req.onerror = err => {

                    reject((err as object as DOMException).message);
                    context.reply(resultPayload)

                }
                cursor_req.onsuccess = ev => {
                    const cursor = cursor_req.result;
                    if (cursor) {
                        if (msg.filter && cursor.value && this.matchFilter(cursor.value, msg.filter)) {
                            os.store.delete(cursor.key);
                            resultPayload.success = true;
                            resolve(resultPayload)
                            context.reply(resultPayload)
                        }
                        cursor.continue();
                    }
                }
            })
        })


    }

    async deleteRecordByIDHandler(context: MessageContext) {
        var msg = context.message.getPayload<deleteRecordByIDPayload>();
        if (msg) {
            return this.withStore<object>(msg.dbName, msg.schema.storeName, 'readwrite', os => {
                return new Promise<object>((resolve, reject) => {
                    var res = os.store.delete(msg.id);
                    var resultPayload: deleteRecordByIDResult = { success: false };
                    res.onsuccess = ev => {
                        resultPayload.success = true
                        resolve(resultPayload);
                        context.reply(resultPayload);
                    }
                    res.onerror = ev => {

                        reject("error while deleting record" + res.error?.message.toString())
                        context.error("error while deleting record" + res.error?.message.toString())
                    }
                })
            })
        }
    }

    matchFilter(val: any, filter: IFilter): any {
        var result: any = false;
        switch (filter.operator) {
            case "EQ":
                result = this.matchFilter(val, filter.left) === this.matchFilter(val, filter.right);
                break;
            case "PROP":
                result = val[filter.value]
                break;
            case "VAL":
                result = filter.value;
                break;
            case "OR":
                result = this.matchFilter(val, filter.left) || this.matchFilter(val, filter.right);
                break;
            case "AND":
                result = this.matchFilter(val, filter.left) && this.matchFilter(val, filter.right);
                break;
            case "GT":
                result = this.matchFilter(val, filter.left) > this.matchFilter(val, filter.right);
                break;
            case "LT":
                result = this.matchFilter(val, filter.left) < this.matchFilter(val, filter.right);
                break;
            case "GTE":
                result = this.matchFilter(val, filter.left) >= this.matchFilter(val, filter.right);
                break;
            case "LTE":
                result = this.matchFilter(val, filter.left) <= this.matchFilter(val, filter.right);
                break;
            case "CONTANIS":
                {
                    const l = this.matchFilter(val, filter.left);
                    const r = this.matchFilter(val, filter.right);
                    result = l && typeof (l) === 'string' && r && typeof (r) === 'string' && l.indexOf(r) > -1;
                }
                break;
            case "LIKE":
                {
                    const l = this.matchFilter(val, filter.left);
                    var r = this.matchFilter(val, filter.right);
                    if (r && typeof (r) === 'number') {
                        r = r.toString();
                    }
                    result = l && typeof (l) === 'string' && r && typeof (r) === 'string' && utils.wildCardMatch(l, r);
                }
                break;

            default:
                break;

        }
        return result;

    }
    async getRecordByIDhandler(context: MessageContext) {
        var msg = context.message.getPayload<getRecordByIDPayload>();
        if (msg) {
            return this.withStore<object>(msg.dbName, msg.schema.storeName, 'readonly', os => {
                return new Promise<object>((resolve, reject) => {
                    var res = os.store.get(msg.id);
                    res.onsuccess = ev => {

                        resolve(res.result);
                        context.reply(res.result);
                    }
                    res.onerror = ev => {

                        reject("error while retrieving record" + res.error?.message.toString())
                        context.error("error while retrieving record" + res.error?.message.toString())
                    }
                })
            })
        }
    }
    async fetch(query: fetchPayload): Promise<fetchResultPayload> {
        //const os = await this.getObjectStore(query.dbName, query.schema.storeName, 'readonly');
        return this.withStore<fetchResultPayload>(query.dbName, query.schema.storeName, 'readonly', os => {
            return new Promise<fetchResultPayload>((resolve, reject) => {
                const result: fetchResultPayload = {
                    items: [],
                }
                //const transaction = db.transaction([query.schema.storeName], "readonly");
                const objectStore = os.store
                var index = query.skip
                var dir: IDBCursorDirection | undefined = query.descending ? "prev" : undefined;
                const cursor_req = query.orderBy
                    ? objectStore.index(query.orderBy.toLowerCase()).openCursor(undefined, dir)
                    : objectStore.openCursor(undefined, dir);
                var skipped = 0;
                cursor_req.onerror = err => reject((err as object as DOMException).message);
                cursor_req.onsuccess = ev => {
                    const cursor = cursor_req.result;
                    if (cursor) {
                        if (query.filter && !this.matchFilter(cursor.value, query.filter)) {
                            cursor.continue();
                            return;
                        }
                        if (query.skip && skipped < query.skip) {
                            skipped++;
                            cursor.continue();
                            return;
                        }
                        if (query.select && query.select == "key")
                            result.items.push(cursor.key);
                        else
                            result.items.push(cursor.value);

                        if (query.take && result.items.length >= query.take) {
                            resolve(result);
                            return;
                        }
                        //cursor.advance()
                        cursor.continue();

                    }
                    else {
                        // No more data
                        //os.close();
                        logger.log(`************* ${result.items.length} items fetched`);
                        resolve(result);
                    }
                }



            })

        })



    }
    fetchHandler(context: MessageContext) {
        var payload = context.message.getPayload<fetchPayload>();
        this.fetch(payload)
            .then(r => context.reply(r))
            .catch(err => context.error(err.message ? err.message : err));



    }
    async count(dbName: string, schema: ISchema): Promise<number> {
        var db = await this.getDatabase(dbName);
        var storeName = schema.storeName;
        return new Promise<number>((reslove, reject) => {
            const transaction = db.transaction([schema.storeName], 'readonly');
            const os = transaction.objectStore(schema.storeName);
            const req = os.count();
            req.onsuccess = () => reslove(req.result);
            req.onerror = err => reject(err.target);
        });
    }
    countHandler(ctx: MessageContext) {
        const payload = ctx.message.getPayload<countPayload>();
        if (this.get_db_name_error(payload.dbName)) {
            ctx.error(this.get_db_name_error(payload.dbName));
            return;
        }
        if (this.get_schema_error(payload.schema)) {
            ctx.error(this.get_schema_error(payload.schema));
            return;
        }
        this.count(payload.dbName, payload.schema)
            .then(r => ctx.reply(r))
            .catch(err => ctx.error(err));
    }
    play(context: MessageContext) {
        var ctx = { count: 0 };
        const NEXT = (__ctx: any) => {
            __ctx.count = __ctx.count || 0;
            __ctx.count++;
            if (__ctx.count > 5)
                return null;
            return context.bus.request(new Message('get', __ctx.count.toString()))
        }
        replay(ctx, NEXT);
    }

    getDatabase(name: string): Promise<IDBDatabase> {
        return new Promise<IDBDatabase>((resolve, reject) => {
            const dbrequest = self.indexedDB.open(name);
            dbrequest.onerror = err => reject(err);
            dbrequest.onsuccess = ev => resolve(dbrequest.result)
        });
    }
    async getDatabaseSchema(dbName: string): Promise<IDatabaseSchema> {
        const db = await this.getDatabase(dbName);
        var result: IDatabaseSchema = {
            dbName: dbName,
            stores: []
        };
        for (var i = 0; i < db.objectStoreNames.length; i++) {
            var storeName = db.objectStoreNames.item(i);
            if (storeName) {
                var trans = db.transaction(storeName, "readonly");
                var os = trans.objectStore(storeName);

                const store: ISchema = {
                    //dbName: dbName,
                    storeName: storeName,
                    indexes: [],
                    primaryKey: {
                        name: 'name',
                        unique: true,
                        keyPath: os.keyPath as string
                    }
                }
                for (var j = 0; j < os.indexNames.length; j++) {
                    const index_name = os.indexNames.item(j);
                    if (index_name) {
                        const index = os.index(index_name);
                        store.indexes.push({
                            unique: index.unique,
                            name: index.name,
                            keyPath: index.keyPath as string
                        })
                        //result += ` Index ${index.name} ${index.keyPath} ${index.unique}`;
                    }
                }
                result.stores.push(store);
            }

        }
        db.close();
        return result;
    }
    getDatabaseSchemaHandler(context: MessageContext) {
        var schema = context.message.getPayload<getSchemaPayload>();
        this.getDatabaseSchema(schema.dbName)
            .then(r => context.reply(r))
            .catch(e => context.error(e));

    }
    private _dataBaseExists(dbName: string): Promise<boolean> {
        return new Promise<boolean>((resolve, reject) => {
            var req = self.indexedDB.open(dbName);
            var existed = true;
            req.onupgradeneeded = ev => {
                existed = false;
            }
            req.onsuccess = () => {
                req.result.close();
                if (!existed)
                    self.indexedDB.deleteDatabase(dbName);
                resolve(existed);
            }
            req.onerror = err => reject(err);
        })
    }
    databaseExistsHandler(ctx: MessageContext) {
        this._dataBaseExists(ctx.message.getPayload<string>())
            .then(x => ctx.reply(x))
            .catch(err => ctx.error(err));

    }
    private _deleteDatabase(dbName: string): Promise<void> {
        return new Promise((resolve, reject) => {
            const req = self.indexedDB.deleteDatabase(dbName);
            req.onerror = err => reject(err);
            req.onsuccess = ev => {
                logger.log(`Database Successfully Deeleted: ${dbName}`);
                resolve()
            };
        });

    }
    deleteDatabaseHandler(context: MessageContext) {
        var dbName = context.message.getPayload<string>();
        this._deleteDatabase(dbName)
            .then(() => context.reply(`Database Deleted`))
            .catch(err => context.error(err));

    }
    async storeExists(schema: ISchema, db: IDBDatabase): Promise<boolean> {
        return db.objectStoreNames.contains(schema.storeName);
    }
    async _put(dbName: string, schema: ISchema, items: any[]): Promise<void> {
        const db = await this.getDatabase(dbName);
        if (!this.storeExists(schema, db)) {
            await this.createStore(dbName, schema);
        }
        const transaction = db.transaction(schema.storeName, "readwrite");
        return new Promise<void>((resolve, reject) => {
            transaction.onerror = err => { reject(err.target); }
            transaction.oncomplete = ev => { db.close(); resolve(); };
            const os = transaction.objectStore(schema.storeName);
            items.forEach(x => os.put(x));
            bus.createMessage(`put`, items.length).publish();

        })
    }

    putHandler(context: MessageContext) {
        var command = context.message.getPayload<putPayload>();
        var dbName = command.dbName;
        var schema = command.schema;
        var items = command.items;
        if (this.get_db_name_error(dbName)) {
            context.error(this.get_db_name_error(dbName))
            return;
        }
        if (this.get_schema_error(schema, false)) {
            context.error(this.get_schema_error(schema, false))
            return;
        }
        if (items == null || !Array.isArray(items)) {
            context.error("Bad Items. Items should be array.")
            return;
        }
        this._put(dbName, schema, items)
            .then(() => context.reply("Success"))
            .catch(err => context.error(err));

    }
    put(context: MessageContext) {
        var command = context.message.getPayload<putPayload>();
        var schema = command.schema;

        if (this.get_schema_error(command.schema)) {
            context.error(this.get_schema_error(command.schema));
            return;
        }
        this.getDatabase(command.dbName)
            .then(db => {
                this.storeExists(command.schema, db)
                    .then(res => {
                        if (!res) {
                            context.error(`Store not found.`)
                            return;
                        }
                        const transaction = db.transaction(schema.storeName, "readwrite");
                        transaction.onerror = err => { context.error(`Transaction Failed ${err}`) }
                        transaction.oncomplete = ev => { context.reply(`done`) };
                        const os = transaction.objectStore(schema.storeName);
                        command.items.forEach(x => os.put(x));
                    })
            })
            .catch(err => context.error(err))
    }
    async createDatabaseHandler(context: MessageContext) {
        //context.ack("database create accepted");
        var dbName = context.message.getPayload<createDatabasePayload>()?.databaseName;
        if (!dbName) {
            context.error(new Error("invalid database name"));
            return;
        }
        var req = self.indexedDB.open(dbName);
        req.onerror = ev => {

            context.error(new Error("Error: " + ev.target));
        }
        req.onupgradeneeded = (ev: IDBVersionChangeEvent) => {
            const db = req.result;
            //db.close();
            //const objectStore = db.createObjectStore("some_store", { keyPath: "index" });
            return db;
        }
        req.onsuccess = ev => {
            const db = req.result;
            context.reply(`Database created ${db.name}`);
            db.close();
        }
    }
    async createStore(dbName: string, schema: ISchema): Promise<boolean> {
        var db = await this.getDatabase(dbName);
        var exists = db.objectStoreNames.contains(schema.storeName);
        if (exists)
            return true;
        return new Promise((resolve, reject) => {
            /// Upgrade Version If necessary


            var version = db.version + 1;
            db.close();
            if (this.get_schema_error(schema)) {
                reject(this.get_schema_error(schema));
            }
            const request = indexedDB.open(dbName, version);
            request.onerror = e => {
                db.close();
                reject(`Open Database Failed.`);
            };

            request.onupgradeneeded = ev => {
                const db = request.result;
                const exists = db.objectStoreNames.contains(schema.storeName);
                if (exists) {
                    resolve(true)
                    logger.log(`Store ${schema.storeName} already existed. We will return this store.`)
                    db.close();
                    return;
                }
                var pimary = schema.primaryKey;
                if (!pimary || !pimary.keyPath || pimary.keyPath.length == 0) {
                    //db.close();
                    reject("Invalid PrimaryKey");
                    return;
                }
                const store = db.createObjectStore(schema.storeName, { keyPath: pimary.keyPath });
                var indxes = schema.indexes;
                if (indxes && Array.isArray(indxes)) {
                    indxes.forEach(x => {
                        if (x.keyPath && x.keyPath.length > 0) {
                            x.name = x.name || x.keyPath;
                            store.createIndex(x.name, x.keyPath, { unique: x.unique })
                        }
                    });
                }
                store.transaction.onerror = ev => {
                    //db.close();
                    logger.error(`An error occured while trying to create store. Name:${schema.storeName}. Err: ${ev.target}  `)
                    reject(ev.target);
                };
                store.transaction.oncomplete = ev => {
                    db.close();
                    logger.info(
                        `Store Successfully Created in Datadabe: ${dbName}. Schema:`, schema);
                    //context.reply('reply', `Store Successfuly Created. Name:'${schema.storeName}'`);
                    resolve(true);
                }
            }

        });
    }
    createStoreHandler(context: MessageContext): void {
        const message = context.message.getPayload<createStore>();
        var schema = message.schema;
        if (!message.dbName)
            context.error(`Error: Invalid Database Name ${message.dbName}`);
        if (!schema.storeName)
            context.error(`Error: Invalid Database Name ${schema.storeName}`);
        if (this.get_db_name_error(message.dbName)) {
            context.error(this.get_db_name_error(message.dbName));
        }
        if (this.get_schema_error(schema, true)) {
            context.error(this.get_schema_error(schema, true));
        }

        this.createStore(message.dbName, schema)
            .then(x => { context.reply(schema) })
            .catch(e => { context.error(e) });

        return;
    }
}


const db = new IndexedDbAdapter();

//#endregion 




this.addEventListener('connect', (e: any) => {
    var m = e as MessageEvent<any>;
    var port = m.ports[0];
    bus.port = port;
    ports.push(port);
    port.start();
    port.onmessage = function (e: MessageEvent<any>) {
        var msg = utils.toMessage(e.data);
        if (msg && msg.subject) {
            try {
                logger.log(`Handling Starts. Message:${msg.toString()}`);
                bus.publish(msg);
                logger.log(`Message Successfully Handled. [${msg.toString()}]`)
            }
            catch (e) {
                logger.error(
                    `An error occured while trying to handle this message. Message:[${msg.toString()}], Error: ${e}`
                )
            }

        } else {
            port.postMessage(msg.error('bad request'));
        }


    };


})

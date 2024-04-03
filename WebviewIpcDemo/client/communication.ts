const ipcQueue: Function[] = [];
let isWaitingForIpc: boolean = false;

function uid() {
    return window.crypto.getRandomValues(new Uint32Array(1))[0];
}

function generateCallback(callback: Function) {
    const id = uid();
    const prop = `_${id}`;
    Object.defineProperty(window, prop, {
        value: (result: any) => {

            Reflect.deleteProperty(window, prop);
            return callback && callback(result);
        },
        writable: false,
        configurable: true
    })
    return id;
}

function invoke(method: string, ...params: any[]): Promise<any> {
    return new Promise((resolve, reject) => {
        const responseCallbackId = generateCallback(resolve);
        const errorCallbackId = generateCallback(reject);

        const action = () => {
            //@ts-ignore
            window.chrome.webview.postMessage({
                method,
                params,
                responseCallbackId,
                errorCallbackId
            });
        };
        //@ts-ignore
        if ('postMessage' in window.chrome.webview) {
            action();
        } else {
            ipcQueue.push(action);
            if (!isWaitingForIpc) {
                waitForIpc();
                isWaitingForIpc = true;
            }
        }
    });
}


function waitForIpc(): void {
    //@ts-ignore
    if ('postMessage' in window.chrome.webview) {
        while (ipcQueue.length > 0) {
            const action = ipcQueue.shift();
            if (action) {
                action();
            }
        }
        isWaitingForIpc = false;
    } else {
        setTimeout(waitForIpc, 50);
    }
}

interface IResult {
    responseCallbackId: number;
    result: any;
}

//@ts-ignore
window.chrome.webview.addEventListener('message', function (result: any) {
    const data = result.data as IResult;
    //@ts-ignore
    const responseCallback = window['_' + data.responseCallbackId];
    if (responseCallback) {
        responseCallback(data.result);
    }

});
window['invoke'] = invoke;

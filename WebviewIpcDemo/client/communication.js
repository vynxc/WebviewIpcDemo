var ipcQueue = [];
var isWaitingForIpc = false;
function uid() {
    return window.crypto.getRandomValues(new Uint32Array(1))[0];
}
function generateCallback(callback) {
    var id = uid();
    var prop = "_".concat(id);
    Object.defineProperty(window, prop, {
        value: function (result) {
            Reflect.deleteProperty(window, prop);
            return callback && callback(result);
        },
        writable: false,
        configurable: true
    });
    return id;
}
function invoke(method) {
    var params = [];
    for (var _i = 1; _i < arguments.length; _i++) {
        params[_i - 1] = arguments[_i];
    }
    return new Promise(function (resolve, reject) {
        var responseCallbackId = generateCallback(resolve);
        var errorCallbackId = generateCallback(reject);
        var action = function () {
            //@ts-ignore
            window.chrome.webview.postMessage({
                method: method,
                params: params,
                responseCallbackId: responseCallbackId,
                errorCallbackId: errorCallbackId
            });
        };
        //@ts-ignore
        if ('postMessage' in window.chrome.webview) {
            action();
        }
        else {
            ipcQueue.push(action);
            if (!isWaitingForIpc) {
                waitForIpc();
                isWaitingForIpc = true;
            }
        }
    });
}
function waitForIpc() {
    //@ts-ignore
    if ('postMessage' in window.chrome.webview) {
        while (ipcQueue.length > 0) {
            var action = ipcQueue.shift();
            if (action) {
                action();
            }
        }
        isWaitingForIpc = false;
    }
    else {
        setTimeout(waitForIpc, 50);
    }
}
//@ts-ignore
window.chrome.webview.addEventListener('message', function (result) {
    var data = result.data;
    //@ts-ignore
    var responseCallback = window['_' + data.responseCallbackId];
    if (responseCallback) {
        responseCallback(data.result);
    }
});
window['invoke'] = invoke;

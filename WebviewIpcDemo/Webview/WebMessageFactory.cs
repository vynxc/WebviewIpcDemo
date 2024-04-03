using Newtonsoft.Json;

namespace WebviewIpcDemo.Webview;

public static class WebMessageFactory
{
    public static string CreateResponseMessage(long callbackId, object result)
    {
        return JsonConvert.SerializeObject(new
        {
            responseCallbackId = callbackId,
            result
        });
    }
}
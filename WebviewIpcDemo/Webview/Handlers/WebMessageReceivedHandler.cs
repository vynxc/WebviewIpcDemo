using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
using Newtonsoft.Json;
using WebviewIpcDemo.Webview.Communication;

namespace WebviewIpcDemo.Webview.Handlers;

public class WebMessageReceivedHandler(WebView2 webView, List<Delegate> actions)
{
    public readonly InvokeHandler InvokeHandler = new(webView, actions);

    public void OnReceived(CoreWebView2WebMessageReceivedEventArgs e)
    {
        var incomingMessage = JsonConvert.DeserializeObject<IncomingMessage>(e.WebMessageAsJson);

        Console.WriteLine($"Incoming message: {e.WebMessageAsJson}");
        if (incomingMessage is null)
        {
            Console.WriteLine("Failed to deserialize incoming message");
            return;
        }

        InvokeHandler.HandleInvoke(incomingMessage);
    }
}
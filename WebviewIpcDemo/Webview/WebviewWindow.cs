using System.Reflection;
using Microsoft.Web.WebView2.WinForms;
using WebviewIpcDemo.Webview.Handlers;
using WebviewIpcDemo.Webview.Helpers;

namespace WebviewIpcDemo.Webview;

public class WebviewWindow : Form
{
    public WebviewWindow(Uri source)
    {
        Console.WriteLine("Initializing WebviewForm");
        var webView = new WebView2
        {
            Dock = DockStyle.Fill,
            Source = source
        };
        var webMessageHandler = new WebMessageReceivedHandler(webView, [GetMessage]);
        var scriptLoader = new ScriptLoader(Assembly.GetExecutingAssembly());
        var communicationScript = scriptLoader.GetScript("communication.js");
        Controls.Add(webView);
        webView.CoreWebView2InitializationCompleted += async (_, e) =>
            await new CoreWebView2InitializationHandler(webView, communicationScript).OnReceived(e);
        webView.WebMessageReceived += (_, e) =>
            webMessageHandler.OnReceived(e);
    }

    private class ReturnMessage
    {
        public string Message { get; set; }
        public string Id { get; set; }
        public DateTime Timestamp { get; set; }
    }

    private ReturnMessage GetMessage()
    {
        return new ReturnMessage()
        {
            Message = "Hello from C#",
            Id = "1",
            Timestamp = DateTime.Now
        };
    }
}
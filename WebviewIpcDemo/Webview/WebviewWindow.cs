using System.Reflection;
using Microsoft.Web.WebView2.WinForms;
using WebviewIpcDemo.Webview.Handlers;
using WebviewIpcDemo.Webview.Helpers;

namespace WebviewIpcDemo.Webview;

public class WebviewWindow : Form
{
    public WebviewWindow(Uri source)
    {
        var webView = new WebView2
        {
            Dock = DockStyle.Fill,
            Source = source
        };
        var webMessageHandler = new WebMessageReceivedHandler(webView, [GetMessage]);

        var communicationScript = new ScriptLoader(Assembly.GetExecutingAssembly()).GetScript("communication.js");

        Controls.Add(webView);

        webView.CoreWebView2InitializationCompleted += async (_, e) =>
            await new CoreWebView2InitializationHandler(webView, communicationScript).OnReceived(e);

        webView.WebMessageReceived += (_, e) =>
            webMessageHandler.OnReceived(e);
    }

    private class ReturnMessage
    {
        public required string Message { get; set; }
        public required string Id { get; set; }
        public DateTime Timestamp { get; set; }
    }

    private class ReturnMessageParams
    {
        public bool Error { get; set; }
    }

    private ReturnMessage GetMessage(ReturnMessageParams returnMessageParams)
    {
        if (returnMessageParams is not null && returnMessageParams.Error)
            throw new Exception("Custom error message at GetMessage method");

        return new ReturnMessage
        {
            Message = "Hello from C#",
            Id = "1",
            Timestamp = DateTime.Now
        };
    }
}
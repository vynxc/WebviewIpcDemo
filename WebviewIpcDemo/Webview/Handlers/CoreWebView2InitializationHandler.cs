using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;

namespace WebviewIpcDemo.Webview.Handlers;

public class CoreWebView2InitializationHandler(WebView2 webView2, string communicationScript)
{
    public async Task OnReceived(CoreWebView2InitializationCompletedEventArgs e)
    {
        if (e.IsSuccess) await webView2.CoreWebView2.AddScriptToExecuteOnDocumentCreatedAsync(communicationScript);
    }
}
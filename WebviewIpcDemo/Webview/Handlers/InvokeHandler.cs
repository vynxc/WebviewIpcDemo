using Microsoft.Web.WebView2.WinForms;
using Newtonsoft.Json.Linq;
using WebviewIpcDemo.Webview.Communication;

namespace WebviewIpcDemo.Webview.Handlers;

public class InvokeHandler(WebView2 webView, List<Delegate> actions)
{
    public void HandleInvoke(IncomingMessage incomingMessage)
    {
        try
        {
            var command = actions.FirstOrDefault(c => c.Method.Name == incomingMessage.Method);

            if (command is null)
            {
                Console.WriteLine($"Command {incomingMessage.Method} not found");
                var rejectedPayload =
                    WebMessageFactory.CreateResponseMessage(incomingMessage.ErrorCallbackId, "Command not found");

                webView.CoreWebView2.PostWebMessageAsJson(rejectedPayload);
                return;
            }

            Console.WriteLine($"Executing command {incomingMessage.Method}");

            var paramsPayload = incomingMessage.Payload;
            dynamic? result;
            if (paramsPayload is not null)
            {
                var paramsAsArray = ((JArray)paramsPayload).ToArray();
                var mappedParams = command.Method.GetParameters()
                    .Select((p, i) => Convert.ChangeType(paramsAsArray[i].ToObject(p.ParameterType), p.ParameterType))
                    .ToArray();
                result = command.DynamicInvoke(mappedParams);
            }
            else
            {
                result = command.DynamicInvoke();
            }

            Console.WriteLine($"Command {incomingMessage.Method} executed with result {result}");
            var payload = WebMessageFactory.CreateResponseMessage(incomingMessage.ResponseCallbackId, result);
            webView.CoreWebView2.PostWebMessageAsJson(payload);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error executing command {incomingMessage.Method}: {e.Message}");
            var errorPayload = WebMessageFactory.CreateResponseMessage(incomingMessage.ErrorCallbackId, e.Message);
            webView.CoreWebView2.PostWebMessageAsJson(errorPayload);
        }
    }
}
using System.Reflection;
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
                var methodParams = command.Method.GetParameters();
                var optionalParams = methodParams.Count(p => !p.HasDefaultValue);
                if (optionalParams > paramsAsArray.Length)
                    throw new Exception(
                        $"Expected at least {methodParams.Length} parameters, but got {paramsAsArray.Length}.");

                var mappedParams = methodParams
                    .Select((p, i) =>
                    {
                        if (i < paramsAsArray.Length)
                            return Convert.ChangeType(paramsAsArray[i].ToObject(p.ParameterType), p.ParameterType);
                        if (p.HasDefaultValue)
                            return p.DefaultValue;
                        if (p.ParameterType.IsValueType && Nullable.GetUnderlyingType(p.ParameterType) == null)
                            return Activator.CreateInstance(p.ParameterType);

                        return null;
                    })
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
            var innerException = e.InnerException ?? e;
            Console.WriteLine($"Error executing command {incomingMessage.Method}: {innerException.Message}");
            var errorPayload =
                WebMessageFactory.CreateResponseMessage(incomingMessage.ErrorCallbackId, innerException.Message);
            webView.CoreWebView2.PostWebMessageAsJson(errorPayload);
        }
    }
}
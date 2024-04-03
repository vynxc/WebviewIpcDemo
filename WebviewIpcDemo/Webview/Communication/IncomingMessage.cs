using Newtonsoft.Json;

namespace WebviewIpcDemo.Webview.Communication;

public class IncomingMessage
{
    public required long ResponseCallbackId { get; set; }

    public required long ErrorCallbackId { get; set; }
    public required string Method { get; set; }
    [JsonProperty("params")] public dynamic? Payload { get; set; }
}
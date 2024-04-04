using System.Diagnostics;
using System.Reflection;
using WebviewIpcDemo.Webview;

namespace WebviewIpcDemo;

public static class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        var isDev = args.Contains("dev");

        var src = isDev
            ? "http://localhost:5173/"
            : $"file:///{Path.Combine(
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ??
                throw new InvalidOperationException(), "wwwroot")}/index.html";


        Application.Run(new WebviewWindow(new Uri(src))
        {
            Width = 800,
            Height = 500
        });
    }
}
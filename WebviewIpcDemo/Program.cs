using System.Diagnostics;
using System.Reflection;
using WebviewIpcDemo.Webview;

namespace WebviewIpcDemo;

public static class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        Uri source;
        if (!args.Contains("dev"))
        {
            var pathToWwwroot =
                Path.Combine(
                    Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ??
                    throw new InvalidOperationException(), "wwwroot");
            source = new Uri($"file:///{pathToWwwroot}/index.html");
        }

        else
        {
            source = new Uri("http://localhost:5173/");
        }

        Application.Run(new WebviewWindow(source)
        {
            Width = 800,
            Height = 500
        });
    }
}
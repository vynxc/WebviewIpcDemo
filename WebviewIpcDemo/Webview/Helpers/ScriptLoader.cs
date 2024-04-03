using System.Reflection;

namespace WebviewIpcDemo.Webview.Helpers;

public class ScriptLoader(Assembly assembly)
{
    public string GetScript(string scriptName)
    {
        var stream =
            assembly.GetManifestResourceNames()
                .Where(x => x.Contains(scriptName))
                .Select(assembly.GetManifestResourceStream).FirstOrDefault();

        if (stream == null) throw new Exception($"Failed to load {scriptName}");

        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }
}
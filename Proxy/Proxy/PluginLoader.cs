using Proxy.Interface;

namespace Proxy;

public static class PluginLoader {
    private static readonly Logger Log = new(typeof(PluginLoader));

    public static void AttachPlugins(Proxy proxy) {
        var pluginType = typeof(IPlugin);
        var plugins = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(s => s.GetTypes())
            .Where(p => pluginType.IsAssignableFrom(p) && !p.IsInterface && !p.IsAbstract)
            .Select(Activator.CreateInstance)
            .Cast<IPlugin>()
            .ToList();

        foreach (var plugin in plugins) {
            plugin.Initialize(proxy);

            Log.Info($"Loaded plugin: {plugin.GetType().Name}");
        }
    }
}
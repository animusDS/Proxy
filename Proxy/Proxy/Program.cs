using Proxy.Networking;

namespace Proxy;

public static class Program {
    private static readonly Logger Log = new(typeof(Program));

    private static Proxy _proxy;
    private static GameData _gameData;
    private static Settings _settings;

    public static void Main() {
        StartProxy();

        while (true) {
            var command = Console.ReadLine();
            if (string.IsNullOrEmpty(command))
                continue;

            if (!HandleCommand(command))
                break;
        }
    }

    // ReSharper disable once StringLiteralTypo
    private static bool HandleCommand(string command) {
        var split = command.Split(' ');
        if (split.Length == 0)
            return true;

        var commandName = split[0];
        var commandArgs = split[1..];
        switch (commandName.ToLower()) {
            default:
                Log.Info("Unknown command!");
                break;
            case "con":
                ConCommand(commandArgs);
                break;
            case "goto":
                GotoCommand(commandArgs);
                break;
            case "ip":
                IpCommand();
                break;
            case "servers":
                ListServersCommand();
                break;
            case "exit":
                StopProxy();
                return false;
        }

        return true;
    }

    private static void ConCommand(IReadOnlyList<string> args) {
        if (!_proxy.Client.Connected) {
            Log.Info("Client not connected!");
            return;
        }

        if (args.Count == 0) {
            Log.Info("Invalid command!");
            return;
        }

        var server = args[0];
        ReconnectHandler.ExecuteConnectCommand(_proxy.Client, server);
    }

    private static void GotoCommand(IReadOnlyList<string> args) {
        if (!_proxy.Client.Connected) {
            Log.Info("Client not connected!");
            return;
        }

        if (args.Count == 0) {
            Log.Info("Invalid command!");
            return;
        }

        var host = args[0];
        ReconnectHandler.ExecuteGotoCommand(_proxy.Client, host);
    }

    private static void IpCommand() {
        if (!_proxy.Client.Connected) {
            Log.Info("Client not connected!");
            return;
        }

        ReconnectHandler.ExecuteIpCommand(_proxy.Client);
    }

    private static void ListServersCommand() {
        var servers = _gameData.Servers;
        foreach (var server in servers)
            Log.Info($"{server.Name} - {server.Ip}");
    }

    private static void StartProxy() {
        Log.Info("Starting proxy...");

        _settings = Settings.Load();

        Log.Info("Loaded Settings.");

        _gameData = new GameData();

        _proxy = new Proxy(_settings, _gameData);
        _proxy.StartListener();

        PluginLoader.AttachPlugins(_proxy);

        Log.Info("Proxy started.");
    }

    private static void StopProxy() {
        Log.Info("Stopping proxy...");

        _proxy.Client?.Dispose();
        Thread.Sleep(1000);

        Log.Info("Proxy stopped.");
    }
}
using Proxy.Networking.Packets;
using Proxy.Networking.Packets.Client;
using Proxy.Networking.Packets.Server;
using Proxy.Resources.DataStructures;

namespace Proxy.Networking;

public class ReconnectHandler {
    private static readonly Logger Log = new("ReconHandler");

    private static string _reconnectHost = Proxy.DefaultHost;
    private static ushort _reconnectPort = Proxy.GamePort;

    public void Attach(Proxy proxy) {
        proxy.HookPacket<Hello>(OnHello);
        proxy.HookPacket<Reconnect>(SendReconnect);

        proxy.HookCommand("con", OnConnectCommand);
        proxy.HookCommand("ip", OnIpCommand);
        proxy.HookCommand("goto", OnGotoCommand);
    }

    private void OnHello(Client client, Hello packet) {
        if (_reconnectHost == Proxy.LocalHost && _reconnectPort == Proxy.GamePort) {
            Log.Error("Cannot reconnect to local host.");
            return;
        }
        
        client.Connect(packet, _reconnectHost, _reconnectPort);
        packet.Send = false;
    }
    
    private static void SendReconnect(Client client, Reconnect reconnect) {
        var packet = (Reconnect) Packet.Create(PacketType.Reconnect);
        packet.Name = reconnect.Name;
        packet.Host = Proxy.LocalHost;
        packet.Port = Proxy.GamePort;
        packet.GameId = reconnect.GameId;
        packet.KeyTime = reconnect.KeyTime;
        packet.Key = reconnect.Key;
        client.SendToClient(packet);
        
        if (reconnect.Host != string.Empty) {
            _reconnectHost = reconnect.Host;
            _reconnectPort = reconnect.Port;
        }
    }

    private static void OnGotoCommand(Client client, string command, string[] args) {
        if (args.Length != 1) {
            return;
        }

        var host = args[0];
        var reconnect = (Reconnect) Packet.Create(PacketType.Reconnect);
        reconnect.Name = "Realm";
        reconnect.Host = host;
        reconnect.Port = Proxy.GamePort;
        reconnect.GameId = -2;
        reconnect.KeyTime = -1;
        reconnect.Key = [];
        SendReconnect(client, reconnect);

        Log.Info($"Connecting to {reconnect.Name} ({reconnect.Host}:{reconnect.Port}");
        client.CreateTextNotification("Reconnect Handler", $"Connecting to {reconnect.Name} ({reconnect.Host}:{reconnect.Port}");
    }

    private static void OnIpCommand(Client client, string command, string[] args) {
        client.CreateTextNotification("Reconnect Handler", $"IP:");
    }

    private static void OnConnectCommand(Client client, string command, string[] args) {
        if (args.Length != 1) {
            return;
        }

        var input = args[0];
        var server = ServerStructure.GetServer(client.Proxy.GameData.Servers, input);
        if (string.IsNullOrEmpty(server.Name)) {
            Log.Error($"Server {input} not found.");
            return;
        }

        var reconnect = (Reconnect) Packet.Create(PacketType.Reconnect);
        reconnect.Name = "Nexus";
        reconnect.Host = server.Ip;
        reconnect.Port = 2050;
        reconnect.GameId = -2;
        reconnect.KeyTime = -1;
        reconnect.Key = [];
        SendReconnect(client, reconnect);

        Log.Info($"Connecting to {server.Name} ({reconnect.Host}:{reconnect.Port}");
        client.CreateTextNotification("Reconnect Handler", $"Connecting to {server.Name} ({reconnect.Host}:{reconnect.Port}");
    }

    public static void ExecuteGotoCommand(Client client, string host) {
        var reconnect = (Reconnect) Packet.Create(PacketType.Reconnect);
        reconnect.Name = "Realm";
        reconnect.Host = host;
        reconnect.Port = Proxy.GamePort;
        reconnect.GameId = -2;
        reconnect.KeyTime = -1;
        reconnect.Key = [];
        SendReconnect(client, reconnect);

        Log.Info($"Connecting to {reconnect.Name} ({reconnect.Host}:{reconnect.Port}");
        client.CreateTextNotification("Reconnect Handler", $"Connecting to {reconnect.Name} ({reconnect.Host}:{reconnect.Port}");
    }

    public static void ExecuteIpCommand(Client client) {
        
    }

    public static void ExecuteConnectCommand(Client client, string serverName) {
        var server = ServerStructure.GetServer(client.Proxy.GameData.Servers, serverName);
        if (string.IsNullOrEmpty(server.Name)) {
            Log.Error($"Server {serverName} not found.");
            client.CreateTextNotification("Reconnect Handler", $"Server {serverName} not found.");
            return;
        }

        var reconnect = (Reconnect) Packet.Create(PacketType.Reconnect);
        reconnect.Name = "Nexus";
        reconnect.Host = server.Ip;
        reconnect.Port = 2050;
        reconnect.GameId = -2;
        reconnect.KeyTime = -1;
        reconnect.Key = [];
        SendReconnect(client, reconnect);

        Log.Info($"Connecting to {server.Name} ({reconnect.Host}:{reconnect.Port}");
        client.CreateTextNotification("Reconnect Handler", $"Connecting to {server.Name} ({reconnect.Host}:{reconnect.Port}");
    }
}
using System.Text;
using Proxy.Networking.Packets;
using Proxy.Networking.Packets.Client;
using Proxy.Networking.Packets.Server;
using Proxy.Resources.DataStructures;

namespace Proxy.Networking;

public class ReconnectHandler {
    private static readonly Logger Log = new("ReconHandler");

    public void Attach(Proxy proxy) {
        proxy.HookPacket<Hello>(OnHello);
        proxy.HookPacket<Reconnect>(OnReconnect);

        proxy.HookCommand("con", OnConnectCommand);
        proxy.HookCommand("ip", OnIpCommand);
        proxy.HookCommand("goto", OnGotoCommand);
    }

    private void OnHello(Client client, Hello packet) {
        client.State = client.Proxy.GetState(packet.Key);
        if (client.State.ConRealKey.Length != 255) // Todo: very scuffed, but needed for /con
        {
            packet.Key = client.State.ConRealKey;
            client.State.ConRealKey = new byte[255];
        }

        client.Connect(packet);
        packet.Send = false;
    }

    private void OnReconnect(Client client, Reconnect packet) {
        var recon = (Reconnect) Packet.Create(PacketType.Reconnect);
        recon.Name = packet.Name;
        recon.Host = string.IsNullOrEmpty(packet.Host) ? client.State.ConTargetAddress : packet.Host;
        recon.Port = packet.Port;
        recon.GameId = packet.GameId;
        recon.KeyTime = packet.KeyTime;
        recon.Key = packet.Key;

        if (!string.IsNullOrEmpty(packet.Host)) {
            client.State.ConTargetAddress = packet.Host;
        }

        if (packet.Key.Length != 0) {
            client.State.ConRealKey = packet.Key;
        }

        packet.Key = Encoding.UTF8.GetBytes(client.State.Guid);
        packet.Host = Proxy.LocalHost;
        packet.Port = Proxy.GamePort;

        Log.Info($"Reconnecting to {packet.Name} ({recon.Host}:{recon.Port})...");
    }

    private static void SendReconnect(Client client, Reconnect reconnect) {
        var host = reconnect.Host;
        var port = reconnect.Port;
        var key = reconnect.Key;
        client.State.ConTargetAddress = host;
        client.State.ConTargetPort = port;
        client.State.ConRealKey = key;
        reconnect.Key = Encoding.UTF8.GetBytes(client.State.Guid);
        reconnect.Host = Proxy.LocalHost;
        reconnect.Port = Proxy.GamePort;

        client.SendToClient(reconnect);

        reconnect.Key = key;
        reconnect.Host = host;
        reconnect.Port = port;
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
        reconnect.Key = Array.Empty<byte>();
        SendReconnect(client, reconnect);

        Log.Info($"Connecting to {reconnect.Name} ({reconnect.Host}:{reconnect.Port}");
        client.CreateTextNotification("Reconnect Handler", $"Connecting to {reconnect.Name} ({reconnect.Host}:{reconnect.Port}");
    }

    private static void OnIpCommand(Client client, string command, string[] args) {
        client.CreateTextNotification("Reconnect Handler", $"IP: {client.State.ConTargetAddress}");
    }

    private static void OnConnectCommand(Client client, string command, string[] args) {
        if (args.Length != 1)
            return;

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
        reconnect.Key = Array.Empty<byte>();
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
        reconnect.Key = Array.Empty<byte>();
        SendReconnect(client, reconnect);

        Log.Info($"Connecting to {reconnect.Name} ({reconnect.Host}:{reconnect.Port}");
        client.CreateTextNotification("Reconnect Handler", $"Connecting to {reconnect.Name} ({reconnect.Host}:{reconnect.Port}");
    }

    public static void ExecuteIpCommand(Client client) {
        var ip = client.State.ConTargetAddress;
        Log.Info($"IP: {ip}");
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
        reconnect.Key = Array.Empty<byte>();
        SendReconnect(client, reconnect);

        Log.Info($"Connecting to {server.Name} ({reconnect.Host}:{reconnect.Port}");
        client.CreateTextNotification("Reconnect Handler", $"Connecting to {server.Name} ({reconnect.Host}:{reconnect.Port}");
    }
}
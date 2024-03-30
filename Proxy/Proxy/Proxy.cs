using System.Net;
using System.Net.Sockets;
using System.Text;
using Proxy.Networking;
using Proxy.Networking.Packets;
using Proxy.Networking.Packets.Client;

namespace Proxy;

public class Proxy {
    private static readonly Logger Log = new(typeof(Proxy));

    public const string DefaultHost = "54.153.13.68";
    public const string LocalHost = "127.0.0.1";
    public const int GamePort = 2050;

    private readonly TcpListener _localListener = new(IPAddress.Parse(LocalHost), GamePort);

    public readonly Settings Settings;

    public readonly GameData GameData;

    public Client Client;

    public State State;

    public delegate void PacketHandler(Client client, Packet packet);

    private readonly Dictionary<PacketHandler, List<PacketType>> _packetHooks;

    public delegate void GenericPacketHandler<in T>(Client client, T packet) where T : Packet;

    private readonly Dictionary<object, Type> _genericPacketHooks;

    public delegate void CommandHandler(Client client, string command, string[] args);

    private readonly Dictionary<CommandHandler, List<string>> _commandHooks;

    public Proxy(Settings settings, GameData gameData) {
        Settings = settings;
        GameData = gameData;

        _packetHooks = new Dictionary<PacketHandler, List<PacketType>>();
        _genericPacketHooks = new Dictionary<object, Type>();
        _commandHooks = new Dictionary<CommandHandler, List<string>>();

        new StateManager().Attach(this);
        new ReconnectHandler().Attach(this);
    }

    public void StartListener() {
        try {
            _localListener.Start();
            _localListener.BeginAcceptTcpClient(LocalConnect, null);
        }
        catch (Exception e) {
            Log.Error($"Could not start listener. {e}");
        }
    }

    private void LocalConnect(IAsyncResult ar) {
        try {
            var tcpClient = _localListener.EndAcceptTcpClient(ar);
            Client = new Client(this, tcpClient);

            _localListener.BeginAcceptTcpClient(LocalConnect, null);
        }
        catch (Exception e) {
            Log.Error($"Could not accept client. {e}");
        }
    }

    public State GetState(byte[] key) {
        var guid = key.Length == 0 ? "n/a" : Encoding.UTF8.GetString(key);
        var newState = new State(Guid.NewGuid().ToString("n"));
        if (guid == "n/a") {
            State = newState;
            return newState;
        }

        newState.ConTargetAddress = State.ConTargetAddress;
        newState.ConTargetPort = State.ConTargetPort;
        newState.ConRealKey = State.ConRealKey;

        State = newState;

        return newState;
    }

    public void HookPacket<T>(GenericPacketHandler<T> callback) where T : Packet {
        if (_genericPacketHooks.ContainsKey(callback))
            throw new InvalidOperationException("Callback already bound");

        _genericPacketHooks.Add(callback, typeof(T));
    }

    public void HookPacket(PacketType type, PacketHandler callback) {
        if (_packetHooks.TryGetValue(callback, out var hook)) {
            hook.Add(type);
            return;
        }

        _packetHooks.Add(callback, new List<PacketType> { type });
    }

    public void HookCommand(string command, CommandHandler callback) {
        if (_commandHooks.TryGetValue(callback, out var hook)) {
            hook.Add(command);
            return;
        }

        _commandHooks.Add(callback, new List<string> {
            command[0] == '/'
                ? new string(command.Skip(1).ToArray()).ToLower()
                : command.ToLower(),
        });
    }

    public void FireServerPacket(Client client, Packet packet) {
        try {
            foreach (var pair in _genericPacketHooks.Where(pair => pair.Value == packet.GetType())) {
                if (pair.Key is not Delegate d)
                    continue;

                d.Method.Invoke(d.Target, new[] { client, Convert.ChangeType(packet, pair.Value) });
            }

            foreach (var pair in _packetHooks)
                if (pair.Value.Contains(packet.Type))
                    pair.Key(client, packet);
        }
        catch (Exception e) {
            Log.Error($"Error while firing server packet. {e}");
        }
    }

    public void FireClientPacket(Client client, Packet packet) {
        try {
            if (packet.Type == PacketType.PlayerText) {
                var playerText = (PlayerText) packet;
                var text = playerText.Text.Replace("/", "").ToLower();
                var command = text.Contains(' ')
                    ? text.Split(' ')[0].ToLower()
                    : text;

                var args = text.Contains(' ')
                    ? text.Split(' ').Skip(1).ToArray()
                    : Array.Empty<string>();

                foreach (var pair in _commandHooks.Where(pair => pair.Value.Contains(command))) {
                    packet.Send = false;
                    pair.Key(client, command, args);
                }
            }

            foreach (var pair in _genericPacketHooks.Where(pair => pair.Value == packet.GetType())) {
                if (pair.Key is not Delegate d)
                    continue;

                d.Method.Invoke(d.Target, new[] { client, Convert.ChangeType(packet, pair.Value) });
            }

            foreach (var pair in _packetHooks)
                if (pair.Value.Contains(packet.Type))
                    pair.Key(client, packet);
        }
        catch (Exception e) {
            Log.Error($"Error while firing client packet. {e}");
        }
    }
}
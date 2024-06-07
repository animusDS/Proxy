using System.Net;
using System.Net.Sockets;
using Proxy.Networking;
using Proxy.Networking.Packets;
using Proxy.Networking.Packets.Client;
using Titanium.Web.Proxy;
using Titanium.Web.Proxy.EventArguments;
using Titanium.Web.Proxy.Models;
using Titanium.Web.Proxy.Network;

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

    private readonly ProxyServer _httpProxy;
    
    public delegate void PacketHandler(Client client, Packet packet);
    private readonly Dictionary<PacketHandler, List<PacketType>> _packetHooks;

    public delegate void GenericPacketHandler<in T>(Client client, T packet) where T : Packet;
    private readonly Dictionary<object, Type> _genericPacketHooks;

    public delegate void CommandHandler(Client client, string command, string[] args);
    private readonly Dictionary<CommandHandler, List<string>> _commandHooks;

    private bool _shuttingDown;

    public Proxy(Settings settings, GameData gameData) {
        Settings = settings;
        GameData = gameData;

        _packetHooks = new Dictionary<PacketHandler, List<PacketType>>();
        _genericPacketHooks = new Dictionary<object, Type>();
        _commandHooks = new Dictionary<CommandHandler, List<string>>();

        new StateManager().Attach(this);
        new ReconnectHandler().Attach(this);

        // _httpProxy = new ProxyServer();
        // _httpProxy.CertificateManager.CreateRootCertificate();
        // _httpProxy.CertificateManager.TrustRootCertificate(true);
        // _httpProxy.CertificateManager.CertificateEngine = CertificateEngine.BouncyCastleFast;
        //
        // var explicitEndPoint = new ExplicitProxyEndPoint(IPAddress.Any, 8000);
        // explicitEndPoint.BeforeTunnelConnectRequest += OnBeforeTunnelConnectRequest;
        //
        // _httpProxy.BeforeResponse += OnHttpResponse;
        //
        // _httpProxy.AddEndPoint(explicitEndPoint);
        // _httpProxy.Start();
        //
        // _httpProxy.SetAsSystemHttpProxy(explicitEndPoint);
        // _httpProxy.SetAsSystemHttpsProxy(explicitEndPoint);
        //
        // AppDomain.CurrentDomain.ProcessExit += OnExit;
        // Console.CancelKeyPress += (sender, e) => {
        //     OnExit(sender, e);
        //     Environment.Exit(0);
        // };
    }
    
    private void OnExit(object sender, EventArgs e) {
        Log.Info("Shutting down HTTP proxy...");
        
        _shuttingDown = true;
            
        _httpProxy.Stop();
        _httpProxy.Dispose();
    }

    private static Task OnBeforeTunnelConnectRequest(object sender, TunnelConnectSessionEventArgs e) {
        if (!e.HttpClient.Request.Url.Contains("realmofthemadgod")) {
            e.DecryptSsl = false;
        }
        
        return Task.CompletedTask;
    }

    private static async Task OnHttpResponse(object sender, SessionEventArgs e) {
        var responseString = await e.GetResponseBodyAsString();
        e.SetResponseBodyString(responseString.Replace("<Servers>",
            "<Servers>" +
            "<Server>" +
            $"<Name>~I love femboys~</Name>" +
            "<DNS>127.0.0.1</DNS>" +
            "<Lat>32.80</Lat>" +
            "<Long>-96.77</Long>" +
            "<Usage>0.00</Usage>" +
            "</Server>"));
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
using System.Net;
using System.Net.Sockets;
using Proxy.Crypto;
using Proxy.Networking.Packets;
using Proxy.Networking.Packets.Client;

namespace Proxy.Networking;

public class Client {
    private static readonly Logger Log = new(typeof(Client));

    private const string ClientKey = "5a4d2016bc16dc64883194ffd9";
    private const string ServerKey = "c91d9eec420160730d825604e0";

    private readonly TcpClient _clientConnection;
    private TcpClient _serverConnection;

    private readonly NetworkStream _clientStream;
    private NetworkStream _serverStream;

    private readonly PacketBuffer _clientBuffer;
    private readonly PacketBuffer _serverBuffer;

    private readonly Rc4Cipher _clientReceiveState;
    private readonly Rc4Cipher _clientSendState;

    private readonly Rc4Cipher _serverReceiveState;
    private readonly Rc4Cipher _serverSendState;

    private readonly object _clientLock;
    private readonly object _serverLock;

    public readonly Proxy Proxy;
    
    public PlayerData PlayerData;
    
    public int LastUpdate = 0;
    public int PreviousTime = 0;

    public bool Connected => _clientConnection.Connected && _serverConnection.Connected;
    
    private bool _disposed;

    public Client(Proxy proxy, TcpClient clientConnection) {
        Proxy = proxy;

        _clientConnection = clientConnection;
        _clientConnection.NoDelay = true;

        _clientStream = _clientConnection.GetStream();

        _clientBuffer = new PacketBuffer();
        _serverBuffer = new PacketBuffer();

        _clientReceiveState = new Rc4Cipher(ClientKey);
        _clientSendState = new Rc4Cipher(ServerKey);

        _serverReceiveState = new Rc4Cipher(ServerKey);
        _serverSendState = new Rc4Cipher(ClientKey);

        _clientLock = new object();
        _serverLock = new object();

        BeginRead(0, 4, true);
    }

    public void Connect(Hello hello, string targetAddress, int targetPort) {
        _serverConnection = new TcpClient { NoDelay = true };
        _serverConnection.BeginConnect(targetAddress, targetPort, ServerConnected, hello);

        Log.Info("Client connected");
    }

    private void ServerConnected(IAsyncResult ar) {
        if (_disposed)
            return;

        try {
            _serverConnection.EndConnect(ar);
            _serverStream = _serverConnection.GetStream();

            if (ar.AsyncState is Packet packet) {
                SendToServer(packet);
            }
            else {
                throw new Exception("Invalid packet type.");
            }

            BeginRead(0, 4, false);
        }
        catch (Exception e) {
            Log.Error($"Could not connect to remote host. {e}");
            Dispose();
        }
    }

    private void BeginRead(int offset, int amount, bool isClient) {
        if (_disposed) {
            return;
        }

        var buffer = isClient ? _clientBuffer : _serverBuffer;
        var stream = isClient ? _clientStream : _serverStream;
        stream.BeginRead(buffer.Bytes, offset, amount, RemoteRead,
            new Tuple<NetworkStream, PacketBuffer>(stream, buffer));
    }

    private void RemoteRead(IAsyncResult ar) {
        var stream = (ar.AsyncState as Tuple<NetworkStream, PacketBuffer>)?.Item1;
        var buffer = (ar.AsyncState as Tuple<NetworkStream, PacketBuffer>)?.Item2;
        if (stream == null || buffer == null || !stream.CanRead || _disposed)
            return;

        try {
            var read = stream.EndRead(ar);
            buffer.Advance(read);

            if (read == 0) {
                Dispose();
                return;
            }

            var isClient = stream == _clientStream;
            if (buffer.Index == 4) {
                buffer.Resize(IPAddress.NetworkToHostOrder(
                    BitConverter.ToInt32(buffer.Bytes, 0)));

                BeginRead(buffer.Index, buffer.BytesRemaining(), isClient);
            }
            else if (buffer.BytesRemaining() > 0) {
                BeginRead(buffer.Index, buffer.BytesRemaining(), isClient);
            }
            else {
                var cipher = isClient ? _clientReceiveState : _serverReceiveState;
                cipher.Cipher(buffer.Bytes);

                var packet = Packet.Create(buffer.Bytes);
                if (isClient) {
                    Proxy.FireClientPacket(this, packet);
                }
                else {
                    Proxy.FireServerPacket(this, packet);
                }

                if (packet.Send)
                    Send(packet, !isClient);

                buffer.Reset();
                BeginRead(0, 4, isClient);
            }
        }
        catch (Exception e) {
            Log.Error($"Remote Read Error: {e}");
            Dispose();
        }
    }

    private void Send(Packet packet, bool client) {
        lock (client ? _clientLock : _serverLock) {
            try {
                var ms = new MemoryStream();
                using (var w = new PacketWriter(ms)) {
                    w.Write(0);
                    w.Write(packet.Id);

                    packet.Write(w);
                    foreach (var b in packet.UnreadData)
                        w.Write(b);
                }

                var data = ms.ToArray();
                PacketWriter.BlockCopyInt32(data, data.Length);

                if (client) {
                    _clientSendState.Cipher(data);
                    _clientStream.Write(data, 0, data.Length);
                }
                else {
                    _serverSendState.Cipher(data);
                    _serverStream.Write(data, 0, data.Length);
                }
            }
            catch (Exception e) {
                Log.Error($"Send Error for packet [{packet?.Type ?? PacketType.Undefined}]: {e}");
                Dispose();
            }
        }
    }

    public void SendToClient(Packet packet) => Send(packet, true);

    public void SendToServer(Packet packet) => Send(packet, false);

    public void Dispose() {
        if (_disposed)
            return;

        _disposed = true;

        _clientStream?.Close();
        _serverStream?.Close();

        _clientConnection?.Close();
        _serverConnection?.Close();

        Log.Info("Client Disconnected");
    }
}
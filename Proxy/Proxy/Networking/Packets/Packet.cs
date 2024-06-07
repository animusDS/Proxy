using Proxy.Networking.Packets.Client;
using Proxy.Networking.Packets.Server;

namespace Proxy.Networking.Packets;

public class Packet {
    public byte Id;
    public bool Send = true;
    public byte[] UnreadData = [];

    private byte[] _data;

    public virtual PacketType Type { get; protected set; } = PacketType.Undefined;

    protected virtual void Read(PacketReader r) {
        _data = r.ReadBytes((int) r.BaseStream.Length - 5);
    }

    protected internal virtual void Write(PacketWriter w) {
        w.Write(_data);
    }

    public static Packet Create(PacketType type) {
        Packet packet;
        switch (type) {
            case PacketType.Failure:
                packet = new Failure();
                Logger.Info(packet);
                break;
            case PacketType.Reconnect:
                packet = new Reconnect();
                break;
            case PacketType.Hello:
                packet = new Hello();
                break;
            case PacketType.PlayerText:
                packet = new PlayerText();
                break;
            case PacketType.Text:
                packet = new Text();
                break;
            case PacketType.MapInfo:
                packet = new MapInfo();
                break;
            case PacketType.Update:
                packet = new Update();
                break;
            case PacketType.CreateSuccess:
                packet = new CreateSuccess();
                break;
            default:
                //Logger.Warn($"Unknown packet type: {(int) type} ({type})");
                packet = new UndefinedPacket();
                break;
        }

        packet.Type = type;
        packet.Id = (byte) type;

        return packet;
    }

    public static Packet Create(byte[] data) {
        using var r = new PacketReader(new MemoryStream(data));
        r.ReadInt32();

        var id = r.ReadByte();
        var type = (PacketType) id;
        var packet = Create(type);
        packet.Id = id;
        packet.Read(r);

        if (r.BaseStream.Position != r.BaseStream.Length) {
            packet.UnreadData = r.ReadBytes((int) (r.BaseStream.Length - r.BaseStream.Position));

            Logger.Warn($"Unread data in packet {packet.Type} ({packet.Id}): {BitConverter.ToString(packet.UnreadData)}");
        }

        return packet;
    }
}

public enum PacketType : byte {
    Failure = 0,
    PlayerText = 9,
    NewTick = 10,
    UseItem = 13,
    PlayerShoot = 30,
    Update = 42,
    Text = 44,
    Reconnect = 45,
    Create = 59,
    Load = 61,
    Move = 62,
    KeyInfoResponse = 63,
    Notification = 67,
    Hello = 74,
    Buy = 85,
    MapInfo = 92,
    KeyInfoRequest = 94,
    InventoryResult = 95,
    CreateSuccess = 101,
    Escape = 105,
    ForgeRequest = 118,
    ForgeResult = 119,
    Undefined = 255,
}

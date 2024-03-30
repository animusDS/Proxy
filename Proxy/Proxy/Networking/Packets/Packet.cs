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
        Packet packet = type switch {
            PacketType.PlayerText => new PlayerText(),
            PacketType.Text => new Text(),
            PacketType.Update => new Update(),
            PacketType.Reconnect => new Reconnect(),
            PacketType.Notification => new Notification(),
            PacketType.Hello => new Hello(),
            PacketType.CreateSuccess => new CreateSuccess(),
            _ => new UndefinedPacket(),
        };

        packet.Type = type;
        packet.Id = (byte) type;

        return packet;
    }

    public static Packet Create(byte[] data) {
        using var r = new PacketReader(new MemoryStream(data));
        r.ReadInt32();

        var id = r.ReadByte();
        var type = (PacketType) id;
        if (!Enum.IsDefined(typeof(PacketType), type)) {
            type = PacketType.Undefined;
        }

        var packet = Create(type);
        packet.Id = id;
        packet.Read(r);

        return packet;
    }
}

public enum PacketType : byte {
    Undefined = 255,

    PlayerText = 9,
    Update = 42,
    Text = 44,
    Reconnect = 45,
    Notification = 67,
    Hello = 74,
    CreateSuccess = 101,
}
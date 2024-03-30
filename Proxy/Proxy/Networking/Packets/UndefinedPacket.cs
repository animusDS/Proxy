namespace Proxy.Networking.Packets;

public class UndefinedPacket : Packet {
    public override PacketType Type => PacketType.Undefined;

    private byte[] _bytes;

    protected override void Read(PacketReader r) {
        var bytesAvailable = r.BaseStream.Length - 5;
        _bytes = new byte[bytesAvailable];

        for (var i = 0; i < bytesAvailable; i++)
            _bytes[i] = r.ReadByte();
    }

    protected internal override void Write(PacketWriter w) {
        foreach (var b in _bytes)
            w.Write(b);
    }
}
namespace Proxy.Networking.Packets.Client;

public class Escape : Packet {
    public override PacketType Type => PacketType.Escape;

    protected override void Read(PacketReader r) {
    }

    protected internal override void Write(PacketWriter w) {
    }

    public override string ToString() {
        return "";
    }
}
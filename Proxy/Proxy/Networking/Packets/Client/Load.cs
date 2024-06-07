using Proxy.Networking.Packets.DataObjects.Location;

namespace Proxy.Networking.Packets.Client;

public class Load : Packet {
    public int CharId;
    public bool IsFromArena;
    
    public override PacketType Type => PacketType.Load;

    protected override void Read(PacketReader r) {
        CharId = r.ReadInt32();
        IsFromArena = r.ReadBoolean();
    }

    protected internal override void Write(PacketWriter w) {
        w.Write(CharId);
        w.Write(IsFromArena);
    }
    
    public override string ToString() {
        return $"{Type} ({CharId}, {IsFromArena})";
    }
}
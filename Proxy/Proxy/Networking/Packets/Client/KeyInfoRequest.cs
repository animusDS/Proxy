namespace Proxy.Networking.Packets.Client;

public class KeyInfoRequest : Packet {
    public int ItemType;
    
    public override PacketType Type => PacketType.KeyInfoRequest;
    
    protected override void Read(PacketReader r) {
        ItemType = r.ReadInt32();
    }
    
    protected internal override void Write(PacketWriter w) {
        w.Write(ItemType);
    }
    
    public override string ToString() {
        return $"ItemType: {ItemType}";
    }
}
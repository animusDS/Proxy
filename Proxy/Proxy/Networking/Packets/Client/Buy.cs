using Proxy.Networking.Packets.DataObjects.Location;

namespace Proxy.Networking.Packets.Client;

public class Buy : Packet {
    public int ObjectId;
    public int Quantity;
    
    public override PacketType Type => PacketType.Buy;

    protected override void Read(PacketReader r) {
        ObjectId = r.ReadInt32();
        Quantity = r.ReadInt32();
    }

    protected internal override void Write(PacketWriter w) {
        w.Write(ObjectId);
        w.Write(Quantity);
    }
    
    public override string ToString() {
        return $"{Type} ({ObjectId}, {Quantity})";
    }
}
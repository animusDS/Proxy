using Proxy.Networking.Packets.DataObjects.Data;
using Proxy.Networking.Packets.DataObjects.Location;

namespace Proxy.Networking.Packets.Client;

public class UseItem : Packet {
    public int Time;
    public SlotObjectData SlotObject = new();
    public Position Position = new();
    public byte UseType;
    
    public override PacketType Type => PacketType.UseItem;

    protected override void Read(PacketReader r) {
        Time = r.ReadInt32();
        SlotObject.Read(r);
        Position.Read(r);
        UseType = r.ReadByte();
    }

    protected internal override void Write(PacketWriter w) {
        w.Write(Time);
        SlotObject.Write(w);
        Position.Write(w);
        w.Write(UseType);
    }
    
    public override string ToString() {
        return $"Time: {Time}," +
               $" SlotObject: {SlotObject}," +
               $" Position: {Position}," +
               $" UseType: {UseType}";
    }
}
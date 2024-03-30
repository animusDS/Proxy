using Proxy.Networking.Packets.DataObjects.Data;

namespace Proxy.Networking.Packets.Server;

public class InvResult : Packet {
    public bool UnknownBool;
    public byte UnknownByte;
    public SlotObjectData FromSlot;
    public SlotObjectData ToSlot;
    public int UnknownInt;
    public int UnknownInt2;
    
    public override PacketType Type => PacketType.InvResult;

    protected override void Read(PacketReader r) {
        UnknownBool = r.ReadBoolean();
        UnknownByte = r.ReadByte();
        FromSlot = new SlotObjectData();
        FromSlot.Read(r);
        ToSlot = new SlotObjectData();
        ToSlot.Read(r);
        UnknownInt = r.ReadInt32();
        UnknownInt2 = r.ReadInt32();
    }

    protected internal override void Write(PacketWriter w) {
        w.Write(UnknownBool);
        w.Write(UnknownByte);
        FromSlot.Write(w);
        ToSlot.Write(w);
        w.Write(UnknownInt);
        w.Write(UnknownInt2);
    }
    
    public override string ToString() {
        return $"UnknownBool: {UnknownBool}," +
               $" UnknownByte: {UnknownByte}," +
               $" FromSlot: {FromSlot}," +
               $" ToSlot: {ToSlot}," +
               $" UnknownInt: {UnknownInt}," +
               $" UnknownInt2: {UnknownInt2}";
    }
}
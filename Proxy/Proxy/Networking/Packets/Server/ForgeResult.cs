using Proxy.Networking.Packets.DataObjects.Data;

namespace Proxy.Networking.Packets.Server;

public class ForgeResult : Packet {
    public bool Success;
    public SlotObjectData[] Slots;
    
    public override PacketType Type => PacketType.ForgeResult;
    
    protected override void Read(PacketReader r) {
        Success = r.ReadBoolean();
        
        Slots = new SlotObjectData[r.ReadByte()];
        for (var i = 0; i < Slots.Length; i++) {
            Slots[i] = new SlotObjectData();
            Slots[i].Read(r);
        }
    }
    
    protected internal override void Write(PacketWriter w) {
        w.Write(Success);
        
        w.Write((byte) Slots.Length);
        foreach (var slot in Slots) {
            slot.Write(w);
        }
    }
    
    public override string ToString() {
        return $"Success: {Success}," +
               $" Slots: {Slots}";
    }
}
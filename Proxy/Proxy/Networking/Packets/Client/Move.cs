using Proxy.Networking.Packets.DataObjects.Location;

namespace Proxy.Networking.Packets.Client;

public class Move : Packet {
    public int TickId;
    public uint Time;
    public MoveRecord[] Records;
    
    public override PacketType Type => PacketType.Move;

    protected override void Read(PacketReader r) {
        TickId = r.ReadInt32();
        Time = r.ReadUInt32();
        
        Records = new MoveRecord[r.ReadInt16()];
        for (var i = 0; i < Records.Length; i++) {
            Records[i] = new MoveRecord();
            Records[i].Read(r);
        }
    }

    protected internal override void Write(PacketWriter w) {
        w.Write((short) Records.Length);
        foreach (var record in Records) {
            record.Write(w);
        }
    }
    
    public override string ToString() {
        return $"TickId: {TickId}, Time: {Time}, Records: {Records}";
    }
}
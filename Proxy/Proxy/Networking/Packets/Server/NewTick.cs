using Proxy.Networking.Packets.DataObjects.Stats;

namespace Proxy.Networking.Packets.Server;

public class NewTick : Packet {
    public int TickId;
    public int TickTime;
    public uint ServerRealTimeMs;
    public ushort ServerLastTimeRtMs;
    public Status[] Statuses;
    
    public override PacketType Type => PacketType.NewTick;

    protected override void Read(PacketReader r) {
        TickId = r.ReadInt32();
        TickTime = r.ReadInt32();
        ServerRealTimeMs = r.ReadUInt32();
        ServerLastTimeRtMs = r.ReadUInt16();
        
        Statuses = new Status[r.ReadInt16()];
        for (var i = 0; i < Statuses.Length; i++) {
            Statuses[i] = new Status();
            Statuses[i].Read(r);
        }
    }

    protected internal override void Write(PacketWriter w) {
        w.Write(TickId);
        w.Write(TickTime);
        w.Write(ServerRealTimeMs);
        w.Write(ServerLastTimeRtMs);
        
        w.Write((short) Statuses.Length);
        foreach (var status in Statuses) {
            status.Write(w);
        }
    }
    
    public override string ToString() {
        return $"TickId: {TickId}," +
               $" TickTime: {TickTime}," +
               $" ServerRealTimeMs: {ServerRealTimeMs}," +
               $" ServerLastTimeRtMs: {ServerLastTimeRtMs}," +
               $" Statuses: {Statuses}";
    }
}
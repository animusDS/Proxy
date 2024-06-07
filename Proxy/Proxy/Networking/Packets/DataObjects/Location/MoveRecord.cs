namespace Proxy.Networking.Packets.DataObjects.Location;

public sealed class MoveRecord : IDataObject {
    public int Time;
    public Position Position = new();

    public void Read(PacketReader r) {
        Time = r.ReadInt32();
        Position.Read(r);
    }

    public void Write(PacketWriter w) {
        w.Write(Time);
        Position.Write(w);
    }

    public object Clone() {
        return new MoveRecord {
            Position = Position.Clone() as Position,
        };
    }

    public override string ToString() {
        return $"Time: {Time}, Position: {Position}";
    }
}
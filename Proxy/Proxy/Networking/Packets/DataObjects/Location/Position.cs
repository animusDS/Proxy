namespace Proxy.Networking.Packets.DataObjects.Location;

public sealed class Position : IDataObject {
    public float X;
    public float Y;

    public Position() {
    }

    public Position(PacketReader r) => Read(r);

    public IDataObject Read(PacketReader r) {
        X = r.ReadSingle();
        Y = r.ReadSingle();
        return this;
    }

    public void Write(PacketWriter w) {
        w.Write(X);
        w.Write(Y);
    }

    public object Clone() {
        return new Position {
            X = X,
            Y = Y,
        };
    }

    public override string ToString() {
        return "{ X=" + X + ", Y=" + Y + " }";
    }
}
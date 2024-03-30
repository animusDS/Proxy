namespace Proxy.Networking.Packets.DataObjects.Location;

public sealed class Position : IDataObject {
    public float X;
    public float Y;

    public void Read(PacketReader r) {
        X = r.ReadSingle();
        Y = r.ReadSingle();
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
    
    public static bool operator ==(Position a, Position b) {
        return a.X == b.X && a.Y == b.Y;
    }
    
    public static bool operator !=(Position a, Position b) {
        return !(a == b);
    }
    
    public static Position operator +(Position a, Position b) {
        return new Position {
            X = a.X + b.X,
            Y = a.Y + b.Y,
        };
    }
    
    public static Position operator -(Position a, Position b) {
        return new Position {
            X = a.X - b.X,
            Y = a.Y - b.Y,
        };
    }
    
    
}
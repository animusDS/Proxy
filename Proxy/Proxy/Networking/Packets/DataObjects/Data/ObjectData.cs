using Proxy.Networking.Packets.DataObjects.Stats;

namespace Proxy.Networking.Packets.DataObjects.Data;

public class ObjectData : IDataObject {
    public ushort ObjectType;
    public Status Status = new();

    public void Read(PacketReader r) {
        ObjectType = r.ReadUInt16();
        Status.Read(r);
    }

    public void Write(PacketWriter w) {
        w.Write(ObjectType);
        Status.Write(w);
    }

    public object Clone() {
        return new ObjectData {
            ObjectType = ObjectType,
            Status = (Status) Status.Clone(),
        };
    }
    
    public override string ToString() {
        return $"ObjectType: {ObjectType}," +
               $" Status: {Status}";
    }
}
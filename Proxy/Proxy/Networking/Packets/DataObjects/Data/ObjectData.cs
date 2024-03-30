using Proxy.Networking.Packets.DataObjects.Stats;

namespace Proxy.Networking.Packets.DataObjects.Data;

public class ObjectData : IDataObject {
    public ushort ObjectType;
    public Status Status = new();

    private ObjectData() {
    }

    public ObjectData(PacketReader r) => Read(r);

    public IDataObject Read(PacketReader r) {
        ObjectType = r.ReadUInt16();
        Status.Read(r);
        return this;
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
}
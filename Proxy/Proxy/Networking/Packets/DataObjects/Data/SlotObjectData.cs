namespace Proxy.Networking.Packets.DataObjects.Data;

public class SlotObjectData : IDataObject {
    public int ObjectId;
    public int SlotId;
    public int ObjectType;

    public void Read(PacketReader r) {
        ObjectId = r.ReadInt32();
        SlotId = r.ReadInt32();
        ObjectType = r.ReadInt32();
    }

    public void Write(PacketWriter w) {
        w.Write(ObjectId);
        w.Write(SlotId);
        w.Write(ObjectType);
    }

    public object Clone() {
        return new SlotObjectData {
            ObjectId = ObjectId,
            SlotId = SlotId,
            ObjectType = ObjectType,
        };
    }
    
    public override string ToString() {
        return $"ObjectId: {ObjectId}," +
               $" SlotId: {SlotId}," +
               $" ObjectType: {ObjectType}";
    }
}
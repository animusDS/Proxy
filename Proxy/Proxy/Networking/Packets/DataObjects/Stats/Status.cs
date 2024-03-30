using Proxy.Networking.Packets.DataObjects.Data;

namespace Proxy.Networking.Packets.DataObjects.Stats;

public class Status : IDataObject {
    public int ObjectId;
    public Location.Position Position = new();
    public StatData[] Data;

    public void Read(PacketReader r) {
        ObjectId = CompressedInt.Read(r);
        Position.Read(r);
        
        Data = new StatData[CompressedInt.Read(r)];
        for (var i = 0; i < Data.Length; i++) {
            Data[i] = new StatData();
            Data[i].Read(r);
        }
    }

    public void Write(PacketWriter w) {
        CompressedInt.Write(w, ObjectId);
        Position.Write(w);
        
        CompressedInt.Write(w, Data.Length);
        foreach (var statData in Data) {
            statData.Write(w);
        }
    }

    public object Clone() {
        return new Status {
            Data = (StatData[]) Data.Clone(),
            ObjectId = ObjectId,
            Position = (Location.Position) Position.Clone(),
        };
    }
    
    public override string ToString() {
        return $"ObjectId: {ObjectId}," +
               $" Position: {Position}," +
               $" Data: {Data}";
    }
}
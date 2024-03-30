using Proxy.Networking.Packets.DataObjects;
using Proxy.Networking.Packets.DataObjects.Data;
using Proxy.Networking.Packets.DataObjects.Location;

namespace Proxy.Networking.Packets.Server;

public class Update : Packet {
    public Position Position;
    public byte LevelType;
    public Tile[] Tiles;
    public ObjectData[] NewObjects;
    public int[] RemovedObjectIds;
    
    public override PacketType Type => PacketType.Update;

    protected override void Read(PacketReader r) {
        Position = new Position(r);
        LevelType = r.ReadByte();

        Tiles = new Tile[CompressedInt.Read(r)];
        for (var i = 0; i < Tiles.Length; i++)
            Tiles[i] = new Tile(r);

        NewObjects = new ObjectData[CompressedInt.Read(r)];
        for (var i = 0; i < NewObjects.Length; i++)
            NewObjects[i] = new ObjectData(r);

        RemovedObjectIds = new int[CompressedInt.Read(r)];
        for (var i = 0; i < RemovedObjectIds.Length; i++)
            RemovedObjectIds[i] = CompressedInt.Read(r);
    }

    protected internal override void Write(PacketWriter w) {
        Position.Write(w);
        w.Write(LevelType);

        CompressedInt.Write(w, Tiles.Length);
        foreach (var tile in Tiles)
            tile.Write(w);

        CompressedInt.Write(w, NewObjects.Length);
        foreach (var entity in NewObjects)
            entity.Write(w);

        CompressedInt.Write(w, RemovedObjectIds.Length);
        foreach (var id in RemovedObjectIds)
            CompressedInt.Write(w, id);
    }
}
using Proxy.Networking.Packets.DataObjects.Location;

namespace Proxy.Networking.Packets.Client;

public class PlayerShoot : Packet {
    public int Time;
    public short BulletId;
    public short ContainerType;
    public byte UnknownByte;
    public Position StartingPos = new();
    public float Angle;
    public bool IsBurst;
    public short UnknownShort;
    public Position EndPos = new();
    
    public override PacketType Type => PacketType.PlayerShoot;

    protected override void Read(PacketReader r) {
        Time = r.ReadInt32();
        BulletId = r.ReadInt16();
        ContainerType = r.ReadInt16();
        UnknownByte = r.ReadByte();
        StartingPos.Read(r);
        Angle = r.ReadSingle();
        IsBurst = r.ReadBoolean();
        UnknownShort = r.ReadInt16();
        EndPos.Read(r);
    }

    protected internal override void Write(PacketWriter w) {
        w.Write(Time);
        w.Write(BulletId);
        w.Write(ContainerType);
        w.Write(UnknownByte);
        StartingPos.Write(w);
        w.Write(Angle);
        w.Write(IsBurst);
        w.Write(UnknownShort);
        EndPos.Write(w);
    }
    
    public override string ToString() {
        return $"Time: {Time}," +
               $" BulletId: {BulletId}," +
               $" ContainerType: {ContainerType}," +
               $" UnknownByte: {UnknownByte}," +
               $" StartingPos: {StartingPos}," +
               $" Angle: {Angle}," +
               $" IsBurst: {IsBurst}," +
               $" UnknownShort: {UnknownShort}," +
               $" EndPos: {EndPos}";
    }
}
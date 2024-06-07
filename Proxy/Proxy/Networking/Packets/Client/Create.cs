using Proxy.Networking.Packets.DataObjects.Location;

namespace Proxy.Networking.Packets.Client;

public class Create : Packet {
    public short ClassType;
    public short SkinType;
    public bool IsChallenger;
    public bool IsSeasonal;
    
    public override PacketType Type => PacketType.Create;

    protected override void Read(PacketReader r) {
        ClassType = r.ReadInt16();
        SkinType = r.ReadInt16();
        IsChallenger = r.ReadBoolean();
        IsSeasonal = r.ReadBoolean();
    }

    protected internal override void Write(PacketWriter w) {
        w.Write(ClassType);
        w.Write(SkinType);
        w.Write(IsChallenger);
        w.Write(IsSeasonal);
    }
    
    public override string ToString() {
        return $"{Type} ({ClassType}, {SkinType}, {IsChallenger}, {IsSeasonal})";
    }
}
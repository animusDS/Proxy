namespace Proxy.Networking.Packets.Server;

public class Text : Packet {
    public string Name = "";
    public int ObjectId;
    public ushort NumStars;
    public byte BubbleTime;
    public string Recipient = "";
    public string DirtyText = "";
    public string CleanText = "";
    public bool IsSupporter;
    public int StarBackground;
    
    public override PacketType Type => PacketType.Text;

    protected override void Read(PacketReader r) {
        Name = r.ReadString();
        ObjectId = r.ReadInt32();
        NumStars = r.ReadUInt16();
        BubbleTime = r.ReadByte();
        Recipient = r.ReadString();
        DirtyText = r.ReadString();
        CleanText = r.ReadString();
        IsSupporter = r.ReadBoolean();
        StarBackground = r.ReadInt32();
    }

    protected internal override void Write(PacketWriter w) {
        w.Write(Name);
        w.Write(ObjectId);
        w.Write(NumStars);
        w.Write(BubbleTime);
        w.Write(Recipient);
        w.Write(DirtyText);
        w.Write(CleanText);
        w.Write(IsSupporter);
        w.Write(StarBackground);
    }
}
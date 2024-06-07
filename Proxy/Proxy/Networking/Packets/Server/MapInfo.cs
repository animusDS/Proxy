namespace Proxy.Networking.Packets.Server;

public class MapInfo : Packet {
    public int Width;
    public int Height;
    public string Name;
    public string DisplayName;

    public override PacketType Type => PacketType.MapInfo;

    protected override void Read(PacketReader r) {
        Width = r.ReadInt32();
        Height = r.ReadInt32();
        Name = r.ReadString();
        DisplayName = r.ReadString();
    }

    protected internal override void Write(PacketWriter w) {
        w.Write(Width);
        w.Write(Height);
        w.Write(Name);
        w.Write(DisplayName);
    }
}
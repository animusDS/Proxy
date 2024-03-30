namespace Proxy.Networking.Packets.Client;

public class PlayerText : Packet {
    public string Text;
    
    public override PacketType Type => PacketType.PlayerText;

    protected override void Read(PacketReader r) {
        Text = r.ReadString();
    }

    protected internal override void Write(PacketWriter w) {
        w.Write(Text);
    }
}
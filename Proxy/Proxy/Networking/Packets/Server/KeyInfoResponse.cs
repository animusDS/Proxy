using Proxy.Networking.Packets.DataObjects.Data;

namespace Proxy.Networking.Packets.Server;

public class KeyInfoResponse : Packet {
    public string Name;
    public string Description;
    public string Creator;
    
    public override PacketType Type => PacketType.KeyInfoResponse;
    
    protected override void Read(PacketReader r) {
        Name = r.ReadString();
        Description = r.ReadString();
        Creator = r.ReadString();
    }
    
    protected internal override void Write(PacketWriter w) {
        w.Write(Name);
        w.Write(Description);
        w.Write(Creator);
    }
    
    public override string ToString() {
        return $"Name: {Name}," +
               $" Description: {Description}," +
               $" Creator: {Creator}";
    }
}
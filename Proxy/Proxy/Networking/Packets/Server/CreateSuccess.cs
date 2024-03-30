namespace Proxy.Networking.Packets.Server;

public class CreateSuccess : Packet {
    public int ObjectId;
    public int CharId;
    public string UnknownString;
    
    public override PacketType Type => PacketType.CreateSuccess;

    protected override void Read(PacketReader r) {
        ObjectId = r.ReadInt32();
        CharId = r.ReadInt32();
        UnknownString = r.ReadString();
    }

    protected internal override void Write(PacketWriter w) {
        w.Write(ObjectId);
        w.Write(CharId);
        w.Write(UnknownString);
    }
    
    public override string ToString() {
        return $"ObjectId: {ObjectId}," +
               $" CharId: {CharId}," +
               $" UnknownString: {UnknownString}";
    }
}
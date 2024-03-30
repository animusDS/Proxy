namespace Proxy.Networking.Packets.Server;

public class Reconnect : Packet {
    public string Name;
    public string Host;
    public ushort Port;
    public int GameId;
    public int KeyTime;
    public byte[] Key;
    
    public override PacketType Type => PacketType.Reconnect;

    protected override void Read(PacketReader r) {
        Name = r.ReadString();
        Host = r.ReadString();
        Port = r.ReadUInt16();
        GameId = r.ReadInt32();
        KeyTime = r.ReadInt32();
        Key = r.ReadBytes(r.ReadInt16());
    }

    protected internal override void Write(PacketWriter w) {
        w.Write(Name);
        w.Write(Host);
        w.Write(Port);
        w.Write(GameId);
        w.Write(KeyTime);
        w.Write((short) Key.Length);
        w.Write(Key);
    }
}
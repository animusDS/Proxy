namespace Proxy.Networking.Packets.Client;

public class Hello : Packet {
    public string BuildVersion;
    public int GameId;
    public string AccessToken;
    public int KeyTime;
    public byte[] Key;
    public string UserPlatform;
    public string PlayPlatform;
    public string PlatformToken;
    public string ClientToken;
    public string ClientHash;
    
    public override PacketType Type => PacketType.Hello;

    protected override void Read(PacketReader r) {
        GameId = r.ReadInt32();
        BuildVersion = r.ReadString();
        AccessToken = r.ReadString();
        KeyTime = r.ReadInt32();
        Key = r.ReadBytes(r.ReadInt16());
        UserPlatform = r.ReadString();
        PlayPlatform = r.ReadString();
        PlatformToken = r.ReadString();
        ClientToken = r.ReadString();
        ClientHash = r.ReadString();
    }

    protected internal override void Write(PacketWriter w) {
        w.Write(GameId);
        w.Write(BuildVersion);
        w.Write(AccessToken);
        w.Write(KeyTime);
        w.Write((short) Key.Length);
        w.Write(Key);
        w.Write(UserPlatform);
        w.Write(PlayPlatform);
        w.Write(PlatformToken);
        w.Write(ClientToken);
        w.Write(ClientHash);
    }
}
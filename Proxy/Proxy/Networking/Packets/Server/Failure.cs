namespace Proxy.Networking.Packets.Server;

public class Failure : Packet {
    public int ErrorId;
    public string ErrorMessage;

    public override PacketType Type => PacketType.Failure;

    protected override void Read(PacketReader r) {
        ErrorId = r.ReadInt32();
        ErrorMessage = r.ReadString();
    }

    protected internal override void Write(PacketWriter w) {
        w.Write(ErrorId);
        w.Write(ErrorMessage);
    }

    public override string ToString() {
        return $"ErrorId: {ErrorId}, Message: {ErrorMessage}";
    }
}
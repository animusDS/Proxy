namespace Proxy.Networking.Packets.DataObjects;

public interface IDataObject : ICloneable {
    void Read(PacketReader r);
    void Write(PacketWriter w);
}
namespace Proxy.Networking.Packets.DataObjects;

public interface IDataObject : ICloneable {
    IDataObject Read(PacketReader r);
    void Write(PacketWriter w);
}
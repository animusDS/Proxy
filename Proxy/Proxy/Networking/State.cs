namespace Proxy.Networking;

public class State {
    public byte[] ConRealKey = Array.Empty<byte>();
    public string ConTargetAddress = Proxy.DefaultHost;
    public ushort ConTargetPort = Proxy.GamePort;
    public readonly string Guid;

    public State(string guid) {
        Guid = guid;
    }
}
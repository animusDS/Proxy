using Proxy.Networking;
using Proxy.Networking.Packets;
using Proxy.Networking.Packets.Server;

namespace Proxy;

public static class Utils {
    public static void Delay(int ms, Action callback) {
        Task.Run(() => {
            Thread.Sleep(ms);
            callback();
        });
    }
}

public static class ClientUtils {
    public static void CreateTextNotification(this Client client, string name, string text) {
        var textPacket = (Text) Packet.Create(PacketType.Text);
        textPacket.Name = "#" + name;
        textPacket.ObjectId = -1;
        textPacket.DirtyText = text;
        client.SendToClient(textPacket);
    }
}
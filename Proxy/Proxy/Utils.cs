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

    public static void CreateDungeonNotification(this Client client, string message, ushort picture = 0x0704) {
        var notificationPacket = (Notification) Packet.Create(PacketType.Notification);
        notificationPacket.Message = message;
        notificationPacket.PictureType = picture;
        notificationPacket.Effect = (byte) Notification.NotificationType.DungeonOpened;
        client.SendToClient(notificationPacket);
    }
}
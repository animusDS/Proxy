using Proxy.Interface;
using Proxy.Networking;
using Proxy.Networking.Packets;

namespace Proxy.Plugins;

public class WelcomePopup : IPlugin {
    public void Initialize(Proxy proxy) {
        proxy.HookPacket(PacketType.CreateSuccess, OnCreateSuccess);
    }

    private static void OnCreateSuccess(Client client, Packet packet) {
        Utils.Delay(1000, () =>
            client.CreateDungeonNotification("Connected to Proxy!"));
    }
}
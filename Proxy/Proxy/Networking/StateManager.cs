using Proxy.Networking.Packets.DataObjects.Data;
using Proxy.Networking.Packets.Server;

namespace Proxy.Networking;

public class StateManager {
    private Proxy _proxy;

    public void Attach(Proxy proxy) {
        _proxy = proxy;

        _proxy.HookPacket<CreateSuccess>(OnCreateSuccess);
        proxy.HookPacket<Update>(OnUpdate);
    }

    private void OnCreateSuccess(Client client, CreateSuccess packet) {
        client.PlayerData = new PlayerData(packet.ObjectId);
    }

    private void OnUpdate(Client client, Update packet) {
        client.PlayerData.ParseUpdate(packet);
    }
}
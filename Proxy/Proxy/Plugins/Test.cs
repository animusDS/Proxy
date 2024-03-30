using Proxy.Interface;
using Proxy.Networking;
using Proxy.Networking.Packets;
using Proxy.Networking.Packets.Client;
using Proxy.Networking.Packets.DataObjects.Data;
using Proxy.Networking.Packets.DataObjects.Location;
using Proxy.Networking.Packets.Server;

namespace Proxy.Plugins;

public class Test : IPlugin {
    private static readonly Logger Log = new(typeof(Test));
    
    public void Initialize(Proxy proxy) {
        proxy.HookCommand("test", OnTestCommand);
        proxy.HookPacket(PacketType.PlayerShoot, OnPlayerShoot);
        proxy.HookPacket(PacketType.UseItem, OnUseItem);
        proxy.HookPacket(PacketType.Update, OnUpdate);
        proxy.HookPacket(PacketType.NewTick, OnNewTick);
        proxy.HookPacket(PacketType.InvResult, OnInvResult);
    }

    private static void OnTestCommand(Client client, string cmd, string[] args) {
    }
    
    private static void OnPlayerShoot(Client client, Packet packet) {
        var playerShoot = (PlayerShoot) packet;
    }
    
    private static void OnUseItem(Client client, Packet packet) {
        var useItem = (UseItem) packet;
        

    }
    
    private static void OnInvResult(Client client, Packet packet) {
        var invResult = (InvResult) packet;
    }
    
    private static void OnUpdate(Client client, Packet packet) {
        var update = (Update) packet;
        foreach (var newObject in update.NewObjects) {
            if (newObject.Status.ObjectId != client.PlayerData.ObjectId) {
                continue;
            }

            foreach (var statData in newObject.Status.Data) {
                
            }
        }
    }
    
    private static void OnNewTick(Client client, Packet packet) {
        var newTick = (NewTick) packet;
        foreach (var status in newTick.Statuses) {
            if (status.ObjectId != client.PlayerData.ObjectId) {
                continue;
            }

            foreach (var statData in status.Data) {
                
            }
        }
    }
}
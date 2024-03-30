using Proxy.Interface;
using Proxy.Networking;
using Proxy.Networking.Packets;
using Proxy.Networking.Packets.DataObjects.Stats;
using Proxy.Networking.Packets.Server;

namespace Proxy.Plugins;

public class Glow : IPlugin {
    public void Initialize(Proxy proxy) {
        proxy.HookCommand("glow", OnGlowCommand);
        proxy.HookPacket(PacketType.Update, OnUpdate);
        proxy.HookPacket(PacketType.NewTick, OnNewTick);
    }

    private static void OnGlowCommand(Client client, string cmd, string[] args) {
        var settings = client.Proxy.Settings.GetPluginSettings("Glow");
        settings.Enabled = !settings.Enabled;
        client.Proxy.Settings.SetPluginSettings("Glow", settings, true);
        client.CreateTextNotification("Glow", settings.Enabled ? "Glow enabled" : "Glow disabled");
    }

    private static void OnUpdate(Client client, Packet packet) {
        var settings = client.Proxy.Settings.GetPluginSettings("Glow");
        var update = (Update) packet;
        foreach (var objData in update.NewObjects) {
            if (objData.Status.ObjectId != client.PlayerData.ObjectId) {
                continue;
            }

            foreach (var statData in objData.Status.Data) {
                if (statData.Id != StatData.StatType.Supporter) {
                    continue;
                }

                statData.IntValue = settings.Enabled ? 1 : 0;
                break;
            }

            break;
        }
    }
    
    private static void OnNewTick(Client client, Packet packet) {
        var settings = client.Proxy.Settings.GetPluginSettings("Glow");
        var newTick = (NewTick) packet;
        var status = new Status {
            ObjectId = client.PlayerData.ObjectId,
            Data = [
                new StatData {
                    Id = StatData.StatType.Supporter,
                    IntValue = settings.Enabled ? 1 : 0,
                },
            ],
        };
        
        Array.Resize(ref newTick.Statuses, newTick.Statuses.Length + 1);
        newTick.Statuses[^1] = status;
    }
}
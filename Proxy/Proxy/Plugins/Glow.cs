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
    }

    private static void OnGlowCommand(Client client, string cmd, string[] args) {
        var settings = client.Proxy.Settings.GetPluginSettings("Glow");
        settings.Enabled = !settings.Enabled;
        client.Proxy.Settings.SetPluginSettings("Glow", settings, true);
        client.CreateTextNotification("Glow", settings.Enabled ? "Glow enabled" : "Glow disabled");
    }

    private static void OnUpdate(Client client, Packet packet) {
        var settings = client.Proxy.Settings.GetPluginSettings("Glow");
        if (!settings.Enabled)
            return;

        var update = (Update) packet;
        foreach (var objData in update.NewObjects) {
            if (objData.Status.ObjectId != client.PlayerData.ObjectId)
                continue;

            foreach (var statData in objData.Status.Data) {
                if (statData.Id != StatData.StatType.Supporter)
                    continue;

                statData.IntValue = 1;
                break;
            }

            break;
        }
    }
}
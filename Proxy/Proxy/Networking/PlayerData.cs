using Proxy.Networking.Packets.DataObjects.Stats;
using Proxy.Networking.Packets.Server;

namespace Proxy.Networking;

public class PlayerData {
    public readonly int ObjectId;

    public string AccountId;
    public int CharId;

    public Classes Class;

    public PlayerData(int objectId) {
        ObjectId = objectId;
    }

    public void ParseUpdate(Update update) {
        foreach (var newObject in update.NewObjects)
            if (newObject.Status.ObjectId == ObjectId) {
                Class = (Classes) newObject.ObjectType;

                foreach (var data in newObject.Status.Data) {
                    ParseStatData(data.Id, data.IntValue, data.StringValue);
                }
            }
    }

    private void ParseStatData(StatData.StatType statType, int intValue, string stringValue) {
        switch (statType) {
            case StatData.StatType.AccountId:
                AccountId = stringValue;
                break;
        }
    }
}
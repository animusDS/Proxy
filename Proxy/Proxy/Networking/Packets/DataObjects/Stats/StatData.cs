using Proxy.Networking.Packets.DataObjects.Data;

namespace Proxy.Networking.Packets.DataObjects.Stats;

public class StatData : IDataObject {
    public StatType Id;
    public int IntValue;
    public int SecondaryValue;
    public string StringValue;

    public StatData() {
    }

    public StatData(PacketReader r) => Read(r);

    public IDataObject Read(PacketReader r) {
        Id = (StatType) r.ReadByte();

        if (IsStringData()) {
            StringValue = r.ReadString();
        }
        else {
            IntValue = CompressedInt.Read(r);
        }

        SecondaryValue = CompressedInt.Read(r);

        return this;
    }

    public void Write(PacketWriter w) {
        w.Write((byte) Id);

        if (IsStringData()) {
            w.Write(StringValue);
        }
        else {
            CompressedInt.Write(w, IntValue);
        }

        CompressedInt.Write(w, SecondaryValue);
    }

    public object Clone() {
        return new StatData {
            Id = Id,
            IntValue = IntValue,
            StringValue = StringValue,
            SecondaryValue = SecondaryValue,
        };
    }

    private bool IsStringData() {
        return IsUtf(Id);
    }

    public override string ToString() {
        return "{ Id=" +
               Id +
               " Value=" +
               (IsStringData() ? StringValue : IntValue.ToString()) +
               " SecondaryValue=" +
               SecondaryValue +
               " }";
    }

    public static bool IsUtf(StatType type) {
        switch (type) {
            case StatType.Experience:
            case StatType.Name:
            case StatType.AccountId:
            case StatType.GuildName:
            case StatType.Skin:
            case StatType.PetName:
            case StatType.GraveAccountId:
            case StatType.DungeonModifiers:
            case StatType.Unknown123:
            case StatType.Unknown127:
                return true;
            default:
                return false;
        }
    }

    public enum StatType : byte {
        Experience = 6,
        Name = 31,
        AccountId = 38,
        GuildName = 62,
        Skin = 80,
        PetName = 82,
        Supporter = 99,
        GraveAccountId = 115,
        DungeonModifiers = 121,
        Unknown123 = 123,
        Unknown127 = 127,
    }
}
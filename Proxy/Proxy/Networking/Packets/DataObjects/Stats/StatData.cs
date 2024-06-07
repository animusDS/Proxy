namespace Proxy.Networking.Packets.DataObjects.Stats;

public class StatData : IDataObject {
    public StatType Id;
    public int IntValue;
    public int SecondaryValue;
    public string StringValue;

    public void Read(PacketReader r) {
        Id = (StatType) r.ReadByte();

        if (IsStringData()) {
            StringValue = r.ReadString();
        }
        else {
            IntValue = CompressedInt.Read(r);
        }

        SecondaryValue = CompressedInt.Read(r);
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

    public bool IsStringData() {
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
        return type switch {
            StatType.Experience or
                StatType.Name or
                StatType.AccountId or
                StatType.OwnerAccountId or
                StatType.GuildName or
                StatType.DustAmount or
                StatType.DustLimit or
                StatType.Skin or
                StatType.PetName or
                StatType.GraveAccountId or
                StatType.DungeonModifiers or
                StatType.Unknown127 or
                StatType.Unknown128 or
                StatType.Unknown147 => true,
            _ => false,
        };
    }

    public enum StatType : byte {
        Size = 2,
        Mp = 4,
        Experience = 6,
        Name = 31,
        MerchandiseType = 34,
        AccountId = 38,
        ObjectConnection = 41,
        MerchandiseRemainingCount = 42,
        MerchandiseRemainingMinutes = 43,
        MerchandiseRankRequired = 45,
        OwnerAccountId = 54,
        GuildName = 62,
        DustAmount = 71,
        DustLimit = 72,
        Skin = 80,
        PetName = 82,
        Supporter = 99,
        GraveAccountId = 115,
        DungeonModifiers = 121,
        Unknown127 = 127,
        Unknown128 = 128,
        Unknown147 = 147,
    }
}
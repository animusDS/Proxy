namespace Proxy.Networking.Packets.Server;

public class Notification : Packet {
    // ReSharper disable MemberCanBePrivate.Global

    public int Color;
    public byte Effect;
    public byte Extra;

    public string Message = "";
    public int ObjectId;
    public int PictureType;
    public int QueuePosition;
    public int UiExtra;

    public int Unknown32;
    public byte UnknownShort;

    public int EmoteId;

    // ReSharper restore once MemberCanBePrivate.Global

    public override PacketType Type => PacketType.Notification;

    protected override void Read(PacketReader r) {
        Effect = r.ReadByte();
        Extra = r.ReadByte();

        var notificationType = (NotificationType) Effect;
        switch (notificationType) {
            case NotificationType.StatIncrease:
                Message = r.ReadString();
                break;
            case NotificationType.ServerMessage:
                Message = r.ReadString();
                break;
            case NotificationType.ErrorMessage:
                Message = r.ReadString();
                break;
            case NotificationType.Ui:
                UiExtra = r.ReadInt16();
                Message = r.ReadString();
                break;
            case NotificationType.Queue:
                Message = r.ReadString();
                QueuePosition = r.ReadInt32();
                break;
            case NotificationType.ObjectText:
                ObjectId = r.ReadInt32();
                Message = r.ReadString();
                // Color = r.ReadInt32();
                break;
            case NotificationType.Death:
                Message = r.ReadString();
                PictureType = r.ReadInt32();
                break;
            case NotificationType.DungeonOpened:
                Message = r.ReadString();
                PictureType = r.ReadInt32();
                break;
            case NotificationType.DungeonCall:
                ObjectId = r.ReadInt32();
                Unknown32 = r.ReadInt32();
                UnknownShort = r.ReadByte();
                break;
            case NotificationType.Emote:
                ObjectId = r.ReadInt32();
                EmoteId = r.ReadInt32();
                break;
        }
    }

    protected internal override void Write(PacketWriter w) {
        w.Write(Effect);
        w.Write(Extra);

        var notificationType = (NotificationType) Effect;
        switch (notificationType) {
            case NotificationType.StatIncrease:
                w.Write(Message);
                break;
            case NotificationType.ServerMessage:
                w.Write(Message);
                break;
            case NotificationType.ErrorMessage:
                w.Write(Message);
                break;
            case NotificationType.Ui:
                w.Write(UiExtra);
                w.Write(Message);
                break;
            case NotificationType.Queue:
                w.Write(ObjectId);
                w.Write(Message);
                w.Write(QueuePosition);
                break;
            case NotificationType.ObjectText:
                w.Write(ObjectId);
                w.Write(Message);
                //w.Write(Color);
                break;
            case NotificationType.Death:
                w.Write(Message);
                w.Write(PictureType);
                break;
            case NotificationType.DungeonOpened:
                w.Write(Message);
                w.Write(PictureType);
                break;
            case NotificationType.DungeonCall:
                w.Write(ObjectId);
                w.Write(Unknown32);
                w.Write(UnknownShort);
                break;
            case NotificationType.Emote:
                w.Write(ObjectId);
                w.Write(EmoteId);
                break;
        }
    }
    
    public enum NotificationType {
        StatIncrease = 1,
        ServerMessage = 2,
        ErrorMessage = 3,
        Ui = 4,
        Queue = 5,
        ObjectText = 6,
        Death = 7,
        DungeonOpened = 8,
        DungeonCall = 10,
        Emote = 13,
    }
}
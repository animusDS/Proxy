using System.IO.Compression;
using Newtonsoft.Json;
using Proxy.Interface;
using Proxy.Networking;
using Proxy.Networking.Packets;
using Proxy.Networking.Packets.DataObjects.Data;
using Proxy.Networking.Packets.DataObjects.Stats;
using Proxy.Networking.Packets.Server;


namespace Proxy.Plugins;

public class MapRipper : IPlugin {
    private static readonly Logger Log = new(typeof(MapRipper));

    private static JsonMap _map;

    public void Initialize(Proxy proxy) {
        proxy.HookCommand("saveMap", OnSaveMapCommand);
        proxy.HookPacket(PacketType.MapInfo, OnMapInfo);
        proxy.HookPacket(PacketType.Update, OnUpdate);
    }

    private static void OnMapInfo(Client client, Packet packet) {
        var mapInfo = (MapInfo) packet;
        _map = new JsonMap(mapInfo.Name, mapInfo.Width, mapInfo.Height);
        
        client.CreateTextNotification("MapRipper", $"Collecting map data for {mapInfo.Name}");
    }
    
    private static void OnUpdate(Client client, Packet packet) {
        if (_map == null) {
            return;
        }

        var update = (Update) packet;
        _map.Update(update);
    }

    private static void OnSaveMapCommand(Client client, string cmd, string[] args) {
        if (args.Length > 0) {
            client.CreateTextNotification("MapRipper", "Usage: /saveMap");
            return;
        }

        if (_map == null) {
            client.CreateTextNotification("MapRipper", "There's no map data available!");
            return;
        }
        
        var json = _map.ToJson(client);
        var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), $"{_map.Name}.jm");
        File.WriteAllText(path, json);
        
        client.CreateTextNotification("MapRipper", $"Map data saved to: {path}");
    }

    private class JsonMap {
        public readonly string Name;
        
        public readonly int Width;
        public readonly int Height;

        public readonly int[][] Tiles;
        public readonly ObjectData[][][] Entities;
        
        public JsonMap(string name, int width, int height) {
            Name = name;
            
            Width = width;
            Height = height;

            Tiles = new int[width][];
            for (var i = 0; i < width; i++) {
                Tiles[i] = new int[height];

                for (var j = 0; j < height; j++) {
                    Tiles[i][j] = -1;
                }
            }

            Entities = new ObjectData[width][][];
            for (var i = 0; i < width; i++) {
                Entities[i] = new ObjectData[height][];
                for (var j = 0; j < height; j++) {
                    Entities[i][j] = [];
                }
            }
        }


#pragma warning disable CS8981 // The type name only contains lower-cased ascii characters. Such names may become reserved for the language.
#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value
        // ReSharper disable InconsistentNaming
        private struct obj {
            public string name;
            public string id;
        }

        private struct loc {
            public string ground;
            public obj[] objs;
            public obj[] regions;
        }

        private struct json_dat {
            public byte[] data;
            public int width;
            public int height;
            public loc[] dict;
        }

        public void Update(Update update) {
            foreach (var tile in update.Tiles) {
                Tiles[tile.X][tile.Y] = tile.Type;
            }

            foreach (var obj in update.NewObjects) {
                if (obj.Clone() is not ObjectData clone) {
                    Log.Warn("Failed to clone object data");
                    continue;
                }

                var isExcluded = false;
                foreach (var data in clone.Status.Data) {
                    switch (data.Id) {
                        case StatData.StatType.AccountId:
                        case StatData.StatType.OwnerAccountId:
                            isExcluded = true;
                            break;
                    }
                }
                
                if (isExcluded) {
                    continue;
                }
                
                clone.Status.Position.X -= 0.5f;
                clone.Status.Position.Y -= 0.5f;

                var x = (int) clone.Status.Position.X;
                var y = (int) clone.Status.Position.Y;
                Array.Resize(ref Entities[x][y], Entities[x][y].Length + 1);

                var arr = Entities[x][y];
                arr[^1] = clone;
            }
        }
        
        public string ToJson(Client client) {
            var obj = new json_dat {
                width = Width,
                height = Height,
            };

            var locs = new List<loc>();
            var ms = new MemoryStream();
            using (PacketWriter wtr = new PacketWriter(ms))
                for (var y = 0; y < obj.height; y++)
                for (var x = 0; x < obj.width; x++) {
                    var loc = new loc {
                        ground = Tiles[x][y] != -1 ? GetTileId(client, (ushort) Tiles[x][y]) : null,
                        objs = new obj[Entities[x][y].Length],
                    };
                    
                    for (var i = 0; i < loc.objs.Length; i++) {
                        var en = Entities[x][y][i];
                        var vals = new Dictionary<StatData.StatType, object>();
                        foreach (var z in en.Status.Data) {
                            vals.Add(z.Id, z.IsStringData() ? z.StringValue : z.IntValue);
                        }

                        var s = "";
                        if (vals.TryGetValue(StatData.StatType.Name, out var val)) {
                            s += ";name:" + val;
                        }

                        if (vals.TryGetValue(StatData.StatType.Size, out val)) {
                            s += ";size:" + val;
                        }

                        if (vals.TryGetValue(StatData.StatType.ObjectConnection, out val)) {
                            s += ";conn:0x" + ((int) val).ToString("X8");
                        }

                        if (vals.TryGetValue(StatData.StatType.MerchandiseType, out val)) {
                            s += ";mtype:" + val;
                        }

                        if (vals.TryGetValue(StatData.StatType.MerchandiseRemainingCount, out val)) {
                            s += ";mcount:" + val;
                        }

                        if (vals.TryGetValue(StatData.StatType.MerchandiseRemainingMinutes, out val)) {
                            s += ";mtime:" + val;
                        }

                        if (vals.TryGetValue(StatData.StatType.MerchandiseRankRequired, out val)) {
                            s += ";nstar:" + val;
                        }

                        var o = new obj { id = GetEntityId(client, en.ObjectType),
                            name = s.Trim(';'),
                        };

                        loc.objs[i] = o;
                    }

                    var ix = -1;
                    for (var i = 0; i < locs.Count; i++) {
                        if (locs[i].ground != loc.ground) {
                            continue;
                        }

                        if (locs[i].objs == null) {
                            continue;
                        }

                        if (locs[i].objs != null) {
                            if (locs[i].objs.Length != loc.objs.Length) {
                                continue;
                            }

                            var b = false;
                            for (var j = 0; j < loc.objs.Length; j++)
                                if (locs[i].objs[j].id != loc.objs[j].id || locs[i].objs[j].name != loc.objs[j].name) {
                                    b = true;
                                    break;
                                }

                            if (b) {
                                continue;
                            }
                        }

                        ix = i;
                        break;
                    }

                    if (ix == -1) {
                        ix = locs.Count;
                        locs.Add(loc);
                    }

                    wtr.Write((short) ix);
                }

            obj.data = Zlib.Compress(ms.ToArray());
            obj.dict = locs.ToArray();

            var settings = new JsonSerializerSettings {
                NullValueHandling = NullValueHandling.Ignore,
            };

            return JsonConvert.SerializeObject(obj, settings);
        }
    }

    private static string GetEntityId(Client client, ushort type) {
        string ret;
        try {
            ret = client.Proxy.GameData.Tiles.TryGetValue(type, out var tile) ? tile.Name : client.Proxy.GameData.Objects[type].Name;
        }
        catch {
            Log.Warn($"Failed to get entity id for type {type}");
            ret = null;
        }
        
        return ret;
    }

    private static string GetTileId(Client client, ushort type) {
        string ret;
        try {
            ret = client.Proxy.GameData.Tiles[type].Name;
        }
        catch {
            Log.Warn($"Failed to get tile id for type {type}");
            ret = null;
        }
        
        return ret;
    }
}

public static class Zlib {
    private static uint ADLER32(IEnumerable<byte> data) {
        const uint modulo = 0xfff1;
        uint a = 1, b = 0;
        foreach (var t in data) {
            a = (a + t) % modulo;
            b = (b + a) % modulo;
        }

        return b << 16 | a;
    }

    public static byte[] Compress(byte[] buffer) {
        byte[] comp;
        using (var output = new MemoryStream()) {
            using (var deflate = new DeflateStream(output, CompressionMode.Compress))
                deflate.Write(buffer, 0, buffer.Length);

            comp = output.ToArray();
        }

        // Refer to http://www.ietf.org/rfc/rfc1950.txt for zlib format
        const byte cm = 8;
        const byte cInfo = 7;
        const byte cmf = cm | cInfo << 4;
        const byte flg = 0xDA;

        var result = new byte[comp.Length + 6];
        result[0] = cmf;
        result[1] = flg;
        Buffer.BlockCopy(comp, 0, result, 2, comp.Length);

        var checkSum = ADLER32(buffer);
        var index = result.Length - 4;
        result[index++] = (byte) (checkSum >> 24);
        result[index++] = (byte) (checkSum >> 16);
        result[index++] = (byte) (checkSum >> 8);
        result[index++] = (byte) (checkSum >> 0);
        return result;
    }

    public static byte[] Decompress(byte[] buffer) {
        // cbf to find the unobfuscated version of this
        var num1 = buffer.Length >= 6 ? buffer[0] : throw new ArgumentException("Invalid ZLIB buffer.");
        var num2 = buffer[1];
        var num3 = (byte) (num1 & 15U);
        var num4 = (byte) ((uint) num1 >> 4);
        if (num3 != 8) {
            throw new NotSupportedException("Invalid compression method.");
        }

        if (num4 != 7) {
            throw new NotSupportedException("Unsupported window size.");
        }

        if ((num2 & 32) != 0) {
            throw new NotSupportedException("Preset dictionary not supported.");
        }

        if (((num1 << 8) + num2) % 31 != 0) {
            throw new InvalidDataException("Invalid header checksum");
        }

        var memoryStream1 = new MemoryStream(buffer, 2, buffer.Length - 6);
        var memoryStream2 = new MemoryStream();
        using (var deflateStream = new DeflateStream(memoryStream1, CompressionMode.Decompress)) {
            deflateStream.CopyTo(memoryStream2);
        }

        var array = memoryStream2.ToArray();
        var num5 = buffer.Length - 4;
        var num6 = num5 + 1;
        var num7 = buffer[num5] << 24;
        var num8 = num6 + 1;
        var num9 = buffer[num6] << 16;
        var num10 = num7 | num9;
        var num11 = num8 + 1;
        var num12 = buffer[num8] << 8;
        var num13 = num10 | num12;
        int num15 = buffer[num11];
        if ((num13 | num15) != (int) ADLER32(array)) {
            throw new InvalidDataException("Invalid data checksum");
        }

        return array;
    }
}
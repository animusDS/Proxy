using Proxy.Resources.DataStructures;

namespace Proxy;

public class GameData {
    private static readonly Logger Log = new(typeof(GameData));

    public List<ServerStructure> Servers;

    public Dictionary<ushort, ObjectStructure> Objects;

    public Dictionary<ushort, ItemStructure> Items;

    public Dictionary<ushort, TileStructure> Tiles;

    public GameData() {
        Servers = ServerStructure.GetServers();
        Log.Info($"Loaded {Servers.Count} servers");

        Objects = ObjectStructure.GetObjects();
        Log.Info($"Loaded {Objects.Count} objects");

        Items = ItemStructure.GetItems();
        Log.Info($"Loaded {Items.Count} items");

        Tiles = TileStructure.GetTiles();
        Log.Info($"Loaded {Tiles.Count} tiles");
    }
}
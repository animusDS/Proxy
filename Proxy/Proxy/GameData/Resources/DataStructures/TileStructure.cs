using System.Xml.Linq;
using Proxy.Resources.Util;

namespace Proxy.Resources.DataStructures;

public struct TileStructure {
    public ushort Type;
    public string Name;

    public TileStructure(XElement obj) {
        Type = (ushort) obj.AttributeDefault("type", "0x0").ParseHex();
        Name = obj.AttributeDefault("id", "Unknown");
    }

    internal static Dictionary<ushort, TileStructure> GetTiles() {
        var path = Path.Combine(Directory.GetCurrentDirectory(), "GameData", "Resources", "Xml", "Tiles.xml");
        var tileMap = new Dictionary<ushort, TileStructure>();
        try {
            var xml = XDocument.Load(path);
            var root = xml.Root;
            if (root == null)
                throw new Exception("XML root is null.");

            foreach (var element in root.Elements("Ground")) {
                var obj = new TileStructure(element);
                tileMap.Add(obj.Type, obj);
            }
        }
        catch (Exception ex) {
            Logger.Error($"Failed to load tiles. {ex}");
        }

        return tileMap;
    }
}
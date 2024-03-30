using System.Xml.Linq;
using Proxy.Resources.Util;

namespace Proxy.Resources.DataStructures;

public struct ObjectStructure {
    public XElement Root;

    public ushort Type;

    public string Name;

    public ObjectStructure(XElement root, XElement obj) {
        Root = root;

        Type = (ushort) obj.AttributeDefault("type", "0x0").ParseHex();
        Name = obj.AttributeDefault("id", "Unknown");
    }

    internal static Dictionary<ushort, ObjectStructure> GetObjects() {
        var path = Path.Combine(Directory.GetCurrentDirectory(), "GameData", "Resources", "Xml", "Objects.xml");
        var objectMap = new Dictionary<ushort, ObjectStructure>();
        try {
            var xml = XDocument.Load(path);
            var root = xml.Root;
            if (root == null)
                throw new Exception("XML root is null.");

            foreach (var element in root.Elements("Object")) {
                if (element.Element("Item") != null)
                    continue;

                var obj = new ObjectStructure(root, element);
                objectMap.TryAdd(obj.Type, obj);
            }
        }
        catch (Exception ex) {
            Logger.Error($"Failed to load objects. {ex}");
        }

        return objectMap;
    }
}
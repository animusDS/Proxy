using System.Text;
using System.Xml;

namespace Proxy.Resources.Xml;

public static class XmlParser {
    private static readonly Logger Log = new(typeof(XmlParser));

    // <Summary>
    // Parses all XML files in the Raw folder and saves them to the Resources folder.
    // Load all XML files from Asset Studio into the Raw folder then run this method.
    // </Summary>
    public static void Parse() {
        // Substitute with the path to your Source Xml folder
        var xmlPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
            "Proxy", "Proxy", "GameData", "Resources", "Xml");

        Log.Info($"Xml path: {xmlPath}");

        var objectXmlPath = Path.Combine(xmlPath, "Objects.xml");
        Log.Info($"Object xml path: {objectXmlPath}");

        var tileXmlPath = Path.Combine(xmlPath, "Tiles.xml");
        Log.Info($"Tile xml path: {tileXmlPath}");

        var rawPath = Path.Combine(xmlPath, "Raw");
        Log.Info($"Raw path: {rawPath}");

        Log.Info("Parsing XML files...");
        var files = Directory.GetFiles(rawPath, "*.*", SearchOption.AllDirectories);
        Log.Info($"Found {files.Length} files");

        var objectXml = new XmlDocument();
        objectXml.LoadXml("<Objects></Objects>");

        var tileXml = new XmlDocument();
        tileXml.LoadXml("<GroundTypes></GroundTypes>");

        foreach (var file in files) {
            var fileContents = File.ReadAllText(file);
            if (!fileContents.Contains("<Objects>") && !fileContents.Contains("<GroundTypes>"))
                continue;

            var xml = new XmlDocument();
            xml.LoadXml(fileContents);

            var objects = xml.SelectNodes("/Objects/Object");
            if (objects != null)
                foreach (XmlNode node in objects) {
                    var xmlNode = node.CloneNode(true);
                    objectXml.DocumentElement?.AppendChild(objectXml.ImportNode(xmlNode, true));
                }

            var tiles = xml.SelectNodes("/GroundTypes/Ground");
            if (tiles != null)
                foreach (XmlNode node in tiles) {
                    var xmlNode = node.CloneNode(true);
                    tileXml.DocumentElement?.AppendChild(tileXml.ImportNode(xmlNode, true));
                }
        }

        Log.Info($"Object xml length: {objectXml.InnerXml.Length}");
        Log.Info($"Tile xml length: {tileXml.InnerXml.Length}");

        var settings = new XmlWriterSettings {
            Indent = true,
            IndentChars = "\t",
            NewLineChars = "\n",
            NewLineHandling = NewLineHandling.Replace,
            Encoding = Encoding.UTF8,
        };

        using var objectWriter = XmlWriter.Create(objectXmlPath, settings);
        objectXml.Save(objectWriter);

        using var tileWriter = XmlWriter.Create(tileXmlPath, settings);
        tileXml.Save(tileWriter);

        Log.Info($"Saved objects to {objectXmlPath}");
        Log.Info($"Saved tiles to {tileXmlPath}");

        Log.Info("Done!");
    }
}
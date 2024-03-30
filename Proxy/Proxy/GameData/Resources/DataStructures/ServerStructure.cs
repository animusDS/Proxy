using System.Xml.Linq;

namespace Proxy.Resources.DataStructures;

public struct ServerStructure {
    private static readonly Logger Log = new(typeof(ServerStructure));

    public string Name;
    public string Ip;
    public double Lat;
    public double Long;
    public double Usage;

    public static List<ServerStructure> GetServers() {
        var path = Path.Combine(Directory.GetCurrentDirectory(), "GameData", "Resources", "Xml", "Servers.xml");
        var servers = new List<ServerStructure>();
        try {
            var xml = XDocument.Load(path);
            var root = xml.Root;
            if (root == null)
                throw new Exception("XML root is null.");

            foreach (var element in root.Elements("Server")) {
                var name = element.Element("Name")!.Value;
                var ip = element.Element("DNS")!.Value;
                var lat = double.Parse(element.Element("Lat")!.Value);
                var @long = double.Parse(element.Element("Long")!.Value);
                var usage = double.Parse(element.Element("Usage")!.Value);
                servers.Add(new ServerStructure {
                    Name = name,
                    Ip = ip,
                    Lat = lat,
                    Long = @long,
                    Usage = usage,
                });
            }
        }
        catch (Exception ex) {
            Logger.Error($"Failed to load servers. {ex}");
        }

        return servers;
    }

    public static ServerStructure GetServer(List<ServerStructure> servers, string name) {
        if (servers == null || !servers.Any())
            return default;

        try {
            var bestMatch = GetBestMatch(name, servers.Select(s => s.Name));
            return string.IsNullOrEmpty(bestMatch) ? default : servers.First(s => s.Name == bestMatch);
        }
        catch (Exception e) {
            Log.Error($"Failed to get server ({name}). {e}");
            return default;
        }
    }

    private static string GetBestMatch(string input, IEnumerable<string> options) {
        switch (input) {
            #region Hardcoded cases

            case "useast2":
            case "use2":
                return "USEast2";
            case "eueast":
            case "eue":
                return "EUEast";
            case "eusouthwest":
            case "eusw":
                return "EUSouthWest";
            case "eunorth":
            case "eun":
                return "EUNorth";
            case "useast":
            case "use":
                return "USEast";
            case "uswest4":
            case "usw4":
                return "USWest4";
            case "euwest2":
            case "euw2":
                return "EUWest2";
            case "asia":
            case "as":
                return "Asia";
            case "ussouth3":
            case "uss3":
                return "USSouth3";
            case "euwest":
            case "euw":
                return "EUWest";
            case "uswest":
            case "usw":
                return "USWest";
            case "usmidwest2":
            case "usmw2":
                return "USMidWest2";
            case "usmidwest":
            case "usmw":
                return "USMidWest";
            case "ussouth":
            case "uss":
                return "USSouth";
            case "uswest3":
            case "usw3":
                return "USWest3";
            case "ussouthwest":
            case "ussw":
                return "USSouthWest";
            case "usnorthwest":
            case "usnw":
                return "USNorthWest";
            case "australia":
            case "aus":
                return "Australia";

            #endregion

            default:
                var bestMatch = FuzzySharp.Process.ExtractOne(input, options);
                return bestMatch.Value;
        }
    }
}
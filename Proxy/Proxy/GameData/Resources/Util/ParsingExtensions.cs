using System.Xml.Linq;

namespace Proxy.Resources.Util;

public static class ParsingExtensions {
    public static string AttributeDefault(this XElement element, XName name, string @default) {
        return element.Attributes(name).Any() ? element.Attribute(name)?.Value : @default;
    }
    
    public static string ElementDefault(this XElement element, XName name, string @default) {
        return element.Elements(name).Any() ? element.Element(name)?.Value : @default;
    }

    public static int ParseHex(this string input) {
        return Convert.ToInt32(input, 16);
    }
    
    public static int ParseInt(this string input) {
        return int.Parse(input);
    }
}
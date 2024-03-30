using System.Xml.Linq;

namespace Proxy.Resources.Util;

public static class ParsingExtensions {
    public static string AttributeDefault(this XElement element, XName name, string @default) {
        return element.Attributes(name).Any() ? element.Attribute(name)?.Value : @default;
    }

    public static int ParseHex(this string input) {
        return Convert.ToInt32(input, 16);
    }
}
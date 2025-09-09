using System.Xml.Linq;

namespace Manpuku.Edinet.Manifest;

internal static class ManifestNames
{
	public static readonly XNamespace ManifestNs = "http://disclosure.edinet-fsa.go.jp/2013/manifest";
	public static readonly XNamespace XmlNs = "http://www.w3.org/XML/1998/namespace";

	public static readonly XName TocComposition = ManifestNs + "tocComposition";
	public static readonly XName Title = ManifestNs + "title";
	public static readonly XName Item = ManifestNs + "item";
	public static readonly XName Insert = ManifestNs + "insert";
	public static readonly XName List = ManifestNs + "list";
	public static readonly XName Instance = ManifestNs + "instance";
	public static readonly XName Ixbrl = ManifestNs + "ixbrl";

	public static readonly XName Lang = XmlNs + "lang";
}
using System.Xml.Linq;

namespace Manpuku.Edinet.Manifest;

internal static class XElementExtensions
{
	public static XName? ResolveQName(this XElement xml, string? qName)
	{
		if (string.IsNullOrWhiteSpace(qName))
			return null;

		var parts = qName.Split(':');
		if (parts.Length == 2)
		{
			var prefix = parts[0];
			var localName = parts[1];
			var ns = xml.GetNamespaceOfPrefix(prefix);

			if (ns == null)
				return null; // 名前空間が見つからない場合は null を返す

			return XName.Get(localName, ns.NamespaceName);
		}
		else if (parts.Length > 2)
		{
			return null;
		}
		// プレフィックスなしの場合は、現在の要素の名前空間を使う
		return XName.Get(qName, xml.Name.NamespaceName);
	}
}

using System.Collections.Immutable;
using System.Xml.Linq;

namespace Manpuku.Edinet.Manifest;

/// <summary>
/// EDINETのマニフェストファイルを表す
/// </summary>
public class Manifest
{
	/// <summary>
	/// manifest/tocComposition要素
	/// </summary>
	public TocComposition TocComposition { get; init; }

	/// <summary>
	/// manifest/list要素
	/// </summary>
	public ImmutableArray<Instance> List { get; init; }

	/// <summary>
	/// 指定された <see cref="XDocument"/> から EDINET マニフェストの構造を解析し、
	/// <see cref="TocComposition"/> および <see cref="List"/> を初期化します。
	/// 
	/// このコンストラクタは、構造的に意味のある XML ドキュメントであることを前提とし、
	/// 必須要素が欠落している場合は <see cref="ManifestParseException"/> を投げます。
	/// </summary>
	/// <param name="document">EDINET マニフェストを表す XML ドキュメント</param>
	/// <exception cref="ManifestParseException">
	/// ルート要素が存在しない、または必須要素が欠落している等形式に誤りがある場合に発生します。
	/// </exception>
	public Manifest(XDocument document)
	{
		if (document?.Root == null)
			throw new ManifestParseException("XMLドキュメントにルート要素が存在しません。");

		var _toc = document.Root.Descendants(ManifestNames.TocComposition).FirstOrDefault();
		if (_toc == null)
		{
			throw new ManifestParseException("必須要素 <tocComposition> が存在しません。");
		}

		TocComposition = new TocComposition(_toc);
		List = document.Root
				.Elements(ManifestNames.List)
				.SelectMany(list => list.Elements(ManifestNames.Instance))
				.Select(instance => new Instance(instance))
				.ToImmutableArray();
	}
}

/// <summary>
/// tocComposition要素を表す
/// </summary>
public class TocComposition
{
	/// <summary>
	/// tocComposition要素下のtitle属性のリスト
	/// </summary>
	public ImmutableArray<Title> Titles { get; init; }

	/// <summary>
	/// tocComposition要素下のitem要素
	/// </summary>
	public Item Item { get; init; }

	internal TocComposition(XElement xml)
	{
		var titles = new List<Title>();
		foreach (var e in xml.Elements())
		{
			if (e.Name == ManifestNames.Title)
			{
				titles.Add(new Title(e));
				continue;
			}
			if (e.Name == ManifestNames.Item)
			{
				Item = new Item(e);
				continue;
			}
		}

		if (Item == null)
		{
			throw new ManifestParseException("必須要素 <item> が存在しません。");
		}
		Titles = [.. titles];
	}
}

/// <summary>
/// title要素を表す
/// </summary>
public class Title
{
	/// <summary>
	/// xml:lang属性の値
	/// </summary>
	public string Lang { get; init; }

	/// <summary>
	/// title要素の値
	/// </summary>
	public string Value { get; init; }

	internal Title(XElement xml)
	{
		var lang = xml.Attribute(ManifestNames.Lang);
		Lang = lang?.Value ?? string.Empty;
		Value = xml.Value ?? string.Empty;
	}
}

/// <summary>
/// item要素を表す
/// </summary>
public class Item
{
	/// <summary>
	/// ref属性の値
	/// </summary>
	public string Ref { get; init; }

	/// <summary>
	/// extrole属性の値
	/// </summary>
	public Uri? Extrole { get; init; }

	/// <summary>
	/// in属性の値
	/// </summary>
	public string In { get; init; }

	/// <summary>
	/// start属性の値
	/// </summary>
	public XName? Start { get; init; }

	/// <summary>
	/// item要素下のinsert要素
	/// </summary>
	public ImmutableArray<Insert>? Inserts { get; init; }

	internal Item(XElement xml)
	{
		Ref = xml.Attribute("ref")?.Value ?? string.Empty;
		var extrole = xml.Attribute("extrole")?.Value;
		if (extrole != null)
		{
			Extrole = new Uri(extrole);
		}
		In = xml.Attribute("in")?.Value ?? string.Empty;

		var _start = xml.Attribute("start")?.Value;
		if (_start != null)
		{
			Start = xml.ResolveQName(_start);
			if (Start == null)
			{
				throw new ManifestParseException($"start属性の値[{_start}]が不正です。");
			}
		}

		var _inserts = xml.Elements(ManifestNames.Insert);
		if (_inserts.Any())
		{
			Inserts = [.. _inserts.Select(e => new Insert(e))];
		}
	}
}

/// <summary>
/// insert要素を表す
/// </summary>
public class Insert
{
	/// <summary>
	/// parent属性の値
	/// </summary>
	public XName? Parent { get; init; }

	/// <summary>
	/// insert要素下のitem要素
	/// </summary>
	public Item Item { get; init; }

	internal Insert(XElement _xml)
	{
		var _parent = _xml.Attribute("parent")?.Value;
		if (_parent != null)
		{
			Parent = _xml.ResolveQName(_parent);
			if (Parent == null)
			{
				throw new ManifestParseException($"parent属性の値[{_parent}]が不正です。");
			}
		}

		var _item = _xml.Element(ManifestNames.Item) ?? throw new ManifestParseException("必須要素 <item> が存在しません。");
		Item = new Item(_item);
	}
}

/// <summary>
/// instance要素を表す
/// </summary>
public class Instance
{
	/// <summary>
	/// instance要素下のixbrl要素の値の配列
	/// </summary>
	public ImmutableArray<string> InlineXBRLFiles { get; init; }

	/// <summary>
	/// id属性の値
	/// </summary>
	public string Id { get; init; }

	/// <summary>
	/// type属性の値
	/// </summary>
	public string Type { get; init; }

	/// <summary>
	/// preferredFilename属性の値
	/// </summary>
	public string PreferredFilename { get; init; }

	internal Instance(XElement xml)
	{
		Id = xml.Attribute("id")?.Value ?? string.Empty;
		Type = xml.Attribute("type")?.Value ?? string.Empty;
		PreferredFilename = xml.Attribute("preferredFilename")?.Value ?? string.Empty;
		InlineXBRLFiles = [.. xml.Elements(ManifestNames.Ixbrl).Select(e => e.Value)];

		if (InlineXBRLFiles.Length == 0)
		{
			throw new ManifestParseException("必須要素 <ixbrl> が存在しません。");
		}
	}
}


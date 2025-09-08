using System.Xml.Linq;

namespace Manpuku.Edinet.Manifest.Tests;

[TestClass]
public sealed class ManifestTests
{
	[TestMethod]
	public void TestParseNormalFundPublic()
	{
		var m1 = @".\\manifests\\fund\\manifest_PublicDoc.xml";
		var d1 = XDocument.Load(m1);
		var manifest = new Manifest(d1);

		Assert.IsNotNull(manifest);

		Assert.AreEqual("ja", manifest.TocComposition.Titles[0].Lang);
		Assert.AreEqual("提出本文書", manifest.TocComposition.Titles[0].Value);
		Assert.AreEqual("en", manifest.TocComposition.Titles[1].Lang);
		Assert.AreEqual("Main Document", manifest.TocComposition.Titles[1].Value);

		Assert.AreEqual("jpsps070000", manifest.TocComposition.Item?.Ref);
		Assert.AreEqual("http://disclosure.edinet-fsa.go.jp/role/jpsps/rol_CabinetOfficeOrdinanceOnDisclosureOfInformationEtcOnSpecifiedSecuritiesFormNo7AnnualSecuritiesReport", manifest.TocComposition.Item.Extrole.AbsoluteUri);
		Assert.AreEqual("presentation", manifest.TocComposition.Item?.In);
		Assert.IsNull(manifest.TocComposition.Item?.Start);

		Assert.AreEqual(2, manifest.TocComposition.Item?.Inserts.Value.Length);
		Assert.AreEqual("{http://disclosure.edinet-fsa.go.jp/taxonomy/jpsps/2024-11-01/jpsps_cor}FinancialInformationOfFundHeading", manifest.TocComposition.Item?.Inserts.Value[0].Parent);
		Assert.AreEqual("jpsps070000_1", manifest.TocComposition.Item?.Inserts.Value[0].Item.Ref);

		Assert.AreEqual("http://disclosure.edinet-fsa.go.jp/role/jpsps/rol_CabinetOfficeOrdinanceOnDisclosureOfInformationEtcOnSpecifiedSecuritiesFormNo7AnnualSecuritiesReport", manifest.TocComposition.Item?.Inserts.Value[0].Item.Extrole.AbsoluteUri);
		Assert.AreEqual("{http://disclosure.edinet-fsa.go.jp/taxonomy/jpsps/2024-11-01/jpsps_cor}FinancialInformationOfFundHeading", manifest.TocComposition.Item?.Inserts.Value[0].Item.Start);
		Assert.AreEqual("presentation", manifest.TocComposition.Item?.Inserts.Value[0].Item.In);

		Assert.AreEqual(3, manifest.List.Length);
		Assert.AreEqual("jpsps070000", manifest.List[0].Id);
		Assert.AreEqual("jpsps070000_1", manifest.List[1].Id);
		Assert.AreEqual("jpsps070000_2", manifest.List[2].Id);

		Assert.AreEqual("PublicDoc", manifest.List[0].Type);
		Assert.AreEqual("jpsps070000-asr-001_G08837-000_2025-06-05_01_2025-09-05.xbrl", manifest.List[0].PreferredFilename);
		Assert.AreEqual(4, manifest.List[0].InlineXBRLFiles.Length);

		Assert.AreEqual("0000000_header_jpsps070000-asr-001_G08837-000_2025-06-05_01_2025-09-05_ixbrl.htm", manifest.List[0].InlineXBRLFiles[0]);
		Assert.AreEqual("0101010_honbun_jpsps070000-asr-001_G08837-000_2025-06-05_01_2025-09-05_ixbrl.htm", manifest.List[0].InlineXBRLFiles[1]);
		Assert.AreEqual("0103070_honbun_jpsps070000-asr-001_G08837-000_2025-06-05_01_2025-09-05_ixbrl.htm", manifest.List[0].InlineXBRLFiles[2]);
		Assert.AreEqual("0201010_honbun_jpsps070000-asr-001_G08837-000_2025-06-05_01_2025-09-05_ixbrl.htm", manifest.List[0].InlineXBRLFiles[3]);
	}


	[TestMethod]
	public void TestParseNormalFundAudit()
	{
		var m1 = @".\\manifests\\fund\\manifest_AuditDoc.xml";
		var d1 = XDocument.Load(m1);
		var manifest = new Manifest(d1);
		Assert.IsNotNull(manifest);
	}

	[TestMethod]
	public void TestParseNormalUfoPublic()
	{
		var m1 = @".\\manifests\\ufo\\manifest_PublicDoc.xml";
		var d1 = XDocument.Load(m1);
		var manifest = new Manifest(d1);
		Assert.IsNotNull(manifest);
	}

	[TestMethod]
	public void TestParseNormalUfoAudit()
	{
		var m1 = @".\\manifests\\ufo\\manifest_AuditDoc.xml";
		var d1 = XDocument.Load(m1);
		var manifest = new Manifest(d1);
		Assert.IsNotNull(manifest);
	}

	[TestMethod]
	public void TestParseNormalLvhPublic()
	{
		var m1 = @".\\manifests\\lvh\\manifest_PublicDoc.xml";
		var d1 = XDocument.Load(m1);
		var manifest = new Manifest(d1);
		Assert.IsNotNull(manifest);
	}

	[TestMethod]
	public void TestParseErrNoTocComposition()
	{
		var m1 = @".\\manifests\\err\\tocCompositionなし.xml";
		var d1 = XDocument.Load(m1);
		try
		{
			var manifest = new Manifest(d1);
			Assert.Fail("例外が発生しませんでした");
		}
		catch (ManifestParseException ex)
		{
			StringAssert.Contains(ex.Message, "<tocComposition>");
		}
	}

	[TestMethod]
	public void TestParseErrInvalidStart()
	{
		var m1 = @".\\manifests\\err\\start不正.xml";
		var d1 = XDocument.Load(m1);
		try
		{
			var manifest = new Manifest(d1);
			Assert.Fail("例外が発生しませんでした");
		}
		catch (ManifestParseException ex)
		{
			StringAssert.Contains(ex.Message, "start属性");
		}
	}

	[TestMethod]
	public void TestParseErrInvalidParent()
	{
		var m1 = @".\\manifests\\err\\parent不正.xml";
		var d1 = XDocument.Load(m1);
		try
		{
			var manifest = new Manifest(d1);
			Assert.Fail("例外が発生しませんでした");
		}
		catch (ManifestParseException ex)
		{
			StringAssert.Contains(ex.Message, "parent属性");
		}
	}

	[TestMethod]
	public void TestParseErrNoItem()
	{
		var m1 = @".\\manifests\\err\\itemなし.xml";
		var d1 = XDocument.Load(m1);
		try
		{
			var manifest = new Manifest(d1);
			Assert.Fail("例外が発生しませんでした");
		}
		catch (ManifestParseException ex)
		{
			StringAssert.Contains(ex.Message, "<item>");
		}
	}


	[TestMethod]
	public void TestParseErrNoIxbrl()
	{
		var m1 = @".\\manifests\\err\\ixbrlなし.xml";
		var d1 = XDocument.Load(m1);
		try
		{
			var manifest = new Manifest(d1);
			Assert.Fail("例外が発生しませんでした");
		}
		catch (ManifestParseException ex)
		{
			StringAssert.Contains(ex.Message, "<ixbrl>");
		}
	}

	[TestMethod]
	public void TestParseErrNoItemOfTocComposition()
	{
		var m1 = @".\\manifests\\err\\tocComposition-itemなし.xml";
		var d1 = XDocument.Load(m1);
		try
		{
			var manifest = new Manifest(d1);
			Assert.Fail("例外が発生しませんでした");
		}
		catch (ManifestParseException ex)
		{
			StringAssert.Contains(ex.Message, "<item>");
		}
	}
}

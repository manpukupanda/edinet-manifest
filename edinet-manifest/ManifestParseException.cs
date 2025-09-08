namespace Manpuku.Edinet.Manifest;

/// <summary>
/// マニフェストパース時に発生した例外
/// </summary>
public class ManifestParseException : Exception
{
	public ManifestParseException(string message) : base(message) { }
}
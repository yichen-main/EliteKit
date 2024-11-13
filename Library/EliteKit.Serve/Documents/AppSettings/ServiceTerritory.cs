namespace EliteKit.Serve.Documents.AppSettings;
public sealed class ServiceTerritory
{
    public required TextSeq Seq { get; init; }
    public readonly struct TextSeq
    {
        public required string Endpoint { get; init; }
        public required string ApiKey { get; init; }
    }
}
namespace EliteKit.Infrastructure.Core.Contracts.TableInformations;
public sealed class QueryIntervalFilter
{
    public required DateTime StartTime { get; init; }
    public required DateTime EndTime { get; init; }
}
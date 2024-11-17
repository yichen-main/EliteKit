namespace EliteKit.Infrastructure.Core.Attributes.ProfileInformations;

[AttributeUsage(AttributeTargets.Property)]
public sealed class RowInfoAttribute : Attribute
{
    public bool ForeignKey { get; init; }
    public bool UniqueIndex { get; init; }
}
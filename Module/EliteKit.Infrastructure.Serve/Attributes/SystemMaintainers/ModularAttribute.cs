namespace EliteKit.Infrastructure.Serve.Attributes.SystemMaintainers;

[AttributeUsage(AttributeTargets.Class)]
public sealed class ModularAttribute(string name = nameof(AsyncCallback)) : Attribute
{
    public string Name { get; init; } = name;
}
namespace EliteKit.Infrastructure.Core.Attributes.DependentModules;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class DependsOnAttribute(params Type[] dependencies) : Attribute
{
    public Type[] Dependencies { get; } = dependencies;
}
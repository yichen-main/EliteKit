namespace EliteKit.Infrastructure.Core.Attributes.DependentModules;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class DependentAttribute(ServiceLifetime serviceLifetime) : Attribute
{
    public ServiceLifetime ServiceLifetime { get; } = serviceLifetime;
}
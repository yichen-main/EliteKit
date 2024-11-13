namespace EliteKit.Serve.Attributes.SystemMaintainers;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class DependentAttribute(ServiceLifetime serviceLifetime) : Attribute
{
    public ServiceLifetime ServiceLifetime { get; } = serviceLifetime;
}
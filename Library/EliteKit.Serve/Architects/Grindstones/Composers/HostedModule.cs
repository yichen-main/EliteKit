namespace EliteKit.Serve.Architects.Grindstones.Composers;
public abstract class HostedModule<T> : ModuleBase where T : ModuleBase, IModularization
{
    protected void Execute(in ContainerBuilder builder)
    {
        LocalType = Initialize(this, builder);
        Lazy<ServiceCollection> services = new(() =>
        {
            ServiceCollection result = new();
            foreach (var validatorType in typeof(IValidator<>).GetSubInterfaces<T>())
            {
                var interfaceType = validatorType.GetInterfaces().First(x =>
                x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IValidator<>));
                result.AddTransient(interfaceType, validatorType);
            }
            foreach (var eventType in typeof(IEventHandler<>).GetSubInterfaces<T>())
            {
                var interfaceType = eventType.GetInterfaces().First(x =>
                x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IEventHandler<>));
                result.AddTransient(interfaceType, eventType);
            }
            result.AddFastEndpoints().AddResponseCaching().AddMemoryCache();
            result.AddMediatR(x => x.RegisterServicesFromAssemblyContaining<T>());
            return result;
        });
        foreach (var module in LocalType.Assembly.GetTypes().Where(x => typeof(IModularization).IsAssignableFrom(x) && !x.IsAbstract))
        {
            if (module is not null) (Activator.CreateInstance(module) as IModularization)?.ConfigureServices(services.Value);
        }
        builder.Populate(services.Value);
    }
    protected Type LocalType { get; private set; } = null!;
}
namespace EliteKit.Serve.Abstracts.ModuleServices;
public abstract class BaseModule<T> : HostedModule<T>, IModularization where T : ModuleBase, IModularization
{
    protected override void Load(ContainerBuilder builder) => Execute(builder);
    public abstract void ConfigureServices(IServiceCollection services);
}
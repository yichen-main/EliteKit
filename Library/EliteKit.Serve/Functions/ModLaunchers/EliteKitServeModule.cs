namespace EliteKit.Serve.Functions.ModLaunchers;
public sealed class EliteKitServeModule : ModuleBase
{
    protected override void Load(ContainerBuilder builder)
    {
        Initialize(this, builder);
        builder.RegisterBuildCallback(x =>
        {
            var configuration = x.Resolve<IConfiguration>();
        });
    }
}
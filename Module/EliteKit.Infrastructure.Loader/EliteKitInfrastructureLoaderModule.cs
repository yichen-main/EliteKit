using EliteKit.Infrastructure.Core;
using EliteKit.Infrastructure.Core.Abstracts.BottomStructures;

namespace EliteKit.Infrastructure.Loader;

[DependsOn(typeof(EliteKitInfrastructureCoreModule))]
public sealed class EliteKitInfrastructureLoaderModule : ModuleBase<EliteKitInfrastructureLoaderModule>
{
    public override void ConfigureServices(IServiceCollection services)
    {

    }
}
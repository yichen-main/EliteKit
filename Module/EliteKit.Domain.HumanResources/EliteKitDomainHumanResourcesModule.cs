using EliteKit.Serve.Abstracts.ModuleServices;
using EliteKit.Serve.Attributes.SystemMaintainers;
using EliteKit.Serve.Functions.ModLaunchers;
using Microsoft.Extensions.DependencyInjection;

namespace EliteKit.Domain.HumanResources;

[DependsOn(typeof(EliteKitServeModule))]
public sealed class EliteKitDomainHumanResourcesModule : BaseModule<EliteKitDomainHumanResourcesModule>
{
    public override void ConfigureServices(IServiceCollection services)
    {
        
    }
}
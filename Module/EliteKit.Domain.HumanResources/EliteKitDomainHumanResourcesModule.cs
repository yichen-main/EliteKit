using EliteKit.Infrastructure.Core;
using EliteKit.Infrastructure.Core.Abstracts.BottomStructures;
using EliteKit.Infrastructure.Core.Attributes.DependentModules;
using Microsoft.Extensions.DependencyInjection;

namespace EliteKit.Domain.HumanResources;

[DependsOn(typeof(EliteKitInfrastructureCoreModule))]
public sealed class EliteKitDomainHumanResourcesModule : ModuleBase<EliteKitDomainHumanResourcesModule>
{
    public override void ConfigureServices(IServiceCollection services)
    {

    }
}
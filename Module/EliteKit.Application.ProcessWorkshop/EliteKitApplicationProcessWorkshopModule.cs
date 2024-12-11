using EliteKit.Domain.FacilityManagement;
using EliteKit.Domain.HumanResources;
using EliteKit.Infrastructure.Core.Abstracts.BottomStructures;
using EliteKit.Infrastructure.Core.Attributes.DependentModules;
using FastEndpoints;
using Microsoft.Extensions.DependencyInjection;

namespace EliteKit.Application.ProcessWorkshop;

[DependsOn(
    typeof(EliteKitDomainFacilityManagementModule),
    typeof(EliteKitDomainHumanResourcesModule))]
public sealed class EliteKitApplicationProcessWorkshopModule : ModuleBase<EliteKitApplicationProcessWorkshopModule>
{
    public override void ConfigureServices(IServiceCollection services)
    {
        services.AddFastEndpoints();
    }
}
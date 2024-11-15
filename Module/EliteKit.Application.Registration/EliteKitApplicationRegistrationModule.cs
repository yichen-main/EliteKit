using EliteKit.Domain.HumanResources;
using EliteKit.Infrastructure.Core.Abstracts.BottomStructures;
using EliteKit.Infrastructure.Core.Attributes.DependentModules;
using FastEndpoints;
using Microsoft.Extensions.DependencyInjection;

namespace EliteKit.Application.Registration;

[DependsOn(typeof(EliteKitDomainHumanResourcesModule))]
public sealed class EliteKitApplicationRegistrationModule : ModuleBase<EliteKitApplicationRegistrationModule>
{
    public override void ConfigureServices(IServiceCollection services)
    {
        services.AddFastEndpoints();
    }
}
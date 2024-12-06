using EliteKit.Application.Registration;
using EliteKit.Infrastructure.Core.Attributes.DependentModules;
using EliteKit.Infrastructure.Serve;
using EliteKit.Infrastructure.Serve.Abstracts.StructureBuilders;
using EliteKit.Infrastructure.Serve.Attributes.SystemMaintainers;
using EliteKit.Vehicle.Blazor.Components;
using FastEndpoints;

namespace EliteKit.Vehicle.Blazor;

[Modular, DependsOn(
    typeof(EliteKitInfrastructureServeModule),
    typeof(EliteKitApplicationRegistrationModule))]
public sealed class ModuleFactory : BuildServerFactory<ModuleFactory>
{
    public override void ConfigureServices(IServiceCollection services)
    {
        services.AddRazorComponents().AddInteractiveServerComponents();
    }
    public override WebApplication Run(WebApplication app)
    {
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error", createScopeForErrors: true);
            app.UseHsts();
        }
        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseAntiforgery();
        app.UseFastEndpoints();
        app.MapRazorComponents<App>().AddInteractiveServerRenderMode();
        return app;
    }
}
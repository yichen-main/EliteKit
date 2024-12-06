using EliteKit.Infrastructure.Core.Expansions.CommonExamples;
using EliteKit.Infrastructure.Serve.Abstracts.StructureBuilders;
using EliteKit.Infrastructure.Serve.Attributes.SystemMaintainers;

namespace EliteKit.Infrastructure.Serve.Functions.StructuralFrames;
public static class LoadHost
{
    public static async Task BuildServerAsync<T>(Action<Exception>? e = default) where T : BuildServerFactory<T>
    {
        var factory = typeof(T);
        var assembly = Assembly.GetAssembly(factory);
        try
        {
            var builder = WebApplication.CreateBuilder(new WebApplicationOptions
            {
                Args = Environment.GetCommandLineArgs(),
                ContentRootPath = AppContext.BaseDirectory,
            });
            var provider = builder.Services.AddSingleton(factory).BuildServiceProvider();
            var modular = factory.GetSpecificTypes<ModularAttribute>().First(x =>
            {
                return factory.Name.IsFuzzy(x.Name);
            });
            var method = modular.GetCustomAttributes<ModularAttribute>().First().Name;
            var station = modular.GetMethod(method)?.Invoke(provider.GetService(modular), parameters: [
                assembly, builder,
            ]);
            if (station is Task<WebApplication?> task)
            {
                var app = await task.ConfigureAwait(false);
                if (app is not null) await app.RunAsync().ConfigureAwait(false);
            }
        }
        catch (Exception exception)
        {
            if (e is not null) e(exception);
            if (assembly is not null) exception.Fatal(typeof(LoadHost));
        }
        finally
        {
            await Log.CloseAndFlushAsync().ConfigureAwait(false);
        }
    }
}
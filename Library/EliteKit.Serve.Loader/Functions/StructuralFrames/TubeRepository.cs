using EliteKit.Serve.Loader.Interfaces.StructuralFrames;

namespace EliteKit.Serve.Loader.Functions.StructuralFrames;
internal sealed class TubeRepository(WebApplicationBuilder webApplicationBuilder) : ITubeDecorator
{
    public async ValueTask<WebApplicationBuilder> AddAsync(Func<WebApplicationBuilder, ValueTask> builder)
    {
        await builder(webApplicationBuilder).ConfigureAwait(false);
        return webApplicationBuilder;
    }
}
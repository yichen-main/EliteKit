using EliteKit.Serve.Loader.Attributes.ModuleComponents;
using EliteKit.Serve.Loader.Interfaces.StructuralFrames;

namespace EliteKit.Serve.Loader.Functions.StructuralFrames;
internal sealed class FiberRepository(WebApplicationBuilder builder, ITubeDecorator decorator) : TubeDecorator(decorator)
{
    public override async ValueTask<WebApplicationBuilder> AddAsync(Func<WebApplicationBuilder, ValueTask>? builder)
    {
        if (builder is not null) await builder(Builder).ConfigureAwait(false);
        return Builder;
    }
    WebApplicationBuilder Builder { get; init; } = builder;
}
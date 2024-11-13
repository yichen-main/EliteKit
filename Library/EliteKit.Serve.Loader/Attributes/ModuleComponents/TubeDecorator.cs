using EliteKit.Serve.Loader.Interfaces.StructuralFrames;

namespace EliteKit.Serve.Loader.Attributes.ModuleComponents;
internal abstract class TubeDecorator(ITubeDecorator decorator) : ITubeDecorator
{
    public virtual ValueTask<WebApplicationBuilder> AddAsync(Func<WebApplicationBuilder, ValueTask> builder) => decorator.AddAsync(builder);
}
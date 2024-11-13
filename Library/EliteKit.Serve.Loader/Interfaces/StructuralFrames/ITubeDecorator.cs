namespace EliteKit.Serve.Loader.Interfaces.StructuralFrames;
public interface ITubeDecorator
{
    ValueTask<WebApplicationBuilder> AddAsync(Func<WebApplicationBuilder, ValueTask> builder);
}
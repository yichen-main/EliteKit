namespace EliteKit.Serve.Loader.Interfaces.StructuralFrames;
public interface IEditionConstructor
{
    ValueTask InitialAsync(Assembly assembly);
    void Add(in Action<KestrelServerOptions> options);
    WebApplication Add(Action<WebApplication, IEndpointRouteBuilder> options);
    WebApplicationBuilder Builder { get; }
}
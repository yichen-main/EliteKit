using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Server.Kestrel.Core;

namespace EliteKit.Serve.Architects.Grindstones.Quarterlies;
public interface IEditionRepository
{
    ValueTask InitialAsync(Assembly assembly);
    void Add(in Action<KestrelServerOptions> options);
    WebApplication Add(Action<WebApplication, IEndpointRouteBuilder> options);
    WebApplicationBuilder Builder { get; }
}
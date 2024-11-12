using EliteKit.Serve.Loader.Architects.Grindstones.Expansions;

namespace EliteKit.Serve.Loader.Architects.Grindstones.Implements;
public static class LoadedHost
{
    public static ValueTask MiddleAsync<T>(Action<Exception>? e = default) where T : FactoryBuilder
    {
        return AttackExpand.CreateAsync<MiddlePlatform, T>(e: exception =>
        {
            if (e is not null) e(exception);
        });
    }
}
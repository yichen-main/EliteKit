using EliteKit.Serve.Loader.Attributes.StructureBuilders;
using EliteKit.Serve.Loader.Expansions.CommonExamples;

namespace EliteKit.Serve.Loader.Functions.StructuralFrames;
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
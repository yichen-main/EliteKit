namespace EliteKit.Infrastructure.Core.Expansions.StructureBehaviors;
internal static class SystemModularExpansion
{
    public static Type SetRootModule(this Type type)
    {
        RootModuleInfos[type.Name] = type;
        return type;
    }
    public static void DelRootModule(this Type type) => RootModuleInfos.TryRemove(type.Name, out _);
    public static ConcurrentDictionary<string, Type> RootModuleInfos { get; private set; } = [];
}
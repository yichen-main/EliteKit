namespace EliteKit.Infrastructure.Core.Functions.BackgroundServices;
internal sealed class ModuleExecutionJob : HostedService
{
    protected override Task ExecuteAsync(CancellationToken ct)
    {
        SetInterval(this, Interval.TenSeconds);
        return KeepAsync(this, async timer =>
        {
            foreach (var rootType in SystemModularExpansion.RootModuleInfos.Values)
            {
                foreach (var tableType in rootType.GetAssemblyTypes<NpgsqlBase>())
                {
                    await InitialParameters.CreateTableAsync(tableType, ct).ConfigureAwait(false);
                }
                rootType.DelRootModule();
            }
            InitialParameters.SetEnabled();
            timer.Dispose();
        });
    }
    public required IInitialParameters InitialParameters { get; init; }
}
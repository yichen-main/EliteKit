namespace EliteKit.Infrastructure.Core.Repositories.DataManagements;
public interface INpgsqlHelper
{
    Task ExecuteAsync(Func<NpgsqlConnection, Task> connection, CancellationToken ct);
    Task TransactionAsync(Func<NpgsqlConnection, NpgsqlTransaction, Task> connection, CancellationToken ct);
    Task QueriesAsync(Func<SqlMapper.GridReader, Task> reader, CancellationToken ct, params IEnumerable<string> fields);
}

[Dependent(ServiceLifetime.Singleton)]
file sealed class NpgsqlHelper : INpgsqlHelper
{
    public async Task ExecuteAsync(Func<NpgsqlConnection, Task> connection, CancellationToken ct)
    {
        if (InitialParameters.Enabled && InitialParameters.ConnectionString is not null)
        {
            NpgsqlConnection npgsqlConnection = new(InitialParameters.ConnectionString);
            await using (npgsqlConnection.ConfigureAwait(false))
            {
                await npgsqlConnection.OpenAsync(ct).ConfigureAwait(false);
                await connection(npgsqlConnection).ConfigureAwait(false);
            }
        }
    }
    public async Task TransactionAsync(Func<NpgsqlConnection, NpgsqlTransaction, Task> connection, CancellationToken ct)
    {
        if (InitialParameters.Enabled && InitialParameters.ConnectionString is not null)
        {
            NpgsqlConnection npgsqlConnection = new(InitialParameters.ConnectionString);
            await using (npgsqlConnection.ConfigureAwait(false))
            {
                await npgsqlConnection.OpenAsync(ct).ConfigureAwait(false);
                var transaction = await npgsqlConnection.BeginTransactionAsync(ct).ConfigureAwait(false);
                try
                {
                    await connection(npgsqlConnection, transaction).ConfigureAwait(false);
                    await transaction.CommitAsync(ct).ConfigureAwait(false);
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync(ct).ConfigureAwait(false);
                    throw;
                }
                finally
                {
                    await transaction.DisposeAsync().ConfigureAwait(false);
                }
            }
        }
    }
    public async Task QueriesAsync(Func<SqlMapper.GridReader, Task> reader, CancellationToken ct, params IEnumerable<string> fields)
    {
        if (InitialParameters.Enabled && InitialParameters.ConnectionString is not null)
        {
            NpgsqlConnection npgsqlConnection = new(InitialParameters.ConnectionString);
            await using (npgsqlConnection.ConfigureAwait(false))
            {
                await npgsqlConnection.OpenAsync(ct).ConfigureAwait(false);
                var gridReader = await npgsqlConnection.QueryMultipleAsync(';'.Join(fields)).ConfigureAwait(false);
                await reader(gridReader).ConfigureAwait(false);
            }
        }
    }
    public required IInitialParameters InitialParameters { get; init; }
}
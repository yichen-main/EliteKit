namespace EliteKit.Infrastructure.Core.Repositories.BaseInstallations;
internal interface IInitialParameters
{
    void SetEnabled();
    void SetConnectionString(in string value);
    Task CreateTableAsync(Type type, CancellationToken ct);
    bool Enabled { get; }
    string? ConnectionString { get; }
}

[Dependent(ServiceLifetime.Singleton)]
file sealed class InitialParameters : IInitialParameters
{
    public void SetEnabled() => Enabled = true;
    public void SetConnectionString(in string value) => ConnectionString ??= value;
    public async Task CreateTableAsync(Type type, CancellationToken ct)
    {
        List<string> columns = [];
        List<string> indexes = [];
        var properties = type.GetProperties();
        var tableName = type.Name.ToSnakeCase();
        for (int i = default; i < properties.Length; i++)
        {
            var property = properties[i];
            var columnName = property.Name.ToSnakeCase();
            var rowInfo = property.GetCustomAttribute<RowInfoAttribute>();
            if (rowInfo is { UniqueIndex: true }) indexes.Add($"""
            CREATE UNIQUE INDEX IF NOT EXISTS {tableName}_{columnName} ON {tableName} ({columnName});
            """);
            switch (property.Name)
            {
                case nameof(NpgsqlBase.Id):
                    columns.Add($"{columnName} CHARACTER(18) PRIMARY KEY");
                    break;

                default:
                    if (rowInfo is { ForeignKey: true })
                    {
                        columns.Add($"{columnName} CHARACTER(18) NOT NULL");
                    }
                    else columns.Add($"{columnName} {property.PropertyType switch
                    {
                        var x when x.IsEnum => "SMALLINT",
                        var x when x.Equals(typeof(Guid)) => "UUID",
                        var x when x.Equals(typeof(bool)) => "BOOLEAN",
                        var x when x.Equals(typeof(short)) => "SMALLINT",
                        var x when x.Equals(typeof(int)) => "INTEGER",
                        var x when x.Equals(typeof(long)) => "BIGINT",
                        var x when x.Equals(typeof(float)) => "REAL",
                        var x when x.Equals(typeof(double)) => "DOUBLE PRECISION",
                        var x when x.Equals(typeof(decimal)) => "MONEY",
                        var x when x.Equals(typeof(string[])) => "TEXT[]",
                        var x when x.Equals(typeof(TimeSpan)) => "TIME",
                        var x when x.Equals(typeof(DateTime)) => "TIMESTAMP WITHOUT TIME ZONE",
                        _ => "VARCHAR",
                    }} NOT NULL");
                    break;
            }
        }
        if (ConnectionString is null) return;
        NpgsqlConnection connection = new(ConnectionString);
        await using (connection.ConfigureAwait(false))
        {
            await connection.OpenAsync(ct).ConfigureAwait(false);
            await connection.ExecuteAsync($"""
            CREATE TABLE IF NOT EXISTS {type.Name.ToSnakeCase()}({','.Join(columns)})
            """).ConfigureAwait(false);
            foreach (var index in indexes) await connection.ExecuteAsync(index).ConfigureAwait(false);
        }
    }
    public bool Enabled { get; private set; }
    public string? ConnectionString { get; private set; }
}
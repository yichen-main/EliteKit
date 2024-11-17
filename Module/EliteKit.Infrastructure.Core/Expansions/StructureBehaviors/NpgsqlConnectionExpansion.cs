namespace EliteKit.Infrastructure.Core.Expansions.StructureBehaviors;
public static class NpgsqlConnectionExpansion
{
    public static async Task InsertAsync<T>(this NpgsqlConnection connection, T entity, CancellationToken ct, NpgsqlTransaction? transaction = null) where T : NpgsqlBase
    {
        List<string> columns = [];
        var type = entity.GetType();
        DynamicParameters parameters = new();
        foreach (var property in type.GetProperties())
        {
            var value = property.GetValue(entity);
            var name = property.Name.ToSnakeCase();
            if (value is not null)
            {
                columns.Add(name);
                parameters.Add(name, value);
            }
        }
        CommandDefinition command = new(commandText: $"""
        INSERT INTO {type.Name.ToSnakeCase()}({','.Join(columns)}) VALUES ({','.Join(columns.Select(x => $"@{x}"))})
        """, parameters: parameters, transaction: transaction, commandType: CommandType.Text, cancellationToken: ct);
        await connection.ExecuteAsync(command).ConfigureAwait(false);
    }
    public static async Task UpdateAsync<T>(this NpgsqlConnection connection, string id, T entity, CancellationToken ct, NpgsqlTransaction? transaction = null) where T : NpgsqlBase
    {
        List<string> columns = [];
        var type = entity.GetType();
        var keyName = nameof(NpgsqlBase.Id);
        DynamicParameters parameters = new();
        parameters.Add($"@{keyName}", id);
        foreach (var property in type.GetProperties())
        {
            var value = property.GetValue(entity);
            var name = property.Name.ToSnakeCase();
            if (value is not null)
            {
                columns.Add(Condition(name));
                parameters.Add(name, value);
            }
        }
        CommandDefinition command = new(commandText: $"""
        UPDATE {type.Name.ToSnakeCase()} SET {','.Join(columns)} WHERE {Condition(keyName)}
        """, parameters: parameters, transaction: transaction, commandType: CommandType.Text, cancellationToken: ct);
        await connection.ExecuteAsync(command).ConfigureAwait(false);
        static string Condition(in string column) => $"{column.ToSnakeCase()}=@{column}";
    }
    public static Task<IEnumerable<T>> SelectAsync<T>(this NpgsqlConnection connection, string id, in QueryIntervalFilter? interval = null, in CancellationToken ct = default)
    {
        return connection.SelectAsync<T>(new QueryColumnFilter
        {
            Name = nameof(NpgsqlBase.Id),
            Value = id,
        }, interval, ct);
    }
    public static Task<IEnumerable<T>> SelectAsync<T>(this NpgsqlConnection connection, in QueryColumnFilter? column = null, in QueryIntervalFilter? interval = null, in CancellationToken ct = default)
    {
        var type = typeof(T);
        List<string> columns = [];
        foreach (var property in type.GetProperties()) columns.Add(property.Name.ToSnakeCase());
        StringBuilder builder = new($"SELECT {','.Join(columns)} FROM {type.Name.ToSnakeCase()} ");
        if (column is not null) builder.Append($"WHERE {column.Name.ToSnakeCase()} = '{column.Value}' ");
        var createTimeTag = nameof(NpgsqlBase.CreateTime).ToSnakeCase();
        if (interval is not null)
        {
            builder.Append($"WHERE {createTimeTag} BETWEEN '{To(interval.StartTime)}' AND '{To(interval.EndTime)}' ");
            static string To(in DateTime time) => time.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.CurrentCulture);
        }
        builder.Append($"ORDER BY {createTimeTag} DESC");
        CommandDefinition command = new(commandText: builder.ToString(), commandType: CommandType.Text, cancellationToken: ct);
        return connection.QueryAsync<T>(command);
    }
}
namespace Impulse.Data.SqlServer.Extensions;

internal static class DbConnectionExtensions
{
    /// <summary>
    /// Executes the specified stored procedure.
    /// </summary>
    public static Task<int> ExecuteProcAsync(this IDbConnection connection, string name, object? parameters = null, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        Guard.IsNotNull(connection);
        Guard.IsNotNull(name);

        return connection.ExecuteAsync(new CommandDefinition(
            name,
            parameters,
            null,
            timeout.HasValue ? (int?)timeout.Value.TotalSeconds : null,
            CommandType.StoredProcedure,
            CommandFlags.Buffered,
            cancellationToken));
    }

    /// <summary>
    /// Executes the specified scalar stored procedure.
    /// </summary>
    public static Task<T> ExecuteScalarProcAsync<T>(this IDbConnection connection, string name, object? parameters = null, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        Guard.IsNotNull(connection);
        Guard.IsNotNull(name);

        return connection.ExecuteScalarAsync<T>(new CommandDefinition(
            name,
            parameters,
            null,
            timeout.HasValue ? (int?)timeout.Value.TotalSeconds : null,
            CommandType.StoredProcedure,
            CommandFlags.Buffered,
            cancellationToken));
    }

    /// <summary>
    /// Executes the specified stored procedure and returns its query result.
    /// </summary>
    public static Task<IEnumerable<T>> QueryProcAsync<T>(this IDbConnection connection, string name, object? parameters = null, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        Guard.IsNotNull(connection);
        Guard.IsNotNull(name);

        return connection.QueryAsync<T>(new CommandDefinition(
            name,
            parameters,
            null,
            timeout.HasValue ? (int?)timeout.Value.TotalSeconds : null,
            CommandType.StoredProcedure,
            CommandFlags.Buffered,
            cancellationToken));
    }

    /// <summary>
    /// Executes the specified stored procedure and returns its single result.
    /// </summary>
    public static Task<T> QuerySingleProcAsync<T>(this IDbConnection connection, string name, object? parameters = null, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        Guard.IsNotNull(connection);
        Guard.IsNotNull(name);

        return connection.QuerySingleAsync<T>(new CommandDefinition(
            name,
            parameters,
            null,
            timeout.HasValue ? (int?)timeout.Value.TotalSeconds : null,
            CommandType.StoredProcedure,
            CommandFlags.Buffered,
            cancellationToken));
    }

    /// <summary>
    /// Executes the specified stored procedure and returns its single result or null.
    /// </summary>
    public static Task<T> QuerySingleOrDefaultProcAsync<T>(this IDbConnection connection, string name, object? parameters = null, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        Guard.IsNotNull(connection);
        Guard.IsNotNull(name);

        return connection.QuerySingleOrDefaultAsync<T>(new CommandDefinition(
            name,
            parameters,
            null,
            timeout.HasValue ? (int?)timeout.Value.TotalSeconds : null,
            CommandType.StoredProcedure,
            CommandFlags.Buffered,
            cancellationToken));
    }
}
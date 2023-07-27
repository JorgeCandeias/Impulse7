using CommunityToolkit.Diagnostics;
using Dapper;
using System.Data;

namespace Impulse.Data.SqlServer.Extensions;

internal static class DbConnectionExtensions
{
    /// <summary>
    /// Executes the specified stored procedure and returns its query result.
    /// </summary>
    public static Task<IEnumerable<T>> QueryProcAsync<T>(this IDbConnection connection, string name, object? parameters = null, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        Guard.IsNotNull(connection);

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

        return connection.QuerySingleAsync<T>(new CommandDefinition(
            name,
            parameters,
            null,
            timeout.HasValue ? (int?)timeout.Value.TotalSeconds : null,
            CommandType.StoredProcedure,
            CommandFlags.Buffered,
            cancellationToken));
    }
}
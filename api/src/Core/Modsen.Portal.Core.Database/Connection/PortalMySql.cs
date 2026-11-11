using System.Data.Common;
using Microsoft.EntityFrameworkCore.Infrastructure;
using MySqlConnector;

// Неймспейс намеренно Microsoft.EntityFrameworkCore — чтобы все точки вызова
// UseNpgsql заменились одним токен-свапом на UsePortalMySql без правки using-ов.
namespace Microsoft.EntityFrameworkCore
{
    /// <summary>Единая точка подключения к MySQL 8 (Pomelo) для всех модулей платформы.</summary>
    public static class PortalMySqlExtensions
    {
        private static MySqlServerVersion Version => new(new Version(8, 0, 36));

        public static DbContextOptionsBuilder UsePortalMySql(
            this DbContextOptionsBuilder options, string connectionString) =>
            options.UseMySql(connectionString, Version);

        public static DbContextOptionsBuilder UsePortalMySql(
            this DbContextOptionsBuilder options, string connectionString,
            Action<MySqlDbContextOptionsBuilder> action) =>
            options.UseMySql(connectionString, Version, action);

        public static DbContextOptionsBuilder UsePortalMySql(
            this DbContextOptionsBuilder options, DbConnection connection) =>
            options.UseMySql(connection, Version);

        public static DbContextOptionsBuilder UsePortalMySql(
            this DbContextOptionsBuilder options, DbConnection connection,
            Action<MySqlDbContextOptionsBuilder> action) =>
            options.UseMySql(connection, Version, action);

        public static DbContextOptionsBuilder<TContext> UsePortalMySql<TContext>(
            this DbContextOptionsBuilder<TContext> options, string connectionString)
            where TContext : DbContext =>
            options.UseMySql<TContext>(connectionString, Version);

        public static DbContextOptionsBuilder<TContext> UsePortalMySql<TContext>(
            this DbContextOptionsBuilder<TContext> options, string connectionString,
            Action<MySqlDbContextOptionsBuilder> action)
            where TContext : DbContext =>
            options.UseMySql<TContext>(connectionString, Version, action);

        public static DbContextOptionsBuilder<TContext> UsePortalMySql<TContext>(
            this DbContextOptionsBuilder<TContext> options, DbConnection connection)
            where TContext : DbContext =>
            options.UseMySql<TContext>(connection, Version);

        public static DbContextOptionsBuilder<TContext> UsePortalMySql<TContext>(
            this DbContextOptionsBuilder<TContext> options, DbConnection connection,
            Action<MySqlDbContextOptionsBuilder> action)
            where TContext : DbContext =>
            options.UseMySql<TContext>(connection, Version, action);
    }
}

namespace Modsen.Portal.Core.Database
{
    /// <summary>Замена NpgsqlDataSource: открывает MySqlConnection по строке подключения.</summary>
    public sealed class PortalDbDataSource
    {
        private readonly string _connectionString;

        public PortalDbDataSource(string connectionString) => _connectionString = connectionString;

        public string ConnectionString => _connectionString;

        public DbConnection OpenConnection()
        {
            var connection = new MySqlConnection(_connectionString);
            connection.Open();
            return connection;
        }

        public DbConnection CreateConnection() => new MySqlConnection(_connectionString);
    }
}

using System.Data;
using System.Data.Common;
using Modsen.Portal.Core.Database;
using Npgsql;

namespace Modsen.Portal.Core.UseCases;

internal sealed class ConnectionFactory : IConnectionFactory
{
    private readonly NpgsqlDataSource _dataSource;
    private DbConnection? _connection;
    private DbTransaction? _transaction;
    private readonly Lock _lockObject = new();
    private bool _disposedValue;

    public ConnectionFactory(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    public DbConnection GetConnection()
    {
        lock (_lockObject)
        {
            ObjectDisposedException.ThrowIf(_disposedValue, this);
            return _connection ??= _dataSource.OpenConnection();
        }
    }

    public DbTransaction GetTransaction()
    {
        ObjectDisposedException.ThrowIf(_disposedValue, this);
        return _transaction ??= GetConnection().BeginTransaction(IsolationLevel.ReadCommitted);
    }

    public Task CommitCurrentTransaction(CancellationToken token)
    {
        ObjectDisposedException.ThrowIf(_disposedValue, this);
        return GetTransaction().CommitAsync(token);
    }

    private void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                _transaction?.Dispose();
                _transaction = null;

                lock (_lockObject)
                {
                    if (_connection != null)
                    {
                        _connection.Close();
                        _connection.Dispose();
                    }
                    _connection = null;
                }
            }

            _disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    ~ConnectionFactory()
    {
        Dispose(disposing: false);
    }
}

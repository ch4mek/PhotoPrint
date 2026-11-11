using System.Data.Common;

namespace Modsen.Portal.Core.Database;

public interface IConnectionFactory : IDisposable
{
    DbConnection GetConnection();
    DbTransaction GetTransaction();

    Task CommitCurrentTransaction(CancellationToken token);
}

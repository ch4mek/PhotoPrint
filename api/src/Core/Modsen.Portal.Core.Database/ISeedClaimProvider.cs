using Microsoft.EntityFrameworkCore;

namespace Modsen.Portal.Core.Database;

public interface ISeedClaimProvider<out TContext, in TEntity>
    where TContext : DbContext
    where TEntity : class
{
    TContext DbContext { get; }
    string Scope { get; }
    void Seed(TEntity entity);
}

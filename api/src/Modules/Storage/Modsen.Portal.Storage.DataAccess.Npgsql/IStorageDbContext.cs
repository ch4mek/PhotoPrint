using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Storage.DataAccess.Npgsql.Entities;

[assembly: InternalsVisibleTo("Modsen.Portal.Storage.Requests")]
namespace Modsen.Portal.Storage.DataAccess.Npgsql;

internal interface IStorageDbContext
{
    public DbSet<ApplicationFileEntity> ApplicationFiles { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}

using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Finance.DataAccess.Npgsql.Entities;

[assembly: InternalsVisibleTo("Modsen.Portal.Finance.Requests")]

namespace Modsen.Portal.Finance.DataAccess.Npgsql;

internal interface IFinanceDbContext
{
    public DbSet<CurrencyEntity> Currencies { get; }

    public DbSet<ExchangeRateEntity> ExchangeRates { get; }

    public DbSet<ExpenseCategoryEntity> ExpenseCategories { get; }

    public DbSet<ExpenseEntity> Expenses { get; }

    public DbSet<FinanceClientEntity> FinanceClients { get; }

    public DbSet<FinanceDepartmentEntity> FinanceDepartments { get; }

    public DbSet<FinanceProjectEntity> FinanceProjects { get; }
    public DbSet<FinanceProjectUserEntity> FinanceProjectUsers { get; }

    public DbSet<FinanceUserEntity> FinanceUsers { get; }

    public DbSet<ProjectRateEntity> ProjectRates { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}

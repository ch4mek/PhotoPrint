using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Finance.DataAccess.Npgsql.Entities;

[assembly: InternalsVisibleTo("Modsen.Portal.Migrator")]
namespace Modsen.Portal.Finance.DataAccess.Npgsql;

internal class FinanceDbContext : DbContext, IFinanceDbContext
{
    public const string Schema = "finance";

    public DbSet<CurrencyEntity> Currencies { get; set; }

    public DbSet<ExchangeRateEntity> ExchangeRates { get; set; }

    public DbSet<ExpenseCategoryEntity> ExpenseCategories { get; set; }

    public DbSet<ExpenseEntity> Expenses { get; set; }

    public DbSet<FinanceClientEntity> FinanceClients { get; set; }

    public DbSet<FinanceDepartmentEntity> FinanceDepartments { get; set; }

    public DbSet<FinanceProjectEntity> FinanceProjects { get; set; }
    public DbSet<FinanceProjectUserEntity> FinanceProjectUsers { get; set; }

    public DbSet<FinanceUserEntity> FinanceUsers { get; set; }

    public DbSet<ProjectRateEntity> ProjectRates { get; set; }

    public FinanceDbContext(DbContextOptions<FinanceDbContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schema);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(FinanceDbContext).Assembly);
    }
}

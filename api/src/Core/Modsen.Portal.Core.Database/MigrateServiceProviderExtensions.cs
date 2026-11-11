using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Modsen.Portal.Core.Context;

namespace Modsen.Portal.Core.Database;

public static class MigrateServiceProviderExtensions
{
    public static void ExecuteSeeds<TContext>(this IServiceProvider scope) where TContext : DbContext
    {
        var seedProviders = scope.GetServices<ISeedEntitiesProvider<TContext>>();
        var logger = scope.GetRequiredService<ILogger<TContext>>();

        foreach (var seedProvider in seedProviders)
        {
            seedProvider.Seed();
            logger.LogInformation("Seed has been finished for {Context}", seedProvider.ContextType.Name);
        }
    }

    public static IQueryable<T> WhereGreaterOrEqual<T>(
        this IQueryable<T> source,
        Expression<Func<T, DateTime>> selector,
        DateOnly? start)
    {
        ArgumentNullException.ThrowIfNull(selector);
        if (!start.HasValue)
        {
            return source;
        }

        var from = start.Value.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
        return source.Where(BuildPredicate(selector, from, true));
    }

    public static IQueryable<T> WhereLessOrEqual<T>(
        this IQueryable<T> source,
        Expression<Func<T, DateTime>> selector,
        DateOnly? end)
    {
        ArgumentNullException.ThrowIfNull(selector);

        if (!end.HasValue)
        {
            return source;
        }

        var to = end.Value.ToDateTime(TimeOnly.MaxValue, DateTimeKind.Utc);
        return source.Where(BuildPredicate(selector, to, false));
    }

    private static Expression<Func<T, bool>> BuildPredicate<T>(
        Expression<Func<T, DateTime>> selector,
        DateTime boundary,
        bool isStart)
    {
        var param = selector.Parameters[0];
        var body = isStart
            ? Expression.GreaterThanOrEqual(selector.Body, Expression.Constant(boundary))
            : Expression.LessThanOrEqual(selector.Body, Expression.Constant(boundary));

        return Expression.Lambda<Func<T, bool>>(body, param);
    }

    public static PropertyBuilder<int> HasReverseOrderEnumComputedColumn<TEntity, TEnum>(
        this PropertyBuilder<int> propertyBuilder,
        Expression<Func<TEntity, TEnum>> enumPropertyExpression)
        where TEntity : class
        where TEnum : struct, Enum
    {
        ArgumentNullException.ThrowIfNull(enumPropertyExpression);

        if (enumPropertyExpression.Body is not MemberExpression memberExpr)
        {
            throw new ArgumentException("Expression must be a property", nameof(enumPropertyExpression));
        }

        var columnName = memberExpr.Member.Name;

        var enumValues = Enum.GetValues<TEnum>().Reverse().ToList();

        var cases = string.Join(" ", enumValues.Select((ev, index) =>
            $"WHEN '{ev}' THEN {(enumValues.Count - 1 - index) * 100}"));

        var sql = $"CASE \"{columnName}\" {cases} ELSE 0 END";

        return propertyBuilder.HasComputedColumnSql(sql, stored: true);
    }

    public static void VerifyDataConsistency(this IServiceProvider serviceProvider)
    {
        var envContext = serviceProvider.GetRequiredService<IHostEnvironmentContext>();
        if (envContext.IsProductionEnvironment() ||
            envContext.IsIntegrationTestEnvironment() ||
            envContext.IsFunctionalTestEnvironment())
        {
            foreach (var guard in serviceProvider.GetServices<IDataConsistencyGuard>())
            {
                guard.Verify();
            }
        }
    }
}

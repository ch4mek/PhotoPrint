namespace Modsen.Portal.Core.Database;

public interface ISeedEntitiesProvider<TContext> where TContext : class
{
    Type ContextType => typeof(TContext);
    void Seed();
}

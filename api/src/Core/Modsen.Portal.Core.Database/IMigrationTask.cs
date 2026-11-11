namespace Modsen.Portal.Core.Database;

public interface IMigrationTask
{
    string TaskName { get; }
    void Execute();
}

namespace Modsen.Portal.Identity.Contract.Models;

public sealed record DepartmentWithNameOutput(
    string DepartmentId,
    string? ShortName,
    string? FullName);

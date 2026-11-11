namespace Modsen.Portal.Core;

[Flags]
public enum AccessOptions
{
    None = 0,
    Self = 1,
    Manager = 2,
    Approver = 4,
    Substitute = 8,
    Hr = 16,
    Accountant = 32,
    RequestViewer = 64,
    Coordinator = 128,
}

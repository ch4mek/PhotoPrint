using System.ComponentModel;

namespace Modsen.Portal.Core.UseCases;

public enum NumberEncryptorScopes
{
    [Description("None")]
    None = 0,
    [Description("Salary")]
    Salary = 1,
    [Description("ProjectRate")]
    ProjectRate = 2,
    [Description("Expense")]
    Expense = 3
}

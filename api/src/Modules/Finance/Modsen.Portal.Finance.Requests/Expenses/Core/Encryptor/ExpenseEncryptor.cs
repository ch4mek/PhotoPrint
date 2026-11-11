using Modsen.Portal.Core.Services;
using Modsen.Portal.Core.UseCases;
using Modsen.Portal.Core.Utils.Extensions;

namespace Modsen.Portal.Finance.Requests.Expenses.Core.Encryptor;

public interface IExpenseEncryptor : INumberEncryptor;

internal class ExpenseEncryptor : NumberEncryptorBase, IExpenseEncryptor
{
    public ExpenseEncryptor(IDataProtectionFactory factory)
        : base(factory, NumberEncryptorScopes.Expense.GetDescription())
    {
    }
}

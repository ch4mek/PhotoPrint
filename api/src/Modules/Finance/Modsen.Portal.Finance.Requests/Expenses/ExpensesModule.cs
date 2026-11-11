using Microsoft.Extensions.DependencyInjection;
using Modsen.Portal.Core.Utils.Modules;
using Modsen.Portal.Finance.Requests.Expenses.Core.Builders;
using Modsen.Portal.Finance.Requests.Expenses.Core.Encryptor;

namespace Modsen.Portal.Finance.Requests.Expenses;

internal sealed class ExpensesModule : RegistrationModule
{
    public override void Load(IServiceCollection services)
    {
        services.AddScoped<IExpenseEncryptor, ExpenseEncryptor>();

        services.AddScoped<IExpensesQueryBuilder, ExpensesQueryBuilder>();
    }
}

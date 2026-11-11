using Microsoft.Extensions.DependencyInjection;
using Modsen.Portal.Core.Utils.Modules;
using Modsen.Portal.Merch.Requests.Transactions.Core.Composers;

namespace Modsen.Portal.Merch.Requests.Transactions;

internal sealed class TransactionsModule : RegistrationModule
{
    public override void Load(IServiceCollection services)
    {
        services.AddScoped<IReceiptTransactionsComposer, ReceiptTransactionsComposer>();
        services.AddScoped<ITransferTransactionsComposer, TransferTransactionsComposer>();
        services.AddScoped<IWriteOffTransactionsComposer, WriteOffTransactionsComposer>();
    }
}

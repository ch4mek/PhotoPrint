using MediatR;
using Modsen.Portal.Core.Database;
using Modsen.Portal.Core.Services.Cqrs;

namespace Modsen.Portal.Core.UseCases.Pipeline;

public sealed class DbTransactionPipe<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>, ITransactionalRequest
{
    private readonly IConnectionFactory _connectionFactory;

    public DbTransactionPipe(IConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(next);

        await using var transaction = _connectionFactory.GetTransaction();

        var response = await next(cancellationToken);

        await transaction.CommitAsync(cancellationToken);

        return response;
    }
}

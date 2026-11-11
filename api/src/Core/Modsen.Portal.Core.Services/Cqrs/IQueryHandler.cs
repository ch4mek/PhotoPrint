using MediatR;

namespace Modsen.Portal.Core.Services.Cqrs;

public interface IQueryHandler<in TQuery, TResult> : IRequestHandler<TQuery, TResult>
    where TQuery : IQuery<TResult>;

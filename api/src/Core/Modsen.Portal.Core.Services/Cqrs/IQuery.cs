using MediatR;

namespace Modsen.Portal.Core.Services.Cqrs;

public interface IQuery<out TResult> : IRequest<TResult>;

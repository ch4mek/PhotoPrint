using MediatR;

namespace Modsen.Portal.Core.Services.Cqrs;

public interface ICommand<out TResult> : IRequest<TResult>;

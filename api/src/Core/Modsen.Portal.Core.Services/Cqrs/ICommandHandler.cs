using MediatR;

namespace Modsen.Portal.Core.Services.Cqrs;

public interface ICommandHandler<in TCommand, TResult> : IRequestHandler<TCommand, TResult>
    where TCommand : ICommand<TResult>;

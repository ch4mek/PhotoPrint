using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Modsen.Portal.Core.Exceptions;

namespace Modsen.Portal.Core.UseCases.Pipeline;

public sealed class FluentValidationPipe<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public FluentValidationPipe(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(next);

        var failures = new List<ValidationFailure>();

        foreach (var validator in _validators)
        {
            var result = await validator.ValidateAsync(
                request,
                cancellationToken);

            if (result != null)
            {
                failures.AddRange(result.Errors);
            }
        }

        if (failures.Count > 0)
        {
            throw new RequestValidationFailedException { Failures = failures };
        }

        return await next(cancellationToken);
    }
}

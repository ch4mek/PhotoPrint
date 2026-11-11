namespace Modsen.Portal.Core.Services;

public interface IDbQuery<in TRequest, TResponse>
{
    Task<TResponse> ExecuteAsync(TRequest request, CancellationToken cancellationToken);
}

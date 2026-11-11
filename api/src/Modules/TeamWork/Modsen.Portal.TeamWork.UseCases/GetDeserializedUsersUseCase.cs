using System.Text.RegularExpressions;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Core.Utils.Extensions;

namespace Modsen.Portal.TeamWork.UseCases;

internal sealed class GetDeserializedUsersUseCase : IQueryHandler<GetDeserializedUsersUseCase.Query, GetDeserializedUsersUseCase.Response>
{
    public Task<Response> Handle(Query request, CancellationToken cancellationToken)
    {
        if (request.Json.IsNullOrEmpty())
        {
            return Task.FromResult(new Response([]));
        }

        var matches = Regex.Matches(request.Json, @"\[~([^\]]+)\]", RegexOptions.None, TimeSpan.FromSeconds(1));

        var emails = matches
            .Select(m => m.Groups[1].Value)
            .Where(value => Regex.IsMatch(value, @"^[a-z0-9._%+-]+@[a-z0-9.-]+\.[a-z]{2,}$", RegexOptions.IgnoreCase, TimeSpan.FromSeconds(5)))
            .Select(email => email.ToLowerInvariant())
            .Distinct()
            .ToList();

        return Task.FromResult(new Response(emails));
    }

    internal sealed record Query(string Json) : IQuery<Response>;
    internal record Response(List<string> Users);
}

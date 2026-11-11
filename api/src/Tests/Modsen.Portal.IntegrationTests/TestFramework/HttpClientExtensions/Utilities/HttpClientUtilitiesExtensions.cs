using System.Net.Http.Json;
using System.Text.Json;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
using Modsen.Portal.TeamBoard.Contract.Contract.Output;
using Modsen.Portal.Utilities.Controllers.Agent;

namespace Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Utilities;

internal static class HttpClientUtilitiesExtensions
{
    private static readonly CancellationToken _cancellationToken = CancellationToken.None;

    public static Task<HttpResponseMessage> GetUserPermanentInfoAsHttpMessage(this HttpClient client,
        UserInfoInput input, JsonSerializerOptions jsonOptions)
    {
        return client.PostAsJsonAsync(
            "api/agent-service/user-info/permanent",
            input,
            jsonOptions,
            _cancellationToken);
    }

    public static Task<PermanentUserInfoOutput?> GetUserPermanentInfo(this HttpClient client,
        UserInfoInput input, JsonSerializerOptions jsonOptions)
    {
        return client.PostAsJsonAsync<UserInfoInput, PermanentUserInfoOutput>(
            "api/agent-service/user-info/permanent",
            input,
            jsonOptions,
            _cancellationToken);
    }

    public static Task<HttpResponseMessage> GetUserImpermanentInfoAsHttpMessage(this HttpClient client,
        UserInfoInput input, JsonSerializerOptions jsonOptions)
    {
        return client.PostAsJsonAsync(
            "api/agent-service/user-info/impermanent",
            input,
            jsonOptions,
            _cancellationToken);
    }

    public static Task<ImpermanentUserInfoOutput?> GetUserImpermanentInfo(this HttpClient client,
        UserInfoInput input, JsonSerializerOptions jsonOptions)
    {
        return client.PostAsJsonAsync<UserInfoInput, ImpermanentUserInfoOutput>(
            "api/agent-service/user-info/impermanent",
            input,
            jsonOptions,
            _cancellationToken);
    }
}

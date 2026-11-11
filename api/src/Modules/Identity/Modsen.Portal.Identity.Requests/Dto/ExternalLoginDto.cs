using Modsen.Portal.Core.Authentication.Jwt;

namespace Modsen.Portal.Identity.Requests.Dto;

public record ExternalLoginDto(bool IsSucceeded, string? Error = null, TokenModel? Token = null)
{
    public static ExternalLoginDto Success(TokenModel token)
    {
        return new ExternalLoginDto(IsSucceeded: true, Token: token);
    }

    public static ExternalLoginDto Failed(string error)
    {
        return new ExternalLoginDto(IsSucceeded: false, Error: error);
    }
}

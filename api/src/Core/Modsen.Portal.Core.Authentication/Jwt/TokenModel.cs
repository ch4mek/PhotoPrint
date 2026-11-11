using System.Web;

namespace Modsen.Portal.Core.Authentication.Jwt;

public sealed record TokenModel(string Token, DateTime ValidTillUtc)
{
    public string ToUrlEncodedString()
    {
        return $"token={HttpUtility.UrlEncode(Token)}";
    }
}

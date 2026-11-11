using Modsen.Portal.Core.Utils.Extensions;

namespace Modsen.Portal.Core.Authentication.TokenContext;

public interface IPublicTokenMaskProvider
{
    string GetMaskedToken(string? token);
}

internal sealed class PublicTokenMaskProvider : IPublicTokenMaskProvider
{
    public string GetMaskedToken(string? token)
    {
        if (token.IsNullOrEmpty())
        {
            return "[null]";
        }

        if (token.Length < 16)
        {
            return $"{token.Substring(0, token.Length / 2)}{new string('*', 4)}";
        }

        return $"{token.Substring(0, 8)}{new string('*', 4)}{token.Substring(token.Length - 4)}";
    }
}

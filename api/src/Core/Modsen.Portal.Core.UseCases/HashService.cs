using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using Modsen.Portal.Core.Services;

namespace Modsen.Portal.Core.UseCases;

public sealed class HashService : IHashService
{
    public string GetSha256Hash(string input)
    {
        var data = SHA256.HashData(Encoding.UTF8.GetBytes(input));
        var sBuilder = new StringBuilder();

        foreach (var t in data)
        {
            sBuilder.Append(t.ToString("x2", CultureInfo.InvariantCulture));
        }

        return sBuilder.ToString();
    }
}

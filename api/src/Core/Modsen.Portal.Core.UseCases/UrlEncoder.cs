using System.Web;
using Modsen.Portal.Core.Services;

namespace Modsen.Portal.Core.UseCases;

internal sealed class UrlEncoder : IUrlEncoder
{
    public string Encode(string url)
    {
        return HttpUtility.UrlEncode(url);
    }
}

using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Api.Controllers;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Identity.DataAccess.Npgsql;
using Modsen.Portal.Identity.Requests.Users.Requests;

namespace Modsen.Portal.IntegrationTests;

internal class TestAccountController : BaseApiController
{
    private readonly IdentityModuleDbContext _identityModuleDbContext;
    private readonly ISender _sender;

    public TestAccountController(IdentityModuleDbContext identityModuleDbContext, ISender sender)
    {
        _identityModuleDbContext = identityModuleDbContext;
        _sender = sender;
    }

    [AllowAnonymous]
    [HttpGet("/api/account/tests/externalLoginCallback")]
    public async Task<string?> ExternalLoginCallback([FromQuery] string email)
    {
        var user = await _identityModuleDbContext.Users
            .Select(x => new
            {
                x.Email,
                GivenName = x.FirstName,
                Surname = x.LastName
            })
            .FirstOrDefaultAsync(x => x.Email == email) ?? throw new UnauthorizedException();

        var query = new ExternalLoginCallbackRequest.Command(
            new AuthenticationResult
            {
                Email = user.Email,
                GivenName = user.GivenName,
                Surname = user.Surname,
                ReturnUrl = "https://localhost:3000",
                Succeeded = true
            });

        var result = await _sender.Send(query);

        return result.Token?.Token;
    }
}

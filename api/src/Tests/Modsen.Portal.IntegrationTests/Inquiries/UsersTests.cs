using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Modsen.Portal.Inquiries.DataAccess.Npgsql;
namespace Modsen.Portal.IntegrationTests.Inquiries;

[Collection("InquiriesCollection")]
public class UsersTests
{
    private readonly IntegrationTestFactory _factory;
    private readonly JsonSerializerOptions _jsonOptions;

    public UsersTests(IntegrationTestFactory factory)
    {
        _factory = factory;
        _jsonOptions = factory.JsonOptions;
    }

    [Fact]
    public async Task SelectUsersCountFromContextMustBeGreatThanZero()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<IInquiriesDbContext>();

        // Assert
        context.InquiriesUsers.Count().Should().BeGreaterThan(0);
    }
}

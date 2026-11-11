using System.Net.Http.Json;
using System.Text.Json;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Modsen.Portal.Core.Database;
using Modsen.Portal.Core.Inquiries;
using Modsen.Portal.Inquiries.Controllers.Contracts.Input;
using Modsen.Portal.Inquiries.Controllers.Contracts.Output;
using Modsen.Portal.Inquiries.Requests.Agreements.Requests;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;

namespace Modsen.Portal.IntegrationTests.Extensions;

internal static class InquiriesTestHelper
{
    internal static async Task<bool> CheckIfAgreementsExist(this HttpClient? client, JsonSerializerOptions serializerOptions, int userId)
    {
        var agreementsResponse = await client.GetAsync(
         $"api/users/{userId}/agreements",
         CancellationToken.None);

        var agreements = await agreementsResponse.Content.ReadFromJsonAsync<List<UserAgreementOutput>>(
           serializerOptions,
           CancellationToken.None);

        return agreements.Count > 0;
    }

    internal static async Task PrepareAgreementsWithTransactions(this IServiceScope? scope, HttpClient? client, int userId)
    {
        await client.PostAsJsonAsync(
            $"api/users/{userId}/agreements",
            new UserAgreementInput
            {
                Type = AgreementType.Unknown,
                AccrualType = AccrualType.ProportianallyEachMonth,
                DateFrom = DateOnly.FromDateTime(DateTime.UtcNow).AddMonths(-9),
                DateTo = DateOnly.FromDateTime(DateTime.UtcNow).AddYears(3),
                FreezeUntilDate = DateOnly.FromDateTime(DateTime.UtcNow).AddMonths(-3),
                DaysPerContractYear = 25,
                IsStaked = true,
                AllowNegativeBalanceInDays = false
            },
            CancellationToken.None);

        await scope.PrepareTransactions();
    }

    private static async Task PrepareTransactions(this IServiceScope scope)
    {
        var sender = scope.ServiceProvider.GetRequiredService<ISender>();

        await sender.Send(
            new CreateAgreementTransactionsRequest.Command { TransactionTime = DateTime.UtcNow },
            CancellationToken.None);
    }

    internal static async Task DeleteAgreements(HttpClient? client, JsonSerializerOptions serializerOptions, int userId)
    {
        var agreementsResponse = await client.GetAsync(
            $"api/users/{userId}/agreements",
            CancellationToken.None);

        var agreements = await agreementsResponse.Content.ReadFromJsonAsync<List<UserAgreementOutput>>(
           serializerOptions,
           CancellationToken.None);

        foreach (var agreement in agreements)
        {
            await client.DeleteAsync(
                $"api/agreements/{agreement.Id}",
                CancellationToken.None);
        }
    }

    internal static async Task SetUserHireDate(this IServiceScope scope, int userId, DateOnly hireDate)
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<ITeamBoardDbContext>();

        var user = await dbContext.TeamBoardUsers
            .FirstOrDefaultAsync(x => x.UserId == userId);

        if (user != null && user.HireDate != hireDate)
        {
            user.HireDate = hireDate;

            dbContext.TeamBoardUsers.Update(user);

            await dbContext.SaveChangesAsync(CancellationToken.None);
        }

        await scope.ServiceProvider
            .GetRequiredService<IConnectionFactory>()
            .GetTransaction()
            .CommitAsync();
    }
}

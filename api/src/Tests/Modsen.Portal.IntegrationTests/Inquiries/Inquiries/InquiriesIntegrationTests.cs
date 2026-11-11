//TODO: FIX TESTS in next story

using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Inquiries;
using Modsen.Portal.Core.Utils;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.Identity.Controllers.Contracts.Input;
using Modsen.Portal.Inquiries.Controllers.Contracts.Input;
using Modsen.Portal.Inquiries.Controllers.Contracts.Output;
using Modsen.Portal.Inquiries.Requests.Agreements.Core.CoreDto;
using Modsen.Portal.Inquiries.Requests.InquiryTemplates.Core.CoreDto;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Identity;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Inquiries;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.TeamboardHttpClientExtensions;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.TimeTracker;
using Modsen.Portal.TimeTrack.Controllers.Contracts.Input;
using Modsen.Portal.TimeTrack.Controllers.Contracts.Output;
namespace Modsen.Portal.IntegrationTests.Inquiries.Inquiries;

[Collection("InquiriesCollection")]
public class InquiriesIntegrationTests
{
    private readonly IntegrationTestFactory _factory;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly CancellationToken _cancellationToken = CancellationToken.None;
    private const InquiryAcceptPolicyScopes CeoCtoScope = InquiryAcceptPolicyScopes.CEO | InquiryAcceptPolicyScopes.CTO;

    public InquiriesIntegrationTests(IntegrationTestFactory factory)
    {
        _factory = factory;
        _jsonOptions = factory.JsonOptions;
    }

    //    private async Task SetUserHireDate(int userId)
    //    {
    //        using var usersScope = _factory.Services.CreateScope();
    //        await usersScope.SetUserHireDate(userId, DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(-9)));
    //    }

    //    [Fact]
    //    public async Task GetInquiries_ResponseWithOk()
    //    {
    //        // Arrange
    //        using var client = _factory.CreateClient();
    //        await client.AuthAsCeo();

    //        // Act
    //        var response = await client.SearchInquiryAsHttpResponseAsync();

    //        // Assert
    //        response.StatusCode.Should().Be(HttpStatusCode.OK);
    //    }

    //    [Fact]
    //    public async Task GetInquiriesByUserId_ResponseWithOk()
    //    {
    //        // Arrange
    //        using var client = _factory.CreateClient();
    //        await client.AuthAsCeo();
    //        const int userId = 3;

    //        // Act
    //        var response = await client.GetInquiryByUserIdAsHttpResponseAsync(userId);

    //        // Assert
    //        response.StatusCode.Should().Be(HttpStatusCode.OK);
    //    }

    //    [Fact]
    //    public async Task GetInquiriesByUserId_ResponseWithForbiddenStatus()
    //    {
    //        // Arrange
    //        using var client = _factory.CreateClient();
    //        await client.AuthAsGroupManager();
    //        const int userId = 3;

    //        // Act
    //        var response = await client.GetInquiryByUserIdAsHttpResponseAsync(userId);

    //        // Assert
    //        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    //    }

    //    [Fact]
    //    public async Task GetInquiriesByApproverId_ResponseWithOk()
    //    {
    //        // Arrange
    //        using var client = _factory.CreateClient();
    //        await client.AuthAsCto();
    //        const int approverId = 3;

    //        // Act
    //        var response = await client.GetInquiryByApproverIdAsHttpResponseAsync(approverId);

    //        // Assert
    //        response.StatusCode.Should().Be(HttpStatusCode.OK);
    //    }

    //    [Fact]
    //    public async Task GetInquiriesByApproverId_ResponseWithForbiddenStatus()
    //    {
    //        // Arrange
    //        using var client = _factory.CreateClient();
    //        await client.AuthAsHeadOfDepartment();
    //        const int approverId = 3;

    //        // Act
    //        var response = await client.GetInquiryByApproverIdAsHttpResponseAsync(approverId);

    //        // Assert
    //        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    //    }

    //    [Fact]
    //    public async Task GetInquiriesByInquiryId_ResponseWithOk()
    //    {
    //        // Arrange
    //        using var client = _factory.CreateClient();

    //        await client.AuthAsCto();

    //        using var scope = _factory.Services.CreateScope();
    //        var context = scope.ServiceProvider.GetRequiredService<IInquiriesDbContext>();

    //        const int userId = 3;
    //        const int templateId = 1;

    //        var template = context.InquiryTemplates.First(x => x.Id == templateId);

    //        var createInquiryInput = new UserInquiryInput
    //        {
    //            InquiryTemplateId = template.Id,
    //            Notes = "",
    //            DateFrom = InquiryDateGenerator.GetDateWithAddOneDay(),
    //            DateTo = InquiryDateGenerator.GetDateWithAddOneDay(),
    //        };

    //        var createInquiryResponse = await client.CreateInquiryAsync(userId, createInquiryInput);

    //        // Act
    //        var response = await client.GetInquiryByIdAsHttpResponseAsync(createInquiryResponse.Id);

    //        // Assert
    //        response.StatusCode.Should().Be(HttpStatusCode.OK);
    //    }

    //    [Fact]
    //    public async Task GetInquiriesByInquiryId_ResponseWithForbiddenStatus()
    //    {
    //        // Arrange
    //        using var client = _factory.CreateClient();

    //        await client.AuthAsCto();

    //        using var scope = _factory.Services.CreateScope();
    //        var context = scope.ServiceProvider.GetRequiredService<IInquiriesDbContext>();

    //        const int userId = 3;
    //        const int templateId = 1;

    //        var template = context.InquiryTemplates.First(x => x.Id == templateId);

    //        if (!await InquiriesTestHelper.CheckIfAgreementsExist(client, _serializerOptions, userId))
    //        {
    //            await SetUserHireDate(userId);
    //            await scope.PrepareAgreementsWithTransactions(client, userId);
    //        }

    //        var createInquiryInput = new UserInquiryInput
    //        {
    //            InquiryTemplateId = template.Id,
    //            Notes = "",
    //            DateFrom = InquiryDateGenerator.GetDateWithAddOneDay(),
    //            DateTo = InquiryDateGenerator.GetDateWithAddOneDay(),
    //        };

    //        var createInquiryResponse = await client.CreateInquiryAsync(userId, createInquiryInput);

    //        await client.AuthAsHeadOfDepartment();

    //        // Act
    //        var response = await client.GetInquiryByIdAsHttpResponseAsync(createInquiryResponse.Id);

    //        // Assert
    //        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    //    }

    //    [Fact]
    //    public async Task CreateInquiry_ShouldCreateOk()
    //    {
    //        // Arrange
    //        using var client = _factory.CreateClient();

    //        await client.AuthAsCeo();

    //        using var scope = _factory.Services.CreateScope();

    //        var context = scope.ServiceProvider.GetRequiredService<IInquiriesDbContext>();

    //        const int userId = 3;
    //        const int templateId = 1;

    //        if (!await InquiriesTestHelper.CheckIfAgreementsExist(client, _serializerOptions, userId))
    //        {
    //            await SetUserHireDate(userId);
    //            await scope.PrepareAgreementsWithTransactions(client, userId);
    //        }

    //        // Act
    //        var template = context.InquiryTemplates.First(x => x.Id == templateId);

    //        var response = await client.CreateInquiryAsync(userId, new UserInquiryInput
    //        {
    //            InquiryTemplateId = template.Id,
    //            Notes = "",
    //            DateFrom = InquiryDateGenerator.GetDateWithAddOneDay(),
    //            DateTo = InquiryDateGenerator.GetDateWithAddOneDay(),
    //        });

    //        // Assert
    //        response.InquiryTemplateId.Should().Be(templateId);
    //    }

    //    [Fact]
    //    public async Task CreateInquiry_WhenDateToIsInvalid_ShouldBadRequest()
    //    {
    //        // Arrange
    //        using var client = _factory.CreateClient();

    //        await client.AuthAsCeo();

    //        using var scope = _factory.Services.CreateScope();
    //        var context = scope.ServiceProvider.GetRequiredService<IInquiriesDbContext>();

    //        const int userId = 3;
    //        const int templateId = 1;

    //        // Act
    //        var template = context.InquiryTemplates.First(x => x.Id == templateId);

    //        var response = await client.CreateInquiryAsHttpResponseMessageAsync(userId, new UserInquiryInput
    //        {
    //            InquiryTemplateId = template.Id,
    //            Notes = "",
    //            DateFrom = InquiryDateGenerator.InquiryDate,
    //        });

    //        // Assert
    //        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    //    }

    //    [Fact]
    //    public async Task CreateInquiry_WhenUserAgreementNotFound_ShouldNotFoundStatus()
    //    {
    //        // Arrange
    //        using var client = _factory.CreateClient();

    //        await client.AuthAsCeo();

    //        using var scope = _factory.Services.CreateScope();
    //        var context = scope.ServiceProvider.GetRequiredService<IInquiriesDbContext>();

    //        const int userId = 3;
    //        const int templateId = 1;

    //        await InquiriesTestHelper.DeleteAgreements(client, _serializerOptions, userId);

    //        // Act
    //        var template = context.InquiryTemplates.First(x => x.Id == templateId);

    //        var response = await client.CreateInquiryAsHttpResponseMessageAsync(userId, new UserInquiryInput
    //        {
    //            InquiryTemplateId = template.Id,
    //            Notes = "",
    //            DateFrom = InquiryDateGenerator.GetDateWithAddOneDay(),
    //            DateTo = InquiryDateGenerator.GetDateWithAddOneDay(),
    //        });

    //        // Assert
    //        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    //    }

    //    [Fact]
    //    public async Task HeadOfDepartmentCreateInquiryToCto_ResponseWithForbiddenStatus()
    //    {
    //        // Arrange
    //        using var client = _factory.CreateClient();

    //        await client.AuthAsHeadOfDepartment();

    //        using var scope = _factory.Services.CreateScope();
    //        var context = scope.ServiceProvider.GetRequiredService<IInquiriesDbContext>();

    //        const int userId = 3;
    //        const int templateId = 1;

    //        // Act
    //        var template = context.InquiryTemplates.First(x => x.Id == templateId);

    //        var response = await client.CreateInquiryAsHttpResponseMessageAsync(userId, new UserInquiryInput
    //        {
    //            InquiryTemplateId = template.Id,
    //            Notes = "",
    //            DateFrom = InquiryDateGenerator.GetDateWithAddOneDay(),
    //            DateTo = InquiryDateGenerator.GetDateWithAddOneDay(),
    //        });

    //        // Assert
    //        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    //    }

    //    [Fact]
    //    public async Task CreateInquiry_WhenInquiryWithSameDateAlreadyExists_ResponseWithConflictStatus()
    //    {
    //        // Arrange
    //        using var client = _factory.CreateClient();

    //        await client.AuthAsCto();

    //        using var scope = _factory.Services.CreateScope();
    //        var context = scope.ServiceProvider.GetRequiredService<IInquiriesDbContext>();

    //        const int userId = 3;
    //        const int templateId = 1;

    //        if (!await InquiriesTestHelper.CheckIfAgreementsExist(client, _serializerOptions, userId))
    //        {
    //            await SetUserHireDate(userId);
    //            await scope.PrepareAgreementsWithTransactions(client, userId);
    //        }

    //        var template = context.InquiryTemplates.First(x => x.Id == templateId);

    //        var createdResponse = await client.CreateInquiryAsHttpResponseMessageAsync(userId, new UserInquiryInput
    //        {
    //            InquiryTemplateId = template.Id,
    //            Notes = "",
    //            DateFrom = InquiryDateGenerator.GetDateWithAddOneDay(),
    //            DateTo = InquiryDateGenerator.GetDateWithAddOneDay(),
    //        });

    //        // Act

    //        var response = await client.CreateInquiryAsHttpResponseMessageAsync(userId, new UserInquiryInput
    //        {
    //            InquiryTemplateId = template.Id,
    //            Notes = "",
    //            DateFrom = InquiryDateGenerator.InquiryDate,
    //            DateTo = InquiryDateGenerator.GetDateWithAddOneDay(),
    //        });

    //        // Assert
    //        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    //    }

    //    [Fact]
    //    public async Task UpdateInquiry_ResponseWithOk()
    //    {
    //        // Arrange
    //        using var client = _factory.CreateClient();

    //        await client.AuthAsCto();

    //        using var scope = _factory.Services.CreateScope();
    //        var context = scope.ServiceProvider.GetRequiredService<IInquiriesDbContext>();

    //        const int userId = 3;
    //        const int templateId = 1;

    //        var template = context.InquiryTemplates.First(x => x.Id == templateId);

    //        if (!await InquiriesTestHelper.CheckIfAgreementsExist(client, _serializerOptions, userId))
    //        {
    //            await SetUserHireDate(userId);
    //            await scope.PrepareAgreementsWithTransactions(client, userId);
    //        }

    //        var createInquiryInput = new UserInquiryInput
    //        {
    //            InquiryTemplateId = template.Id,
    //            Notes = "",
    //            DateFrom = InquiryDateGenerator.GetDateWithAddOneDay(),
    //            DateTo = InquiryDateGenerator.GetDateWithAddOneDay(),
    //        };

    //        var createInquiryResponse = await client.CreateInquiryAsync(userId, createInquiryInput);

    //        var updateInquiryInput = new UpdateUserInquiryInput
    //        {
    //            Notes = "test",
    //            DateFrom = InquiryDateGenerator.GetDateWithAddOneDay(),
    //            DateTo = InquiryDateGenerator.GetDateWithAddOneDay()
    //        };

    //        // Act
    //        var updated = await client.UpdateInquiryAsync(createInquiryResponse.Id, updateInquiryInput);

    //        // Assert
    //        updated!.Notes.Should().Be(updateInquiryInput.Notes);
    //        updated.DateFrom.Should().Be(updateInquiryInput.DateFrom);
    //        updated.DateTo.Should().Be(updateInquiryInput.DateTo);
    //        updated.UserInquiryDates.Count().Should().Be(2);
    //    }

    //    [Fact]
    //    public async Task UpdateInquiry_ResponseWithBadRequestStatus()
    //    {
    //        // Arrange
    //        using var client = _factory.CreateClient();

    //        await client.AuthAsCto();

    //        using var scope = _factory.Services.CreateScope();
    //        var context = scope.ServiceProvider.GetRequiredService<IInquiriesDbContext>();

    //        const int userId = 3;
    //        const int templateId = 1;

    //        var template = context.InquiryTemplates.First(x => x.Id == templateId);

    //        if (!await InquiriesTestHelper.CheckIfAgreementsExist(client, _serializerOptions, userId))
    //        {
    //            await SetUserHireDate(userId);
    //            await scope.PrepareAgreementsWithTransactions(client, userId);
    //        }

    //        var createInquiryInput = new UserInquiryInput
    //        {
    //            InquiryTemplateId = template.Id,
    //            Notes = "",
    //            DateFrom = InquiryDateGenerator.GetDateWithAddOneDay(),
    //            DateTo = InquiryDateGenerator.GetDateWithAddOneDay(),
    //        };

    //        var createInquiryResponse = await client.CreateInquiryAsync(userId, createInquiryInput);

    //        var updateInquiryInput = new UpdateUserInquiryInput
    //        {
    //            Notes = "test",
    //            DateFrom = InquiryDateGenerator.GetDateWithAddOneDay(),
    //            DateTo = InquiryDateGenerator.GetDateWithAddDays(-1)
    //        };
    //        InquiryDateGenerator.GetDateWithAddOneDay();

    //        // Act
    //        var response = await client.UpdateInquiryAsHttpResponseAsync(createInquiryResponse!.Id, updateInquiryInput);

    //        // Assert
    //        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    //    }

    //    [Fact]
    //    public async Task UpdateAcceptedInquiry_ResponseWithForbiddenStatus()
    //    {
    //        // Arrange
    //        using var client = _factory.CreateClient();

    //        await client.AuthAsCto();

    //        using var scope = _factory.Services.CreateScope();
    //        var context = scope.ServiceProvider.GetRequiredService<IInquiriesDbContext>();

    //        const int userId = 3;
    //        const int templateId = 1;

    //        var template = context.InquiryTemplates.First(x => x.Id == templateId);

    //        if (!await InquiriesTestHelper.CheckIfAgreementsExist(client, _serializerOptions, userId))
    //        {
    //            await SetUserHireDate(userId);
    //            await scope.PrepareAgreementsWithTransactions(client, userId);
    //        }

    //        var createInquiryInput = new UserInquiryInput
    //        {
    //            InquiryTemplateId = template.Id,
    //            Notes = "",
    //            DateFrom = InquiryDateGenerator.GetDateWithAddOneDay(),
    //            DateTo = InquiryDateGenerator.GetDateWithAddOneDay(),
    //        };

    //        var createInquiryResponse = await client.CreateInquiryAsync(userId, createInquiryInput);

    //        var updateInquiryInput = new UpdateUserInquiryInput
    //        {
    //            Notes = "test",
    //            DateFrom = InquiryDateGenerator.GetDateWithAddOneDay(),
    //            DateTo = InquiryDateGenerator.GetDateWithAddOneDay()
    //        };

    //        await client.AuthAsCeo();

    //        var acceptResponse = await client.AcceptInquiryAsHttpResponseAsync(
    //            createInquiryResponse!.Id, createInquiryResponse.UserInquiryAccepts.FirstOrDefault()!.Id);

    //        await scope.PrepareAgreementsWithTransactions(client, userId);

    //        await client.AuthAsCto();

    //        // Act
    //        var response = await client.UpdateInquiryAsHttpResponseAsync(createInquiryResponse.Id, updateInquiryInput);

    //        // Assert
    //        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    //    }


    //    [Fact]
    //    public async Task SysAdminUpdateInquiryToCto_ResponseWithForbiddenStatus()
    //    {
    //        // Arrange
    //        using var client = _factory.CreateClient();

    //        await client.AuthAsCto();

    //        using var scope = _factory.Services.CreateScope();
    //        var context = scope.ServiceProvider.GetRequiredService<IInquiriesDbContext>();

    //        const int userId = 3;
    //        const int templateId = 1;

    //        var template = context.InquiryTemplates.First(x => x.Id == templateId);

    //        if (!await InquiriesTestHelper.CheckIfAgreementsExist(client, _serializerOptions, userId))
    //        {
    //            await SetUserHireDate(userId);
    //            await scope.PrepareAgreementsWithTransactions(client, userId);
    //        }

    //        var createInquiryInput = new UserInquiryInput
    //        {
    //            InquiryTemplateId = template.Id,
    //            Notes = "",
    //            DateFrom = InquiryDateGenerator.GetDateWithAddOneDay(),
    //            DateTo = InquiryDateGenerator.GetDateWithAddOneDay(),
    //        };

    //        var createInquiryResponse = await client.CreateInquiryAsync(userId, createInquiryInput);

    //        var updateInquiryInput = new UpdateUserInquiryInput
    //        {
    //            Notes = "test",
    //            DateFrom = InquiryDateGenerator.GetDateWithAddOneDay(),
    //            DateTo = InquiryDateGenerator.GetDateWithAddOneDay()
    //        };

    //        await client.AuthAsAdmin();

    //        // Act
    //        var response = await client.UpdateInquiryAsHttpResponseAsync(createInquiryResponse!.Id, updateInquiryInput);

    //        // Assert
    //        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    //    }

    //    [Fact]
    //    public async Task DeleteInquiry_ResponseWithOk()
    //    {
    //        // Arrange
    //        using var client = _factory.CreateClient();

    //        await client.AuthAsCto();

    //        using var scope = _factory.Services.CreateScope();
    //        var context = scope.ServiceProvider.GetRequiredService<IInquiriesDbContext>();

    //        const int userId = 3;
    //        const int templateId = 1;

    //        var template = context.InquiryTemplates.First(x => x.Id == templateId);

    //        if (!await InquiriesTestHelper.CheckIfAgreementsExist(client, _serializerOptions, userId))
    //        {
    //            await SetUserHireDate(userId);
    //            await scope.PrepareAgreementsWithTransactions(client, userId);
    //        }

    //        var createInquiryInput = new UserInquiryInput
    //        {
    //            InquiryTemplateId = template.Id,
    //            Notes = "",
    //            DateFrom = InquiryDateGenerator.GetDateWithAddOneDay(),
    //            DateTo = InquiryDateGenerator.GetDateWithAddOneDay(),
    //        };

    //        var createInquiryResponse = await client.CreateInquiryAsync(userId, createInquiryInput);

    //        // Act
    //        var deleted = await client.DeleteInquiryAsync(createInquiryResponse.Id);

    //        // Assert
    //        deleted!.Status.Should().Be(UserInquiryAcceptStatus.Cancelled);
    //    }

    //    [Fact]
    //    public async Task EmployeeDeleteCtoInquiry_ResponseWithForbiddenStatus()
    //    {
    //        // Arrange
    //        using var client = _factory.CreateClient();

    //        await client.AuthAsCto();

    //        using var scope = _factory.Services.CreateScope();
    //        var context = scope.ServiceProvider.GetRequiredService<IInquiriesDbContext>();

    //        const int userId = 3;
    //        const int templateId = 1;

    //        var template = context.InquiryTemplates.First(x => x.Id == templateId);

    //        if (!await InquiriesTestHelper.CheckIfAgreementsExist(client, _serializerOptions, userId))
    //        {
    //            await SetUserHireDate(userId);
    //            await scope.PrepareAgreementsWithTransactions(client, userId);
    //        }

    //        var createInquiryInput = new UserInquiryInput
    //        {
    //            InquiryTemplateId = template.Id,
    //            Notes = "",
    //            DateFrom = InquiryDateGenerator.GetDateWithAddOneDay(),
    //            DateTo = InquiryDateGenerator.GetDateWithAddOneDay(),
    //        };

    //        var createInquiryResponse = await client.CreateInquiryAsync(userId, createInquiryInput);

    //        await client.AuthAsEmployee();

    //        // Act
    //        var response = await client.DeleteInquiryAsHttpResponseAsync(createInquiryResponse!.Id);

    //        // Assert
    //        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    //    }

    //    [Fact]
    //    public async Task DeleteAcceptedInquiry_ResponseWithForbiddenStatus()
    //    {
    //        // Arrange
    //        using var client = _factory.CreateClient();

    //        await client.AuthAsCto();

    //        using var scope = _factory.Services.CreateScope();
    //        var context = scope.ServiceProvider.GetRequiredService<IInquiriesDbContext>();

    //        const int userId = 3;
    //        const int templateId = 1;

    //        var template = context.InquiryTemplates.First(x => x.Id == templateId);

    //        if (!await InquiriesTestHelper.CheckIfAgreementsExist(client, _serializerOptions, userId))
    //        {
    //            await SetUserHireDate(userId);
    //            await scope.PrepareAgreementsWithTransactions(client, userId);
    //        }

    //        var createInquiryInput = new UserInquiryInput
    //        {
    //            InquiryTemplateId = template.Id,
    //            Notes = "",
    //            DateFrom = InquiryDateGenerator.GetDateWithAddOneDay(),
    //            DateTo = InquiryDateGenerator.GetDateWithAddOneDay(),
    //        };

    //        var createInquiryResponse = await client.CreateInquiryAsync(userId, createInquiryInput);

    //        await client.AuthAsCeo();

    //        var acceptResponse = await client.AcceptInquiryAsHttpResponseAsync(createInquiryResponse.Id, createInquiryResponse.UserInquiryAccepts.FirstOrDefault().Id);

    //        await scope.PrepareAgreementsWithTransactions(client, userId);

    //        await client.AuthAsCto();

    //        // Act
    //        var response = await client.DeleteInquiryAsHttpResponseAsync(createInquiryResponse.Id);

    //        // Assert
    //        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    //    }

    //    [Fact]
    //    public async Task Get_UserLeavesWithOk()
    //    {
    //        // Arrange
    //        using var client = _factory.CreateClient();

    //        await client.AuthAsCto();

    //        using var scope = _factory.Services.CreateScope();
    //        var context = scope.ServiceProvider.GetRequiredService<IInquiriesDbContext>();

    //        const int userId = 3;
    //        const int templateId = 1;

    //        var template = context.InquiryTemplates.First(x => x.Id == templateId);

    //        if (!await InquiriesTestHelper.CheckIfAgreementsExist(client, _serializerOptions, userId))
    //        {
    //            await SetUserHireDate(userId);
    //            await scope.PrepareAgreementsWithTransactions(client, userId);
    //        }

    //        var createInquiryInput = new UserInquiryInput
    //        {
    //            InquiryTemplateId = template.Id,
    //            Notes = "",
    //            DateFrom = InquiryDateGenerator.GetDateWithAddOneDay(),
    //            DateTo = InquiryDateGenerator.GetDateWithAddOneDay(),
    //        };

    //        var createInquiryResponse = await client.CreateInquiryAsync(userId, createInquiryInput);

    //        await client.AuthAsCeo();

    //        var acceptResponse = await client.AcceptInquiryAsHttpResponseAsync(createInquiryResponse.Id, createInquiryResponse.UserInquiryAccepts.FirstOrDefault().Id);

    //        await scope.PrepareAgreementsWithTransactions(client, userId);

    //        await client.AuthAsCto();

    //        // Act
    //        var leaves = await client.GetLeavesAsync(0, 10, 2025, InquiryDateGenerator.InquiryDate.Month);

    //        // Assert
    //        leaves!.Count.Should().Be(1);
    //    }

    //    [Fact]
    //    public async Task Get_UserLeavesWithBadRequestStatus()
    //    {
    //        // Arrange
    //        using var client = _factory.CreateClient();

    //        await client.AuthAsCto();

    //        // Act
    //        var response = await client.GetLeavesAsHttpResponseAsync(0, 10, 2025, 15);

    //        // Assert
    //        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    //    }

    [Fact]
    public async Task CreateUserInquiryWhenInquiryIsNotDefaultVacationShouldCreateInquiryAndNotCreateAccrualTransactions()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCto();

        var createInput = new InquiryTemplateInput
        {
            Name = Guid.CreateVersion7().ToAlphabetic(),
            Type = InquiryType.Vacation,
            AccrualType = AccrualType.ProportianallyEachMonth,
            ActivityType = ActivityType.NonWorkingPaid,
            IsIncludeHolidays = true,
            IsIncludeWeekends = true,
            IsProhibitOnProbationPeriod = false,
            IsRequireAttachments = false,
            IsSubstitutesToApproveAllowed = false,
            TrackingUnitType = TrackingUnit.Days,
            TransferType = TransferType.Transferred,
            AcceptPolicies =
            [
                new UpdateInquiryTemplateAcceptPolicyDto
                {
                    OrderWeight = 1,
                    Scope = InquiryAcceptPolicyScopes.User,
                    UserId = _factory.AdminUserId!.Value
                }
            ]
        };

        var template = await client.CreateTemplateAsync(createInput);

        var user = await client.CreateUser(new CreateUserInput
        {
            Email = $"{Guid.CreateVersion7().ToAlphabetic()}@modsen-software.com",
            FirstName = Guid.CreateVersion7().ToAlphabetic(),
            LastName = Guid.CreateVersion7().ToAlphabetic(),
            FirstNameRu = "Имя",
            LastNameRu = "Фамилия",
            PatronymicRu = "Отчество",
            Location = LocationType.Minsk,
            Position = PortalRoleNames.Employee,
            DepartmentId = "Net",
            ResourceGroupId = _factory.ResourceGroupId,
            Color = "#123412",
            CountryId = 1,
            RegisteredLocationId = 1,
            ContractDateFrom = DateOnly.FromDateTime(DateTime.UtcNow),
            ContractDateTo = DateOnly.FromDateTime(DateTime.UtcNow).AddYears(1),
        });

        var agreementInput = new CreateAgreementTemplateInput
        {
            Name = "Test Agreement",
            DaysPerContractYear = 24,
            IsDefault = true,
            InquiryTemplateId = template!.Id
        };

        var agreementId = await client.CreateAgreemenTemplateWithResultAsync(agreementInput);

        var userAgreementInquiryAssign = new CreateInquiryAssignInput(AssigneUserIds: [user!.Id], UnassigneUserIds: [],
            template.Id, agreementId, EffectiveFrom: DateOnly.MinValue);

        await client.CreateInquiryAssignAsync(userAgreementInquiryAssign, _jsonOptions);

        var date = InquiryDateGenerator.GetDateWithAddOneDay();
        var createInquiryInput = new UserInquiryInput
        {
            InquiryTemplateId = template.Id,
            Notes = "",
            DateFrom = date,
            DateTo = date.AddDays(12),
            AgreementTemplateId = agreementId
        };

        // Act
        var createdInquiry = await client.CreateInquiryAsync(user!.Id, createInquiryInput);

        var transactionHistoryResponse = await client.GetTransactionInquiryHistoryAsHttpResponse(user!.Id, skip: 0, top: 20);

        // Assert
        createdInquiry.Should().NotBeNull();
        createdInquiry.DateFrom.Should().Be(createdInquiry.DateFrom);
        createdInquiry.DateTo.Should().Be(createdInquiry.DateTo);

        transactionHistoryResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var transactionHistory = await transactionHistoryResponse.Content
            .ReadFromJsonAsync<PageableListOfItems<UserAbsenceHistoryCoreDto>>(
                _jsonOptions, _cancellationToken);

        transactionHistory!.Items.Should().NotContain(x =>
            x.InquiryTemplateId == template.Id && x.TransactionType == UserAgreementTransactionType.Accrual);
    }

    [Fact]
    public async Task CreateUserInquiryForEmployeeWhenAcceptPoliciesAreSpecifiedShouldReturnValidAccepts()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCto();

        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        var user = await client.CreateUser(new CreateUserInput
        {
            Email = $"{Guid.CreateVersion7().ToAlphabetic()}@modsen-software.com",
            FirstName = Guid.CreateVersion7().ToAlphabetic(),
            LastName = Guid.CreateVersion7().ToAlphabetic(),
            FirstNameRu = "Имя",
            LastNameRu = "Фамилия",
            PatronymicRu = "Отчество",
            Location = LocationType.Minsk,
            Position = PortalRoleNames.Employee,
            DepartmentId = "Net",
            ResourceGroupId = _factory.ResourceGroupId,
            Color = "#123412",
            CountryId = 1,
            RegisteredLocationId = 1
        });

        var template = await client.CreateTemplateAsync(new InquiryTemplateInput
        {
            AcceptPolicies = new List<UpdateInquiryTemplateAcceptPolicyDto>
            {
                new UpdateInquiryTemplateAcceptPolicyDto
                {
                    OrderWeight = 1,
                    Scope = InquiryAcceptPolicyScopes.GroupManager
                },
                new UpdateInquiryTemplateAcceptPolicyDto
                {
                    OrderWeight = 2,
                    Scope = InquiryAcceptPolicyScopes.HeadOfDepartment
                }
            },
            AccrualType = null,
            ActivityType = ActivityType.NonWorkingPaid,
            Comment = null,
            IsIncludeHolidays = true,
            IsIncludeWeekends = true,
            IsProhibitOnProbationPeriod = false,
            IsRequireAttachments = false,
            IsSubstitutesToApproveAllowed = false,
            Name = Guid.CreateVersion7().ToAlphabetic(),
            TrackingUnitType = TrackingUnit.Days,
            TransferType = null,
            Type = InquiryType.SickLeave
        });

        await client.CreateInquiryAssignAsync(
            new CreateInquiryAssignInput(AssigneUserIds: [user!.Id], UnassigneUserIds: [],
                template!.Id, null, today), _jsonOptions);

        var expected = new List<UserInquiryAcceptOutput>
        {
            new UserInquiryAcceptOutput
            {
                AcceptUser = new InquiriesUserOutput(_factory.GroupManagerId!.Value, "", "", "", null, null,
                    null, null, null, null),
                Status = UserInquiryAcceptStatus.WaitingForApprove,
            },
            new UserInquiryAcceptOutput
            {
                AcceptUser = new InquiriesUserOutput(_factory.HeadOfDepartmentUserId!.Value, "", "", "", null, null,
                    null, null, null, null),
                Status = UserInquiryAcceptStatus.Pending,
            }
        };

        // Act
        var inquiry = await client.CreateInquiryAsync(user!.Id, new UserInquiryInput
        {
            AgreementTemplateId = null,
            DateFrom = today,
            DateTo = today,
            Files = [],
            Hours = null,
            InquiryTemplateId = template!.Id,
            Notes = null,
        });

        // Assert
        inquiry!.UserInquiryAccepts.Should().NotBeNull();
        inquiry.UserInquiryAccepts.Should().BeEquivalentTo(expected,
            config => config
                .Including(x => x.Status)
                .Including(x => x.AcceptUser!.Id)
                .WithStrictOrdering());
    }

    [Fact]
    public async Task CreateInquiryForEmployeeWhenEmployeeAssignedToProjectsWithManagersShouldReturnProjectManagers()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCto();

        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        var user = await client.CreateUser(new CreateUserInput
        {
            Email = $"{Guid.CreateVersion7().ToAlphabetic()}@modsen-software.com",
            FirstName = Guid.CreateVersion7().ToAlphabetic(),
            LastName = Guid.CreateVersion7().ToAlphabetic(),
            FirstNameRu = "Имя",
            LastNameRu = "Фамилия",
            PatronymicRu = "Отчество",
            Location = LocationType.Minsk,
            Position = PortalRoleNames.Employee,
            DepartmentId = "Net",
            ResourceGroupId = _factory.ResourceGroupId,
            Color = "#123412",
            CountryId = 1,
            RegisteredLocationId = 1
        });

        var projectResponse = await client.CreateProjectAsHttpResponse(new CreateProjectInput
        {
            ClientId = _factory.ModsenClientId!.Value,
            IsBillable = true,
            IsPublic = false,
            Name = Guid.CreateVersion7().ToAlphabetic(),
            ManagerId = _factory.PmoHeadOfDepartmentUserId!.Value,
        });

        var project = await projectResponse.Content.ReadFromJsonAsync<ProjectOutput>(_cancellationToken);

        var projectUser = await client.CreateProjectAssignment(user!.Id, project!.Id, new CreateUserProjectAssignmentInput(
            today, UserOnProjectTo: null, IsTimeOnProjectBillable: true, Fte: 1m, UserOnProjectSeniorityLevelId: "TraineeL2"
        ));

        var template = await client.CreateTemplateAsync(new InquiryTemplateInput
        {
            AcceptPolicies = new List<UpdateInquiryTemplateAcceptPolicyDto>
            {
                new UpdateInquiryTemplateAcceptPolicyDto
                {
                    OrderWeight = 1,
                    Scope = InquiryAcceptPolicyScopes.ProjectManager
                }
            },
            AccrualType = null,
            ActivityType = ActivityType.NonWorkingPaid,
            Comment = null,
            IsIncludeHolidays = true,
            IsIncludeWeekends = true,
            IsProhibitOnProbationPeriod = false,
            IsRequireAttachments = false,
            IsSubstitutesToApproveAllowed = false,
            Name = Guid.CreateVersion7().ToAlphabetic(),
            TrackingUnitType = TrackingUnit.Days,
            TransferType = null,
            Type = InquiryType.SickLeave
        });

        await client.CreateInquiryAssignAsync(
            new CreateInquiryAssignInput(AssigneUserIds: [user!.Id], UnassigneUserIds: [],
                template!.Id, null, today), _jsonOptions);

        var expected = new List<UserInquiryAcceptOutput>
        {
            new UserInquiryAcceptOutput
            {
                AcceptUser = new InquiriesUserOutput(_factory.PmoHeadOfDepartmentUserId!.Value, "", "", "", null, null,
                    null, null, null, null),
                Status = UserInquiryAcceptStatus.WaitingForApprove,
            },
        };

        // Act
        var inquiry = await client.CreateInquiryAsync(user!.Id, new UserInquiryInput
        {
            AgreementTemplateId = null,
            DateFrom = today,
            DateTo = today,
            Files = [],
            Hours = null,
            InquiryTemplateId = template!.Id,
            Notes = null,
        });

        // Assert
        inquiry!.UserInquiryAccepts.Should().NotBeNull();
        inquiry.UserInquiryAccepts.Should().BeEquivalentTo(expected,
            config => config
                .Including(x => x.Status)
                .Including(x => x.AcceptUser!.Id)
                .WithStrictOrdering());
    }

    [Fact]
    public async Task CreateInquiryForEmployeeWhenGmIsSpecifiedInPolicyButDoesNotExistShouldNotReturnGm()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCto();

        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        var user = await client.CreateUser(new CreateUserInput
        {
            Email = $"{Guid.CreateVersion7().ToAlphabetic()}@modsen-software.com",
            FirstName = Guid.CreateVersion7().ToAlphabetic(),
            LastName = Guid.CreateVersion7().ToAlphabetic(),
            FirstNameRu = "Имя",
            LastNameRu = "Фамилия",
            PatronymicRu = "Отчество",
            Location = LocationType.Minsk,
            Position = PortalRoleNames.Employee,
            DepartmentId = "Net",
            ResourceGroupId = null,
            Color = "#123412",
            CountryId = 1,
            RegisteredLocationId = 1
        });

        var projectResponse = await client.CreateProjectAsHttpResponse(new CreateProjectInput
        {
            ClientId = _factory.ModsenClientId!.Value,
            IsBillable = true,
            IsPublic = false,
            Name = Guid.CreateVersion7().ToAlphabetic(),
            ManagerId = _factory.PmoHeadOfDepartmentUserId!.Value,
        });

        var project = await projectResponse.Content.ReadFromJsonAsync<ProjectOutput>(_cancellationToken);

        var projectUser = await client.CreateProjectAssignment(user!.Id, project!.Id, new CreateUserProjectAssignmentInput(
            today, UserOnProjectTo: null, IsTimeOnProjectBillable: true, Fte: 1m, UserOnProjectSeniorityLevelId: "TraineeL2"
        ));

        var template = await client.CreateTemplateAsync(new InquiryTemplateInput
        {
            AcceptPolicies = new List<UpdateInquiryTemplateAcceptPolicyDto>
            {
                new UpdateInquiryTemplateAcceptPolicyDto
                {
                    OrderWeight = 1,
                    Scope = InquiryAcceptPolicyScopes.GroupManager,
                },
                new UpdateInquiryTemplateAcceptPolicyDto
                {
                    OrderWeight = 2,
                    Scope = InquiryAcceptPolicyScopes.HeadOfDepartment,
                },
            },
            AccrualType = null,
            ActivityType = ActivityType.NonWorkingPaid,
            Comment = null,
            IsIncludeHolidays = true,
            IsIncludeWeekends = true,
            IsProhibitOnProbationPeriod = false,
            IsRequireAttachments = false,
            IsSubstitutesToApproveAllowed = false,
            Name = Guid.CreateVersion7().ToAlphabetic(),
            TrackingUnitType = TrackingUnit.Days,
            TransferType = null,
            Type = InquiryType.SickLeave
        });

        await client.CreateInquiryAssignAsync(
            new CreateInquiryAssignInput(AssigneUserIds: [user!.Id], UnassigneUserIds: [],
                template!.Id, null, today), _jsonOptions);

        var expected = new List<UserInquiryAcceptOutput>
        {
            new UserInquiryAcceptOutput
            {
                AcceptUser = new InquiriesUserOutput(_factory.HeadOfDepartmentUserId!.Value, "", "", "", null, null,
                    null, null, null, null),
                Status = UserInquiryAcceptStatus.WaitingForApprove,
            }
        };

        // Act
        var inquiry = await client.CreateInquiryAsync(user!.Id, new UserInquiryInput
        {
            AgreementTemplateId = null,
            DateFrom = today,
            DateTo = today,
            Files = [],
            Hours = null,
            InquiryTemplateId = template!.Id,
            Notes = null,
        });

        // Assert
        inquiry!.UserInquiryAccepts.Should().NotBeNull();
        inquiry.UserInquiryAccepts.Should().BeEquivalentTo(expected,
            config => config
                .Including(x => x.Status)
                .Including(x => x.AcceptUser!.Id)
                .WithStrictOrdering());
    }

    [Fact]
    public async Task CreateInquiryForEmployeeWhenHodIsSpecifiedInPolicyButDoesNotExistShouldReturnCtoInsteadOfHod()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCto();

        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        var user = await client.CreateUser(new CreateUserInput
        {
            Email = $"{Guid.CreateVersion7().ToAlphabetic()}@modsen-software.com",
            FirstName = Guid.CreateVersion7().ToAlphabetic(),
            LastName = Guid.CreateVersion7().ToAlphabetic(),
            FirstNameRu = "Имя",
            LastNameRu = "Фамилия",
            PatronymicRu = "Отчество",
            Location = LocationType.Minsk,
            Position = PortalRoleNames.Employee,
            DepartmentId = "Java",
            ResourceGroupId = null,
            Color = "#123412",
            CountryId = 1,
            RegisteredLocationId = 1
        });

        var projectResponse = await client.CreateProjectAsHttpResponse(new CreateProjectInput
        {
            ClientId = _factory.ModsenClientId!.Value,
            IsBillable = true,
            IsPublic = false,
            Name = Guid.CreateVersion7().ToAlphabetic(),
            ManagerId = _factory.PmoHeadOfDepartmentUserId!.Value,
        });

        var project = await projectResponse.Content.ReadFromJsonAsync<ProjectOutput>(_cancellationToken);

        var projectUser = await client.CreateProjectAssignment(user!.Id, project!.Id, new CreateUserProjectAssignmentInput(
            today, UserOnProjectTo: null, IsTimeOnProjectBillable: true, Fte: 1m, UserOnProjectSeniorityLevelId: "TraineeL2"
        ));

        var template = await client.CreateTemplateAsync(new InquiryTemplateInput
        {
            AcceptPolicies = new List<UpdateInquiryTemplateAcceptPolicyDto>
            {
                new UpdateInquiryTemplateAcceptPolicyDto
                {
                    OrderWeight = 1,
                    Scope = InquiryAcceptPolicyScopes.HeadOfDepartment,
                }
            },
            AccrualType = null,
            ActivityType = ActivityType.NonWorkingPaid,
            Comment = null,
            IsIncludeHolidays = true,
            IsIncludeWeekends = true,
            IsProhibitOnProbationPeriod = false,
            IsRequireAttachments = false,
            IsSubstitutesToApproveAllowed = false,
            Name = Guid.CreateVersion7().ToAlphabetic(),
            TrackingUnitType = TrackingUnit.Days,
            TransferType = null,
            Type = InquiryType.SickLeave
        });

        await client.CreateInquiryAssignAsync(
            new CreateInquiryAssignInput(AssigneUserIds: [user!.Id], UnassigneUserIds: [],
                template!.Id, null, today), _jsonOptions);

        var expected = new List<UserInquiryAcceptOutput>
        {
            new UserInquiryAcceptOutput
            {
                AcceptUser = new InquiriesUserOutput(_factory.CtoUserId!.Value, "", "", "", null, null,
                    null, null, null, null),
                Status = UserInquiryAcceptStatus.WaitingForApprove,
            }
        };

        // Act
        var inquiry = await client.CreateInquiryAsync(user!.Id, new UserInquiryInput
        {
            AgreementTemplateId = null,
            DateFrom = today,
            DateTo = today,
            Files = [],
            Hours = null,
            InquiryTemplateId = template!.Id,
            Notes = null,
        });

        // Assert
        inquiry!.UserInquiryAccepts.Should().NotBeNull();
        inquiry.UserInquiryAccepts.Should().BeEquivalentTo(expected,
            config => config
                .Including(x => x.Status)
                .Including(x => x.AcceptUser!.Id)
                .WithStrictOrdering());
    }

    [Fact]
    public async Task GetInquiryByIdWhenInquiryOpenAndOtherInquiriesCreatedShouldBalanceDaysBeUpdated()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCto();

        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        var user = await client.CreateUser(new CreateUserInput
        {
            Email = $"{Guid.CreateVersion7().ToAlphabetic()}@modsen-software.com",
            FirstName = Guid.CreateVersion7().ToAlphabetic(),
            LastName = Guid.CreateVersion7().ToAlphabetic(),
            FirstNameRu = "Имя",
            LastNameRu = "Фамилия",
            PatronymicRu = "Отчество",
            Location = LocationType.Minsk,
            Position = PortalRoleNames.Employee,
            DepartmentId = "Net",
            ResourceGroupId = _factory.ResourceGroupId,
            Color = "#123412",
            CountryId = 1,
            RegisteredLocationId = 1
        });

        var template = await client.CreateTemplateAsync(new InquiryTemplateInput
        {
            AcceptPolicies = new List<UpdateInquiryTemplateAcceptPolicyDto>
            {
                new UpdateInquiryTemplateAcceptPolicyDto
                {
                    OrderWeight = 2,
                    Scope = InquiryAcceptPolicyScopes.HeadOfDepartment
                }
            },
            AccrualType = null,
            ActivityType = ActivityType.NonWorkingPaid,
            Comment = null,
            IsIncludeHolidays = true,
            IsIncludeWeekends = true,
            IsProhibitOnProbationPeriod = false,
            IsRequireAttachments = false,
            IsSubstitutesToApproveAllowed = false,
            Name = Guid.CreateVersion7().ToAlphabetic(),
            TrackingUnitType = TrackingUnit.Days,
            TransferType = null,
            Type = InquiryType.SickLeave
        });

        await client.CreateInquiryAssignAsync(
            new CreateInquiryAssignInput(AssigneUserIds: [user!.Id], UnassigneUserIds: [],
                template!.Id, null, today), _jsonOptions);

        var inquiry = await client.CreateInquiryAsync(user!.Id, new UserInquiryInput
        {
            AgreementTemplateId = null,
            DateFrom = today,
            DateTo = today,
            Files = [],
            Hours = null,
            InquiryTemplateId = template!.Id,
            Notes = null,
        });

        var expectedBalanceDays = 2;

        var inquiry2 = await client.CreateInquiryAsync(user!.Id, new UserInquiryInput
        {
            AgreementTemplateId = null,
            DateFrom = today.AddDays(1),
            DateTo = today.AddDays(2),
            Files = [],
            Hours = null,
            InquiryTemplateId = template!.Id,
            Notes = null,
        });

        await client.AcceptInquiryAsHttpResponseAsync(inquiry2!.Id);

        // Act
        var result = await client.GetInquiryByIdAsync(inquiry!.Id);

        // Assert
        result!.BalanceDays.Should().Be(expectedBalanceDays);
        result!.BalanceHours.Should().BeNull();
        result!.RemainingDays.Should().BeNull();
    }

    [Fact]
    public async Task GetInquiryByIdWhenInquiryApprovedAndOtherInquiriesCreatedShouldBalanceDaysBeNotUpdated()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCto();

        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        var user = await client.CreateUser(new CreateUserInput
        {
            Email = $"{Guid.CreateVersion7().ToAlphabetic()}@modsen-software.com",
            FirstName = Guid.CreateVersion7().ToAlphabetic(),
            LastName = Guid.CreateVersion7().ToAlphabetic(),
            FirstNameRu = "Имя",
            LastNameRu = "Фамилия",
            PatronymicRu = "Отчество",
            Location = LocationType.Minsk,
            Position = PortalRoleNames.Employee,
            DepartmentId = "Net",
            ResourceGroupId = _factory.ResourceGroupId,
            Color = "#123412",
            CountryId = 1,
            RegisteredLocationId = 1
        });

        var template = await client.CreateTemplateAsync(new InquiryTemplateInput
        {
            AcceptPolicies = new List<UpdateInquiryTemplateAcceptPolicyDto>
            {
                new UpdateInquiryTemplateAcceptPolicyDto
                {
                    OrderWeight = 2,
                    Scope = InquiryAcceptPolicyScopes.HeadOfDepartment
                }
            },
            AccrualType = null,
            ActivityType = ActivityType.NonWorkingPaid,
            Comment = null,
            IsIncludeHolidays = true,
            IsIncludeWeekends = true,
            IsProhibitOnProbationPeriod = false,
            IsRequireAttachments = false,
            IsSubstitutesToApproveAllowed = false,
            Name = Guid.CreateVersion7().ToAlphabetic(),
            TrackingUnitType = TrackingUnit.Days,
            TransferType = null,
            Type = InquiryType.SickLeave
        });

        await client.CreateInquiryAssignAsync(
            new CreateInquiryAssignInput(AssigneUserIds: [user!.Id], UnassigneUserIds: [],
                template!.Id, null, today), _jsonOptions);

        var inquiry = await client.CreateInquiryAsync(user!.Id, new UserInquiryInput
        {
            AgreementTemplateId = null,
            DateFrom = today,
            DateTo = today,
            Files = [],
            Hours = null,
            InquiryTemplateId = template!.Id,
            Notes = null,
        });

        await client.AcceptInquiryAsHttpResponseAsync(inquiry!.Id);

        var expectedBalanceDays = (await client.GetInquiryByIdAsync(inquiry!.Id))!.BalanceDays;

        var inquiry2 = await client.CreateInquiryAsync(user!.Id, new UserInquiryInput
        {
            AgreementTemplateId = null,
            DateFrom = today.AddDays(1),
            DateTo = today.AddDays(20),
            Files = [],
            Hours = null,
            InquiryTemplateId = template!.Id,
            Notes = null,
        });

        await client.AcceptInquiryAsHttpResponseAsync(inquiry2!.Id);

        // Act
        var result = await client.GetInquiryByIdAsync(inquiry!.Id);

        // Assert
        result!.BalanceDays.Should().Be(expectedBalanceDays);
        result!.BalanceHours.Should().BeNull();
        result!.RemainingDays.Should().BeNull();
    }

    [Fact]
    public async Task GetInquiryByIdWhenInquiryRejectedAndOtherInquiriesCreatedShouldBalanceDaysBeNotUpdated()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCto();

        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        var user = await client.CreateUser(new CreateUserInput
        {
            Email = $"{Guid.CreateVersion7().ToAlphabetic()}@modsen-software.com",
            FirstName = Guid.CreateVersion7().ToAlphabetic(),
            LastName = Guid.CreateVersion7().ToAlphabetic(),
            FirstNameRu = "Имя",
            LastNameRu = "Фамилия",
            PatronymicRu = "Отчество",
            Location = LocationType.Minsk,
            Position = PortalRoleNames.Employee,
            DepartmentId = "Net",
            ResourceGroupId = _factory.ResourceGroupId,
            Color = "#123412",
            CountryId = 1,
            RegisteredLocationId = 1
        });

        var template = await client.CreateTemplateAsync(new InquiryTemplateInput
        {
            AcceptPolicies = new List<UpdateInquiryTemplateAcceptPolicyDto>
            {
                new UpdateInquiryTemplateAcceptPolicyDto
                {
                    OrderWeight = 2,
                    Scope = InquiryAcceptPolicyScopes.HeadOfDepartment
                }
            },
            AccrualType = null,
            ActivityType = ActivityType.NonWorkingPaid,
            Comment = null,
            IsIncludeHolidays = true,
            IsIncludeWeekends = true,
            IsProhibitOnProbationPeriod = false,
            IsRequireAttachments = false,
            IsSubstitutesToApproveAllowed = false,
            Name = Guid.CreateVersion7().ToAlphabetic(),
            TrackingUnitType = TrackingUnit.Days,
            TransferType = null,
            Type = InquiryType.SickLeave
        });

        await client.CreateInquiryAssignAsync(
            new CreateInquiryAssignInput(AssigneUserIds: [user!.Id], UnassigneUserIds: [],
                template!.Id, null, today), _jsonOptions);

        var inquiry = await client.CreateInquiryAsync(user!.Id, new UserInquiryInput
        {
            AgreementTemplateId = null,
            DateFrom = today,
            DateTo = today,
            Files = [],
            Hours = null,
            InquiryTemplateId = template!.Id,
            Notes = null,
        });

        await client.RejectInquiryAsHttpResponseAsync(inquiry!.Id);

        var expectedBalanceDays = (await client.GetInquiryByIdAsync(inquiry!.Id))!.BalanceDays;

        var inquiry2 = await client.CreateInquiryAsync(user!.Id, new UserInquiryInput
        {
            AgreementTemplateId = null,
            DateFrom = today.AddDays(1),
            DateTo = today.AddDays(20),
            Files = [],
            Hours = null,
            InquiryTemplateId = template!.Id,
            Notes = null,
        });

        await client.AcceptInquiryAsHttpResponseAsync(inquiry2!.Id);

        // Act
        var result = await client.GetInquiryByIdAsync(inquiry!.Id);

        // Assert
        result!.BalanceDays.Should().Be(expectedBalanceDays);
        result!.BalanceHours.Should().BeNull();
        result!.RemainingDays.Should().BeNull();
    }


    [Fact]
    public async Task GetInquiryByIdWhenInquiryWithAgreementOpenAndOtherInquiriesCreatedShouldRemainingDaysBeUpdated()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCto();

        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        var user = await client.CreateUser(new CreateUserInput
        {
            Email = $"{Guid.CreateVersion7().ToAlphabetic()}@modsen-software.com",
            FirstName = Guid.CreateVersion7().ToAlphabetic(),
            LastName = Guid.CreateVersion7().ToAlphabetic(),
            FirstNameRu = "Имя",
            LastNameRu = "Фамилия",
            PatronymicRu = "Отчество",
            Location = LocationType.Minsk,
            Position = PortalRoleNames.Employee,
            DepartmentId = "Net",
            ResourceGroupId = _factory.ResourceGroupId,
            Color = "#123412",
            CountryId = 1,
            RegisteredLocationId = 1
        });

        var template = await client.CreateTemplateAsync(new InquiryTemplateInput
        {
            AcceptPolicies = new List<UpdateInquiryTemplateAcceptPolicyDto>
            {
                new UpdateInquiryTemplateAcceptPolicyDto
                {
                    OrderWeight = 2,
                    Scope = InquiryAcceptPolicyScopes.HeadOfDepartment
                }
            },
            AccrualType = AccrualType.ProportianallyEachMonth,
            ActivityType = ActivityType.NonWorkingPaid,
            Comment = null,
            IsIncludeHolidays = true,
            IsIncludeWeekends = true,
            IsProhibitOnProbationPeriod = false,
            IsRequireAttachments = false,
            IsSubstitutesToApproveAllowed = false,
            Name = Guid.CreateVersion7().ToAlphabetic(),
            TrackingUnitType = TrackingUnit.Days,
            TransferType = TransferType.Transferred,
            Type = InquiryType.Vacation
        });

        var agreementTemplateId = await client.CreateAgreemenTemplateWithResultAsync(new CreateAgreementTemplateInput
        {
            Name = "Name",
            DaysPerContractYear = 25,
            InquiryTemplateId = template.Id,
            IsDefault = false
        });

        await client.CreateInquiryAssignAsync(
            new CreateInquiryAssignInput(AssigneUserIds: [user!.Id], UnassigneUserIds: [],
                template!.Id, agreementTemplateId, today), _jsonOptions);

        var inquiry = await client.CreateInquiryAsync(user!.Id, new UserInquiryInput
        {
            AgreementTemplateId = agreementTemplateId,
            DateFrom = today,
            DateTo = today,
            Files = [],
            Hours = null,
            InquiryTemplateId = template!.Id,
            Notes = null,
        });

        var expectedRemainingDays = -3m;

        var inquiry2 = await client.CreateInquiryAsync(user!.Id, new UserInquiryInput
        {
            AgreementTemplateId = agreementTemplateId,
            DateFrom = today.AddDays(1),
            DateTo = today.AddDays(2),
            Files = [],
            Hours = null,
            InquiryTemplateId = template!.Id,
            Notes = null,
        });

        await client.AcceptInquiryAsHttpResponseAsync(inquiry2!.Id);

        // Act
        var result = await client.GetInquiryByIdAsync(inquiry!.Id);

        // Assert
        result!.BalanceDays.Should().BeNull();
        result!.BalanceHours.Should().BeNull();
        result!.RemainingDays.Should().Be(expectedRemainingDays);
    }

    [Fact]
    public async Task GetInquiryByIdWhenInquiryWithAgreementApprovedAndOtherInquiriesCreatedShouldRemainingDaysBeNotUpdated()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCto();

        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        var user = await client.CreateUser(new CreateUserInput
        {
            Email = $"{Guid.CreateVersion7().ToAlphabetic()}@modsen-software.com",
            FirstName = Guid.CreateVersion7().ToAlphabetic(),
            LastName = Guid.CreateVersion7().ToAlphabetic(),
            FirstNameRu = "Имя",
            LastNameRu = "Фамилия",
            PatronymicRu = "Отчество",
            Location = LocationType.Minsk,
            Position = PortalRoleNames.Employee,
            DepartmentId = "Net",
            ResourceGroupId = _factory.ResourceGroupId,
            Color = "#123412",
            CountryId = 1,
            RegisteredLocationId = 1
        });

        var template = await client.CreateTemplateAsync(new InquiryTemplateInput
        {
            AcceptPolicies = new List<UpdateInquiryTemplateAcceptPolicyDto>
            {
                new UpdateInquiryTemplateAcceptPolicyDto
                {
                    OrderWeight = 2,
                    Scope = InquiryAcceptPolicyScopes.HeadOfDepartment
                }
            },
            AccrualType = AccrualType.ProportianallyEachMonth,
            ActivityType = ActivityType.NonWorkingPaid,
            Comment = null,
            IsIncludeHolidays = true,
            IsIncludeWeekends = true,
            IsProhibitOnProbationPeriod = false,
            IsRequireAttachments = false,
            IsSubstitutesToApproveAllowed = false,
            Name = Guid.CreateVersion7().ToAlphabetic(),
            TrackingUnitType = TrackingUnit.Days,
            TransferType = TransferType.Transferred,
            Type = InquiryType.Vacation
        });

        var agreementTemplateId = await client.CreateAgreemenTemplateWithResultAsync(new CreateAgreementTemplateInput
        {
            Name = "Name",
            DaysPerContractYear = 25,
            InquiryTemplateId = template.Id,
            IsDefault = false
        });

        await client.CreateInquiryAssignAsync(
            new CreateInquiryAssignInput(AssigneUserIds: [user!.Id], UnassigneUserIds: [],
                template!.Id, agreementTemplateId, today), _jsonOptions);

        var inquiry = await client.CreateInquiryAsync(user!.Id, new UserInquiryInput
        {
            AgreementTemplateId = agreementTemplateId,
            DateFrom = today,
            DateTo = today,
            Files = [],
            Hours = null,
            InquiryTemplateId = template!.Id,
            Notes = null,
        });

        await client.AcceptInquiryAsHttpResponseAsync(inquiry!.Id);

        var expectedRemainingDays = -1m;

        var inquiry2 = await client.CreateInquiryAsync(user!.Id, new UserInquiryInput
        {
            AgreementTemplateId = agreementTemplateId,
            DateFrom = today.AddDays(1),
            DateTo = today.AddDays(2),
            Files = [],
            Hours = null,
            InquiryTemplateId = template!.Id,
            Notes = null,
        });

        await client.AcceptInquiryAsHttpResponseAsync(inquiry2!.Id);

        // Act
        var result = await client.GetInquiryByIdAsync(inquiry!.Id);

        // Assert
        result!.BalanceDays.Should().BeNull();
        result!.BalanceHours.Should().BeNull();
        result!.RemainingDays.Should().Be(expectedRemainingDays);
    }

    [Fact]
    public async Task GetInquiryByIdWhenInquiryWithAgreementRejectedAndOtherInquiriesCreatedShouldRemainingDaysBeNotUpdated()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCto();

        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        var user = await client.CreateUser(new CreateUserInput
        {
            Email = $"{Guid.CreateVersion7().ToAlphabetic()}@modsen-software.com",
            FirstName = Guid.CreateVersion7().ToAlphabetic(),
            LastName = Guid.CreateVersion7().ToAlphabetic(),
            FirstNameRu = "Имя",
            LastNameRu = "Фамилия",
            PatronymicRu = "Отчество",
            Location = LocationType.Minsk,
            Position = PortalRoleNames.Employee,
            DepartmentId = "Net",
            ResourceGroupId = _factory.ResourceGroupId,
            Color = "#123412",
            CountryId = 1,
            RegisteredLocationId = 1
        });

        var template = await client.CreateTemplateAsync(new InquiryTemplateInput
        {
            AcceptPolicies = new List<UpdateInquiryTemplateAcceptPolicyDto>
            {
                new UpdateInquiryTemplateAcceptPolicyDto
                {
                    OrderWeight = 2,
                    Scope = InquiryAcceptPolicyScopes.HeadOfDepartment
                }
            },
            AccrualType = AccrualType.ProportianallyEachMonth,
            ActivityType = ActivityType.NonWorkingPaid,
            Comment = null,
            IsIncludeHolidays = true,
            IsIncludeWeekends = true,
            IsProhibitOnProbationPeriod = false,
            IsRequireAttachments = false,
            IsSubstitutesToApproveAllowed = false,
            Name = Guid.CreateVersion7().ToAlphabetic(),
            TrackingUnitType = TrackingUnit.Days,
            TransferType = TransferType.Transferred,
            Type = InquiryType.Vacation
        });

        var agreementTemplateId = await client.CreateAgreemenTemplateWithResultAsync(new CreateAgreementTemplateInput
        {
            Name = "Name",
            DaysPerContractYear = 25,
            InquiryTemplateId = template.Id,
            IsDefault = false
        });

        await client.CreateInquiryAssignAsync(
            new CreateInquiryAssignInput(AssigneUserIds: [user!.Id], UnassigneUserIds: [],
                template!.Id, agreementTemplateId, today), _jsonOptions);

        var inquiry = await client.CreateInquiryAsync(user!.Id, new UserInquiryInput
        {
            AgreementTemplateId = agreementTemplateId,
            DateFrom = today,
            DateTo = today,
            Files = [],
            Hours = null,
            InquiryTemplateId = template!.Id,
            Notes = null,
        });

        await client.RejectInquiryAsHttpResponseAsync(inquiry!.Id);

        var expectedRemainingDays = 0m;

        var inquiry2 = await client.CreateInquiryAsync(user!.Id, new UserInquiryInput
        {
            AgreementTemplateId = agreementTemplateId,
            DateFrom = today.AddDays(1),
            DateTo = today.AddDays(2),
            Files = [],
            Hours = null,
            InquiryTemplateId = template!.Id,
            Notes = null,
        });

        await client.AcceptInquiryAsHttpResponseAsync(inquiry2!.Id);

        // Act
        var result = await client.GetInquiryByIdAsync(inquiry!.Id);

        // Assert
        result!.BalanceDays.Should().BeNull();
        result!.BalanceHours.Should().BeNull();
        result!.RemainingDays.Should().Be(expectedRemainingDays);
    }

    [Fact]
    public async Task GetInquiryByIdWhenInquiryOpenAndOtherInquiriesCreatedShouldBalanceHoursBeUpdated()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCto();

        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        var user = await client.CreateUser(new CreateUserInput
        {
            Email = $"{Guid.CreateVersion7().ToAlphabetic()}@modsen-software.com",
            FirstName = Guid.CreateVersion7().ToAlphabetic(),
            LastName = Guid.CreateVersion7().ToAlphabetic(),
            FirstNameRu = "Имя",
            LastNameRu = "Фамилия",
            PatronymicRu = "Отчество",
            Location = LocationType.Minsk,
            Position = PortalRoleNames.Employee,
            DepartmentId = "Net",
            ResourceGroupId = _factory.ResourceGroupId,
            Color = "#123412",
            CountryId = 1,
            RegisteredLocationId = 1
        });

        var template = await client.CreateTemplateAsync(new InquiryTemplateInput
        {
            AcceptPolicies = new List<UpdateInquiryTemplateAcceptPolicyDto>
            {
                new UpdateInquiryTemplateAcceptPolicyDto
                {
                    OrderWeight = 2,
                    Scope = InquiryAcceptPolicyScopes.HeadOfDepartment
                }
            },
            AccrualType = null,
            ActivityType = ActivityType.NonWorkingPaid,
            Comment = null,
            IsIncludeHolidays = true,
            IsIncludeWeekends = true,
            IsProhibitOnProbationPeriod = false,
            IsRequireAttachments = false,
            IsSubstitutesToApproveAllowed = false,
            Name = Guid.CreateVersion7().ToAlphabetic(),
            TrackingUnitType = TrackingUnit.Hours,
            TransferType = null,
            Type = InquiryType.SickLeave
        });

        await client.CreateInquiryAssignAsync(
            new CreateInquiryAssignInput(AssigneUserIds: [user!.Id], UnassigneUserIds: [],
                template!.Id, null, today), _jsonOptions);

        var inquiry = await client.CreateInquiryAsync(user!.Id, new UserInquiryInput
        {
            AgreementTemplateId = null,
            DateFrom = today,
            DateTo = today,
            Files = [],
            Hours = 7,
            InquiryTemplateId = template!.Id,
            Notes = null
        });

        var expectedBalanceHours = 7;

        var inquiry2 = await client.CreateInquiryAsync(user!.Id, new UserInquiryInput
        {
            AgreementTemplateId = null,
            DateFrom = today.AddDays(1),
            DateTo = today.AddDays(1),
            Files = [],
            Hours = 7,
            InquiryTemplateId = template!.Id,
            Notes = null,
        });

        await client.AcceptInquiryAsHttpResponseAsync(inquiry2!.Id);

        // Act
        var result = await client.GetInquiryByIdAsync(inquiry!.Id);

        // Assert
        result!.BalanceDays.Should().BeNull();
        result!.BalanceHours.Should().Be(expectedBalanceHours);
        result!.RemainingDays.Should().BeNull();
    }

    [Fact]
    public async Task GetInquiryByIdWhenInquiryApprovedAndOtherInquiriesCreatedShouldBalanceHoursBeNotUpdated()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCto();

        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        var user = await client.CreateUser(new CreateUserInput
        {
            Email = $"{Guid.CreateVersion7().ToAlphabetic()}@modsen-software.com",
            FirstName = Guid.CreateVersion7().ToAlphabetic(),
            LastName = Guid.CreateVersion7().ToAlphabetic(),
            FirstNameRu = "Имя",
            LastNameRu = "Фамилия",
            PatronymicRu = "Отчество",
            Location = LocationType.Minsk,
            Position = PortalRoleNames.Employee,
            DepartmentId = "Net",
            ResourceGroupId = _factory.ResourceGroupId,
            Color = "#123412",
            CountryId = 1,
            RegisteredLocationId = 1
        });

        var template = await client.CreateTemplateAsync(new InquiryTemplateInput
        {
            AcceptPolicies = new List<UpdateInquiryTemplateAcceptPolicyDto>
            {
                new UpdateInquiryTemplateAcceptPolicyDto
                {
                    OrderWeight = 2,
                    Scope = InquiryAcceptPolicyScopes.HeadOfDepartment
                }
            },
            AccrualType = null,
            ActivityType = ActivityType.NonWorkingPaid,
            Comment = null,
            IsIncludeHolidays = true,
            IsIncludeWeekends = true,
            IsProhibitOnProbationPeriod = false,
            IsRequireAttachments = false,
            IsSubstitutesToApproveAllowed = false,
            Name = Guid.CreateVersion7().ToAlphabetic(),
            TrackingUnitType = TrackingUnit.Hours,
            TransferType = null,
            Type = InquiryType.SickLeave
        });

        await client.CreateInquiryAssignAsync(
            new CreateInquiryAssignInput(AssigneUserIds: [user!.Id], UnassigneUserIds: [],
                template!.Id, null, today), _jsonOptions);

        var inquiry = await client.CreateInquiryAsync(user!.Id, new UserInquiryInput
        {
            AgreementTemplateId = null,
            DateFrom = today,
            DateTo = today,
            Files = [],
            Hours = 7,
            InquiryTemplateId = template!.Id,
            Notes = null,
        });

        await client.AcceptInquiryAsHttpResponseAsync(inquiry!.Id);

        var expectedBalanceHours = (await client.GetInquiryByIdAsync(inquiry!.Id))!.BalanceHours;

        var inquiry2 = await client.CreateInquiryAsync(user!.Id, new UserInquiryInput
        {
            AgreementTemplateId = null,
            DateFrom = today.AddDays(1),
            DateTo = today.AddDays(1),
            Files = [],
            Hours = 2,
            InquiryTemplateId = template!.Id,
            Notes = null,
        });

        await client.AcceptInquiryAsHttpResponseAsync(inquiry2!.Id);

        // Act
        var result = await client.GetInquiryByIdAsync(inquiry!.Id);

        // Assert
        result!.BalanceDays.Should().BeNull();
        result!.BalanceHours.Should().Be(expectedBalanceHours);
        result!.RemainingDays.Should().BeNull();
    }

    [Fact]
    public async Task GetInquiryByIdWhenInquiryRejectedAndOtherInquiriesCreatedShouldBalanceHoursBeNotUpdated()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCto();

        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        var user = await client.CreateUser(new CreateUserInput
        {
            Email = $"{Guid.CreateVersion7().ToAlphabetic()}@modsen-software.com",
            FirstName = Guid.CreateVersion7().ToAlphabetic(),
            LastName = Guid.CreateVersion7().ToAlphabetic(),
            FirstNameRu = "Имя",
            LastNameRu = "Фамилия",
            PatronymicRu = "Отчество",
            Location = LocationType.Minsk,
            Position = PortalRoleNames.Employee,
            DepartmentId = "Net",
            ResourceGroupId = _factory.ResourceGroupId,
            Color = "#123412",
            CountryId = 1,
            RegisteredLocationId = 1
        });

        var template = await client.CreateTemplateAsync(new InquiryTemplateInput
        {
            AcceptPolicies = new List<UpdateInquiryTemplateAcceptPolicyDto>
            {
                new UpdateInquiryTemplateAcceptPolicyDto
                {
                    OrderWeight = 2,
                    Scope = InquiryAcceptPolicyScopes.HeadOfDepartment
                }
            },
            AccrualType = null,
            ActivityType = ActivityType.NonWorkingPaid,
            Comment = null,
            IsIncludeHolidays = true,
            IsIncludeWeekends = true,
            IsProhibitOnProbationPeriod = false,
            IsRequireAttachments = false,
            IsSubstitutesToApproveAllowed = false,
            Name = Guid.CreateVersion7().ToAlphabetic(),
            TrackingUnitType = TrackingUnit.Hours,
            TransferType = null,
            Type = InquiryType.SickLeave
        });

        await client.CreateInquiryAssignAsync(
            new CreateInquiryAssignInput(AssigneUserIds: [user!.Id], UnassigneUserIds: [],
                template!.Id, null, today), _jsonOptions);

        var inquiry = await client.CreateInquiryAsync(user!.Id, new UserInquiryInput
        {
            AgreementTemplateId = null,
            DateFrom = today,
            DateTo = today,
            Files = [],
            Hours = 5,
            InquiryTemplateId = template!.Id,
            Notes = null,
        });

        await client.RejectInquiryAsHttpResponseAsync(inquiry!.Id);

        var expectedBalanceHours = (await client.GetInquiryByIdAsync(inquiry!.Id))!.BalanceHours;

        var inquiry2 = await client.CreateInquiryAsync(user!.Id, new UserInquiryInput
        {
            AgreementTemplateId = null,
            DateFrom = today.AddDays(1),
            DateTo = today.AddDays(1),
            Files = [],
            Hours = 7,
            InquiryTemplateId = template!.Id,
            Notes = null,
        });

        await client.AcceptInquiryAsHttpResponseAsync(inquiry2!.Id);

        // Act
        var result = await client.GetInquiryByIdAsync(inquiry!.Id);

        // Assert
        result!.BalanceDays.Should().BeNull();
        result!.BalanceHours.Should().Be(expectedBalanceHours);
        result!.RemainingDays.Should().BeNull();
    }

    [Fact]
    public async Task GetInquiryByIdWhenInquiryApprovedAndPositionsOfUsersChangedResponseWithOldPositions()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCto();

        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        var user = await client.CreateUser(new CreateUserInput
        {
            Email = $"{Guid.CreateVersion7().ToAlphabetic()}@modsen-software.com",
            FirstName = Guid.CreateVersion7().ToAlphabetic(),
            LastName = Guid.CreateVersion7().ToAlphabetic(),
            FirstNameRu = "Имя",
            LastNameRu = "Фамилия",
            PatronymicRu = "Отчество",
            Location = LocationType.Minsk,
            Position = PortalRoleNames.Employee,
            DepartmentId = "Ruby",
            ResourceGroupId = null,
            Color = "#123412",
            CountryId = 1,
            RegisteredLocationId = 1
        });

        var existingHod = (await client.GetUsersWithSearch(
                filter: "(isActive eq true) and (departmentId in ('Ruby')) and (position in ('HeadOfDepartment'))"))
                .Items.FirstOrDefault();

        var manager = existingHod ??
            await client.CreateUser(new CreateUserInput
            {
                Email = $"{Guid.CreateVersion7().ToAlphabetic()}@modsen-software.com",
                FirstName = Guid.CreateVersion7().ToAlphabetic(),
                LastName = Guid.CreateVersion7().ToAlphabetic(),
                FirstNameRu = "Имя",
                LastNameRu = "Фамилия",
                PatronymicRu = "Отчество",
                Location = LocationType.Minsk,
                Position = PortalRoleNames.HeadOfDepartment,
                DepartmentId = "Ruby",
                ResourceGroupId = null,
                Color = "#123412",
                CountryId = 1,
                RegisteredLocationId = 1
            });

        var template = await client.CreateTemplateAsync(new InquiryTemplateInput
        {
            AcceptPolicies = new List<UpdateInquiryTemplateAcceptPolicyDto>
            {
                new UpdateInquiryTemplateAcceptPolicyDto
                {
                    OrderWeight = 2,
                    Scope = InquiryAcceptPolicyScopes.HeadOfDepartment
                }
            },
            AccrualType = null,
            ActivityType = ActivityType.NonWorkingPaid,
            Comment = null,
            IsIncludeHolidays = true,
            IsIncludeWeekends = true,
            IsProhibitOnProbationPeriod = false,
            IsRequireAttachments = false,
            IsSubstitutesToApproveAllowed = false,
            Name = Guid.CreateVersion7().ToAlphabetic(),
            TrackingUnitType = TrackingUnit.Hours,
            TransferType = null,
            Type = InquiryType.SickLeave
        });

        await client.CreateInquiryAssignAsync(
            new CreateInquiryAssignInput(AssigneUserIds: [user!.Id], UnassigneUserIds: [],
                template!.Id, null, today), _jsonOptions);

        var inquiry = await client.CreateInquiryAsync(user!.Id, new UserInquiryInput
        {
            AgreementTemplateId = null,
            DateFrom = today,
            DateTo = today,
            Files = [],
            Hours = 5,
            InquiryTemplateId = template!.Id,
            Notes = null,
        });

        await client.AuthAsCto();

        var userToUpdate = new UpdateUserInput
        {
            Id = user.Id,
            Email = null,
            FirstName = null,
            LastName = null,
            Location = user.Location,
            Position = PortalRoleNames.GroupManager,
            DepartmentId = user.DepartmentId,
            ResourceGroupId = user.ResourceGroupId,
            Color = user.Color,
            CountryId = user.CountryId,
            RegisteredLocationId = user.RegisteredLocationId
        };

        var managerToUpdate = new UpdateUserInput
        {
            Id = manager!.Id,
            Email = null,
            FirstName = null,
            LastName = null,
            Location = LocationType.Vitebsk,
            Position = PortalRoleNames.HeadOfDepartment,
            DepartmentId = manager.DepartmentId,
            ResourceGroupId = manager.ResourceGroupId,
            Color = manager.Color,
            CountryId = manager.CountryId,
            RegisteredLocationId = manager.RegisteredLocationId
        };

        foreach (var accept in inquiry!.UserInquiryAccepts)
        {
            await client.AcceptInquiryAsHttpResponseAsync(inquiry.Id);
        }

        await client.UpdateUserAsHttpMessageAsync(userToUpdate, _jsonOptions);
        await client.UpdateUserAsHttpMessageAsync(managerToUpdate, _jsonOptions);

        // Act
        var result = await client.GetInquiryByIdAsync(inquiry!.Id);

        // Assert
        result!.ManagerOfUser!.Id.Should().Be(manager.Id);
        result.ManagerOfUser.Position.Should().Be(manager.Position);
        result.CreatedForUser!.Position.Should().Be(user.Position);
    }

    [Fact]
    public async Task GetInquiryByIdWhenInquiryRejectedAndPositionsOfUsersChangedResponseWithOldPositions()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCto();

        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        var user = await client.CreateUser(new CreateUserInput
        {
            Email = $"{Guid.CreateVersion7().ToAlphabetic()}@modsen-software.com",
            FirstName = Guid.CreateVersion7().ToAlphabetic(),
            LastName = Guid.CreateVersion7().ToAlphabetic(),
            FirstNameRu = "Имя",
            LastNameRu = "Фамилия",
            PatronymicRu = "Отчество",
            Location = LocationType.Minsk,
            Position = PortalRoleNames.Employee,
            DepartmentId = "Ruby",
            ResourceGroupId = null,
            Color = "#123412",
            CountryId = 1,
            RegisteredLocationId = 1
        });

        var existingHod = (await client.GetUsersWithSearch(
                filter: "(isActive eq true) and (departmentId in ('Ruby')) and (position in ('HeadOfDepartment'))"))
                .Items.FirstOrDefault();

        var manager = existingHod ??
            await client.CreateUser(new CreateUserInput
            {
                Email = $"{Guid.CreateVersion7().ToAlphabetic()}@modsen-software.com",
                FirstName = Guid.CreateVersion7().ToAlphabetic(),
                LastName = Guid.CreateVersion7().ToAlphabetic(),
                FirstNameRu = "Имя",
                LastNameRu = "Фамилия",
                PatronymicRu = "Отчество",
                Location = LocationType.Minsk,
                Position = PortalRoleNames.HeadOfDepartment,
                DepartmentId = "Ruby",
                ResourceGroupId = null,
                Color = "#123412",
                CountryId = 1,
                RegisteredLocationId = 1
            });

        var template = await client.CreateTemplateAsync(new InquiryTemplateInput
        {
            AcceptPolicies = new List<UpdateInquiryTemplateAcceptPolicyDto>
            {
                new UpdateInquiryTemplateAcceptPolicyDto
                {
                    OrderWeight = 2,
                    Scope = InquiryAcceptPolicyScopes.HeadOfDepartment
                }
            },
            AccrualType = null,
            ActivityType = ActivityType.NonWorkingPaid,
            Comment = null,
            IsIncludeHolidays = true,
            IsIncludeWeekends = true,
            IsProhibitOnProbationPeriod = false,
            IsRequireAttachments = false,
            IsSubstitutesToApproveAllowed = false,
            Name = Guid.CreateVersion7().ToAlphabetic(),
            TrackingUnitType = TrackingUnit.Hours,
            TransferType = null,
            Type = InquiryType.SickLeave
        });

        await client.CreateInquiryAssignAsync(
            new CreateInquiryAssignInput(AssigneUserIds: [user!.Id], UnassigneUserIds: [],
                template!.Id, null, today), _jsonOptions);

        var inquiry = await client.CreateInquiryAsync(user!.Id, new UserInquiryInput
        {
            AgreementTemplateId = null,
            DateFrom = today,
            DateTo = today,
            Files = [],
            Hours = 5,
            InquiryTemplateId = template!.Id,
            Notes = null,
        });

        await client.AuthAsCto();

        var userToUpdate = new UpdateUserInput
        {
            Id = user.Id,
            Email = null,
            FirstName = null,
            LastName = null,
            Location = user.Location,
            Position = PortalRoleNames.GroupManager,
            DepartmentId = user.DepartmentId,
            ResourceGroupId = user.ResourceGroupId,
            Color = user.Color,
            CountryId = user.CountryId,
            RegisteredLocationId = user.RegisteredLocationId
        };

        var managerToUpdate = new UpdateUserInput
        {
            Id = manager!.Id,
            Email = null,
            FirstName = null,
            LastName = null,
            Location = LocationType.Vitebsk,
            Position = PortalRoleNames.HeadOfDepartment,
            DepartmentId = manager.DepartmentId,
            ResourceGroupId = manager.ResourceGroupId,
            Color = manager.Color,
            CountryId = manager.CountryId,
            RegisteredLocationId = manager.RegisteredLocationId
        };

        await client.RejectInquiryAsHttpResponseAsync(inquiry!.Id);
        await client.UpdateUserAsHttpMessageAsync(userToUpdate, _jsonOptions);
        await client.UpdateUserAsHttpMessageAsync(managerToUpdate, _jsonOptions);

        // Act
        var result = await client.GetInquiryByIdAsync(inquiry!.Id);

        // Assert
        result!.ManagerOfUser!.Id.Should().Be(manager.Id);
        result.ManagerOfUser.Position.Should().Be(manager.Position);
        result.CreatedForUser!.Position.Should().Be(user.Position);
    }

    [Fact]
    public async Task GetInquiriesWhenUserHasViewAllRequestsClaimShouldReturnInquiryButCannotToApprove()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCto();

        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        var userInquiryFor = await client.CreateUser(new CreateUserInput
        {
            Email = $"{Guid.CreateVersion7().ToAlphabetic()}@modsen-software.com",
            FirstName = Guid.CreateVersion7().ToAlphabetic(),
            LastName = Guid.CreateVersion7().ToAlphabetic(),
            FirstNameRu = "Имя",
            LastNameRu = "Фамилия",
            PatronymicRu = "Отчество",
            Location = LocationType.Minsk,
            Position = PortalRoleNames.Employee,
            DepartmentId = "Ruby",
            ResourceGroupId = null,
            Color = "#123412",
            CountryId = 1,
            RegisteredLocationId = 1
        });

        var template = await client.CreateTemplateAsync(new InquiryTemplateInput
        {
            AcceptPolicies = new List<UpdateInquiryTemplateAcceptPolicyDto>
            {
                new UpdateInquiryTemplateAcceptPolicyDto
                {
                    OrderWeight = 2,
                    Scope = InquiryAcceptPolicyScopes.HeadOfDepartment
                }
            },
            AccrualType = null,
            ActivityType = ActivityType.NonWorkingPaid,
            Comment = null,
            IsIncludeHolidays = true,
            IsIncludeWeekends = true,
            IsProhibitOnProbationPeriod = false,
            IsRequireAttachments = false,
            IsSubstitutesToApproveAllowed = false,
            Name = Guid.CreateVersion7().ToAlphabetic(),
            TrackingUnitType = TrackingUnit.Hours,
            TransferType = null,
            Type = InquiryType.SickLeave
        });

        await client.CreateInquiryAssignAsync(
            new CreateInquiryAssignInput(AssigneUserIds: [userInquiryFor!.Id], UnassigneUserIds: [],
                template!.Id, null, today), _jsonOptions);

        var inquiry = await client.CreateInquiryAsync(userInquiryFor!.Id, new UserInquiryInput
        {
            AgreementTemplateId = null,
            DateFrom = today,
            DateTo = today,
            Files = [],
            Hours = 5,
            InquiryTemplateId = template!.Id,
            Notes = null,
        });

        await client.AuthAsCto();

        var userWithViewAllClaim = await client.CreateUser(new CreateUserInput
        {
            Email = $"{Guid.CreateVersion7().ToAlphabetic()}@modsen-software.com",
            FirstName = Guid.CreateVersion7().ToAlphabetic(),
            LastName = Guid.CreateVersion7().ToAlphabetic(),
            FirstNameRu = "Имя",
            LastNameRu = "Фамилия",
            PatronymicRu = "Отчество",
            Location = LocationType.Minsk,
            Position = PortalRoleNames.Employee,
            DepartmentId = "Administration",
            ResourceGroupId = null,
            Color = "#123412",
            CountryId = 1,
            RegisteredLocationId = 1
        });

        var claimsToUpdate = new UserClaimsInput { ClaimTypes = ["ViewAllRequests"], };
        await client.PatchAsJsonAsync($"/api/users/{userWithViewAllClaim!.Id}/claims", claimsToUpdate);
        await client.AuthAs(userWithViewAllClaim.Email);

        // Act
        var getByIdResult = await client.GetInquiryByIdAsync(inquiry!.Id);
        var getManyResult = await client.GetPageableInquiriesAsync(filter: $"(id eq {inquiry!.Id})");
        var acceptResponse = await client.AcceptInquiryAsHttpResponseAsync(inquiry!.Id);

        // Assert
        getByIdResult!.Id = inquiry!.Id;
        getManyResult.Items.Should().Contain(x => x.Id == inquiry!.Id);
        acceptResponse.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetInquiryByIdAsChiefWhenInquiryIsApprovedAndApproversListIsEmptyShouldCanBeRejectedShouldBeTrue()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCto();
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        //  create template
        var template = await client.CreateTemplateAsync(new InquiryTemplateInput
        {
            AcceptPolicies = new List<UpdateInquiryTemplateAcceptPolicyDto>
            {
                new UpdateInquiryTemplateAcceptPolicyDto
                {
                    OrderWeight = 1,
                    Scope = InquiryAcceptPolicyScopes.ProjectManager,
                }
            },
            AccrualType = null,
            ActivityType = ActivityType.NonWorkingUnpaid,
            Comment = null,
            IsIncludeHolidays = true,
            IsIncludeWeekends = true,
            IsProhibitOnProbationPeriod = false,
            IsRequireAttachments = false,
            IsSubstitutesToApproveAllowed = true,
            Name = Guid.CreateVersion7().ToAlphabetic(),
            TrackingUnitType = TrackingUnit.Days,
            TransferType = null,
            Type = InquiryType.OwnExpense
        });

        //  create user inquiry for
        var user = await client.CreateUser(new CreateUserInput
        {
            Email = $"{Guid.CreateVersion7().ToAlphabetic()}@modsen-software.com",
            FirstName = Guid.CreateVersion7().ToAlphabetic(),
            LastName = Guid.CreateVersion7().ToAlphabetic(),
            FirstNameRu = "Имя",
            LastNameRu = "Фамилия",
            PatronymicRu = "Отчество",
            Location = LocationType.Minsk,
            Position = PortalRoleNames.Employee,
            DepartmentId = "Net",
            ResourceGroupId = _factory.ResourceGroupId,
            Color = "#123412",
            CountryId = 1,
            RegisteredLocationId = 1
        });

        var pm = await client.CreateUser(new CreateUserInput
        {
            Email = $"{Guid.CreateVersion7().ToAlphabetic()}@modsen-software.com",
            FirstName = Guid.CreateVersion7().ToAlphabetic(),
            LastName = Guid.CreateVersion7().ToAlphabetic(),
            FirstNameRu = "Имя",
            LastNameRu = "Фамилия",
            PatronymicRu = "Отчество",
            Location = LocationType.Minsk,
            Position = PortalRoleNames.Employee,
            DepartmentId = "PMO",
            ResourceGroupId = null,
            Color = "#123412",
            CountryId = 1,
            RegisteredLocationId = 1
        });

        var project = await client.CreateProject(new CreateProjectInput
        {
            IsBillable = true,
            IsPublic = false,
            ClientId = _factory.ModsenClientId!.Value,
            Name = Guid.CreateVersion7().ToAlphabetic(),
            ManagerId = pm!.Id,
        });
        await client.CreateProjectAssignmentAsHttpMessageAsync(user!.Id, project!.Id, today, today);

        //  create assign user
        await client.CreateInquiryAssignAsync(
            new CreateInquiryAssignInput(AssigneUserIds: [user!.Id], UnassigneUserIds: [],
                template!.Id, null, today), _jsonOptions);

        var inquiryForUser = await client.CreateInquiryAsync(user!.Id, new UserInquiryInput
        {
            AgreementTemplateId = null,
            DateFrom = today,
            DateTo = today,
            Files = [],
            Hours = null,
            InquiryTemplateId = template!.Id,
            Notes = null,
        });

        await client.AcceptInquiryAsync(inquiryForUser!.Id);

        // Act
        var response = await client.GetInquiryByIdAsync(inquiryForUser!.Id);

        // Assert
        inquiryForUser.UserInquiryAccepts.Should().HaveCount(1);
        response!.UserInquiryAccepts.Should().HaveCount(1);
        response.CanCurrentUserApprove.Should().BeFalse();
        response.CanCurrentUserReject.Should().BeTrue();
    }
}

using Microsoft.AspNetCore.OData.Query;
using MockQueryable.Moq;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.OData;
using Modsen.Portal.Core.Services;
using Modsen.Portal.Core.Utils;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.Identity.Contract;
using Modsen.Portal.Identity.Contract.Models;
using Modsen.Portal.Inquiries.DataAccess.Npgsql;
using Modsen.Portal.Inquiries.DataAccess.Npgsql.Entities;
using Modsen.Portal.Inquiries.Requests.Assigns.Core.CoreDto;
using Modsen.Portal.Inquiries.Requests.Assigns.Requests;
using Moq;

namespace Modsen.Portal.Inquiries.UnitTests.Assigns.Requests;

public class GetAssignsByAgreementRequestTests
{
    private readonly Mock<IInquiriesDbContext> _dbContextMock = new();
    private readonly Mock<IIdentityContract> _identityContractMock = new();
    private readonly Mock<IClockService> _clockServiceMock = new();

    private readonly GetAssignsByAgreementRequest _handler;

    private readonly CancellationToken _token = CancellationToken.None;

    public GetAssignsByAgreementRequestTests()
    {
        _handler = new GetAssignsByAgreementRequest(
            _dbContextMock.Object,
            _identityContractMock.Object,
            _clockServiceMock.Object);
    }

    [Fact]
    public async Task HandleWhenAgreementHasNoAssignsReturnsEmptyList()
    {
        //Arrange
        _dbContextMock
            .Setup(context => context.UserAgreements)
            .Returns(new List<UserAgreementEntity>().BuildMockDbSet().Object);

        var agreements = new List<InquiryTemplateAgreementEntity>() { new() { Id = 1 } };
        _dbContextMock
            .Setup(context => context.InquiryTemplateAgreements)
            .Returns(agreements.BuildMockDbSet().Object);

        var queryParameters = new Dictionary<string, string?>
        {
            [nameof(ODataRawQueryOptions.Top)] = "50",
            [nameof(ODataRawQueryOptions.Skip)] = "0",
        };

        var queryOptions = ODataQueryOptionsMock.BuildMock<UserAgreementAssignCoreDto>(queryParameters);

        var query = new GetAssignsByAgreementRequest.Query(1, queryOptions);

        //Act
        var result = await _handler.Handle(query, _token);

        //Assert
        result.Items.Count.Should().Be(0);

    }

    [Fact]
    public async Task HandleWhenAgreementHasAssignsReturnsValuesWithCorrectData()
    {
        //Arrange
        const int agreementId = 1;

        var userAgreements = new List<UserAgreementEntity>()
        {
            new()
            {
                InquiryAgreementTemplateId = agreementId,
                User = new InquiriesUserEntity()
                {
                    UserId = 1,
                    LastName = "Charles",
                    FirstName = "Leclerc",
                    AvatarFileId = null,
                    Email = $"{Guid.CreateVersion7().ToAlphabetic()}@modsen-software.com",
                    DepartmentId = "js",
                    Position = PortalRoleNames.Employee,
                    IsActive = true,
                }
            },
            new()
            {
                InquiryAgreementTemplateId = agreementId,
                User = new InquiriesUserEntity()
                {
                    UserId = 2,
                    LastName = "Max",
                    FirstName = "Verstappen",
                    AvatarFileId = null,
                    Email = $"{Guid.CreateVersion7().ToAlphabetic()}@modsen-software.com",
                    DepartmentId = "net",
                    Position = PortalRoleNames.Employee,
                    IsActive = true,
                }
            },
            new()
            {
                InquiryAgreementTemplateId = agreementId,
                User = new InquiriesUserEntity()
                {
                    UserId = 3,
                    LastName = "Lando",
                    FirstName = "Norris",
                    AvatarFileId = null,
                    Email = $"{Guid.CreateVersion7().ToAlphabetic()}@modsen-software.com",
                    DepartmentId = "pmo",
                    Position = PortalRoleNames.Employee,
                    IsActive = true,
                }
            },
            new()
            {
                InquiryAgreementTemplateId = agreementId,
                User = new InquiriesUserEntity()
                {
                    UserId = 4,
                    LastName = "Lando",
                    FirstName = "CTO",
                    AvatarFileId = null,
                    Email = $"{Guid.CreateVersion7().ToAlphabetic()}@modsen-software.com",
                    DepartmentId = null,
                    Position = PortalRoleNames.Cto,
                    IsActive = true,
                }
            }
        };

        _dbContextMock
            .Setup(context => context.UserAgreements)
            .Returns(userAgreements.BuildMockDbSet().Object);

        var agreements = new List<InquiryTemplateAgreementEntity>() { new() { Id = agreementId } };
        _dbContextMock
            .Setup(context => context.InquiryTemplateAgreements)
            .Returns(agreements.BuildMockDbSet().Object);

        _identityContractMock
            .Setup(context => context.GetDepartmentsClassificator(_token))
            .ReturnsAsync(
            [
                new DepartmentClassificationDto("js", "JavaScript", "JavaScript"),
                new DepartmentClassificationDto("net", ".NET", ".NET"),
                new DepartmentClassificationDto("pmo", "PMO", "PMO")
            ]);

        var expectedResult = new List<UserAgreementAssignCoreDto>
        {
            new UserAgreementAssignCoreDto
            {
                AgreementTeamplateId = agreementId,
                UserId = userAgreements[0].UserId,
                UserFirstName = userAgreements[0].User!.FirstName,
                UserLastName = userAgreements[0].User!.LastName,
                UserFullName = ToFullName.Reverse(userAgreements[0].User!.FirstName, userAgreements[0].User!.LastName),
                Position = userAgreements[0].User!.Position,
                DepartmentId = "js",
                Department = "JavaScript",
                AvatarFileId = userAgreements[0].User!.AvatarFileId
            },
            new UserAgreementAssignCoreDto
            {
                AgreementTeamplateId = agreementId,
                UserId = userAgreements[1].UserId,
                UserFirstName = userAgreements[1].User!.FirstName,
                UserLastName = userAgreements[1].User!.LastName,
                UserFullName = ToFullName.Reverse(userAgreements[1].User!.FirstName, userAgreements[1].User!.LastName),
                Position = userAgreements[1].User!.Position,
                DepartmentId = "net",
                Department = ".NET",
                AvatarFileId = userAgreements[1].User!.AvatarFileId
            },
            new UserAgreementAssignCoreDto
            {
                AgreementTeamplateId = agreementId,
                UserId = userAgreements[2].UserId,
                UserFirstName = userAgreements[2].User!.FirstName,
                UserLastName = userAgreements[2].User!.LastName,
                UserFullName = ToFullName.Reverse(userAgreements[2].User!.FirstName, userAgreements[2].User!.LastName),
                Position = userAgreements[2].User!.Position,
                DepartmentId = "pmo",
                Department = "PMO",
                AvatarFileId = userAgreements[2].User!.AvatarFileId
            },
            new UserAgreementAssignCoreDto
            {
                AgreementTeamplateId = agreementId,
                UserId = userAgreements[3].UserId,
                UserFirstName = userAgreements[3].User!.FirstName,
                UserLastName = userAgreements[3].User!.LastName,
                UserFullName = ToFullName.Reverse(userAgreements[3].User!.FirstName, userAgreements[3].User!.LastName),
                Position = userAgreements[3].User!.Position,
                DepartmentId = null,
                Department = null,
                AvatarFileId = userAgreements[3].User!.AvatarFileId
            }
        };

        var queryParameters = new Dictionary<string, string?>
        {
            [nameof(ODataRawQueryOptions.Top)] = "50",
            [nameof(ODataRawQueryOptions.Skip)] = "0"
        };

        var queryOptions = ODataQueryOptionsMock.BuildMock<UserAgreementAssignCoreDto>(queryParameters);

        var query = new GetAssignsByAgreementRequest.Query(agreementId, queryOptions);

        //Act
        var result = await _handler.Handle(query, _token);

        //Assert
        result.Items.Count.Should().Be(4);
        result.Items.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    public async Task HandleWhenAgreementHasAssignsAndQueryUseEqualityFilterReturnsFilteredValueWithCorrectData()
    {
        //Arrange
        const int agreementId = 1;

        var userAgreements = new List<UserAgreementEntity>()
        {
            new()
            {
                InquiryAgreementTemplateId = agreementId,
                User = new InquiriesUserEntity()
                {
                    UserId = 1,
                    LastName = "Charles",
                    FirstName = "Leclerc",
                    AvatarFileId = null,
                    Email = $"{Guid.CreateVersion7().ToAlphabetic()}@modsen-software.com",
                    DepartmentId = "js",
                    Position = PortalRoleNames.Employee,
                    IsActive = true,
                }
            },
            new()
            {
                InquiryAgreementTemplateId = agreementId,
                User = new InquiriesUserEntity()
                {
                    UserId = 2,
                    LastName = "Max",
                    FirstName = "Verstappen",
                    AvatarFileId = null,
                    Email = $"{Guid.CreateVersion7().ToAlphabetic()}@modsen-software.com",
                    DepartmentId = "net",
                    Position = PortalRoleNames.Employee,
                    IsActive = true,
                }
            },
            new()
            {
                InquiryAgreementTemplateId = agreementId,
                User = new InquiriesUserEntity()
                {
                    UserId = 3,
                    LastName = "Lando",
                    FirstName = "Norris",
                    AvatarFileId = null,
                    Email = $"{Guid.CreateVersion7().ToAlphabetic()}@modsen-software.com",
                    DepartmentId = "pmo",
                    Position = PortalRoleNames.Employee,
                    IsActive = true,
                }
            }
        };

        _dbContextMock
            .Setup(context => context.UserAgreements)
            .Returns(userAgreements.BuildMockDbSet().Object);

        var agreements = new List<InquiryTemplateAgreementEntity>() { new() { Id = agreementId } };
        _dbContextMock
            .Setup(context => context.InquiryTemplateAgreements)
            .Returns(agreements.BuildMockDbSet().Object);

        _identityContractMock
            .Setup(context => context.GetDepartmentsClassificator(_token))
            .ReturnsAsync(
            [
                new DepartmentClassificationDto("js", "JavaScript", "JavaScript"),
                new DepartmentClassificationDto("net", ".NET", ".NET"),
                new DepartmentClassificationDto("pmo", "PMO", "PMO")
            ]);

        var userFullName = ToFullName.Reverse(userAgreements[0].User!.FirstName, userAgreements[0].User!.LastName);

        var expectedResult = new List<UserAgreementAssignCoreDto>
        {
           new UserAgreementAssignCoreDto
            {
                AgreementTeamplateId = agreementId,
                UserId = userAgreements[0].UserId,
                UserFirstName = userAgreements[0].User!.FirstName,
                UserLastName = userAgreements[0].User!.LastName,
                UserFullName = ToFullName.Reverse(userAgreements[0].User!.FirstName, userAgreements[0].User!.LastName),
                Position = userAgreements[0].User!.Position,
                DepartmentId = "js",
                Department = "JavaScript",
                AvatarFileId = userAgreements[0].User!.AvatarFileId
            }
        };

        var queryParameters = new Dictionary<string, string?>
        {
            [nameof(ODataRawQueryOptions.Top)] = "50",
            [nameof(ODataRawQueryOptions.Skip)] = "0",
            [nameof(ODataRawQueryOptions.Filter)] = $"userFullName eq '{userFullName}'",
        };

        var queryOptions = ODataQueryOptionsMock.BuildMock<UserAgreementAssignCoreDto>(queryParameters);

        var query = new GetAssignsByAgreementRequest.Query(agreementId, queryOptions);

        //Act
        var result = await _handler.Handle(query, _token);

        //Assert
        result.Items.Count.Should().Be(1);
        result.Items.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    public async Task HandleWhenAgreementHasAssignsAndQueryUseContainsFilterReturnsFilteredValueWithCorrectData()
    {
        //Arrange
        const int agreementId = 1;

        var userAgreements = new List<UserAgreementEntity>()
        {
            new()
            {
                InquiryAgreementTemplateId = agreementId,
                User = new InquiriesUserEntity()
                {
                    UserId = 1,
                    LastName = "Charles",
                    FirstName = "Leclerc",
                    AvatarFileId = null,
                    Email = $"{Guid.CreateVersion7().ToAlphabetic()}@modsen-software.com",
                    DepartmentId = "js",
                    Position = PortalRoleNames.Employee,
                    IsActive = true,
                }
            },
            new()
            {
                InquiryAgreementTemplateId = agreementId,
                User = new InquiriesUserEntity()
                {
                    UserId = 2,
                    LastName = "Max",
                    FirstName = "Verstappen",
                    AvatarFileId = null,
                    Email = $"{Guid.CreateVersion7().ToAlphabetic()}@modsen-software.com",
                    DepartmentId = "net",
                    Position = PortalRoleNames.Employee,
                    IsActive = true,
                }
            },
            new()
            {
                InquiryAgreementTemplateId = agreementId,
                User = new InquiriesUserEntity()
                {
                    UserId = 3,
                    LastName = "Lando",
                    FirstName = "Norris",
                    AvatarFileId = null,
                    Email = $"{Guid.CreateVersion7().ToAlphabetic()}@modsen-software.com",
                    DepartmentId = "pmo",
                    Position = PortalRoleNames.Employee,
                    IsActive = true,
                }
            }
        };

        _dbContextMock
            .Setup(context => context.UserAgreements)
            .Returns(userAgreements.BuildMockDbSet().Object);

        var agreements = new List<InquiryTemplateAgreementEntity>() { new() { Id = agreementId } };
        _dbContextMock
            .Setup(context => context.InquiryTemplateAgreements)
            .Returns(agreements.BuildMockDbSet().Object);

        _identityContractMock
            .Setup(context => context.GetDepartmentsClassificator(_token))
            .ReturnsAsync(
            [
                new DepartmentClassificationDto("js", "JavaScript", "JavaScript"),
                new DepartmentClassificationDto("net", ".NET", ".NET"),
                new DepartmentClassificationDto("pmo", "PMO", "PMO")
            ]);

        var expectedResult = new List<UserAgreementAssignCoreDto>
        {
            new UserAgreementAssignCoreDto
            {
                AgreementTeamplateId = agreementId,
                UserId = userAgreements[2].UserId,
                UserFirstName = userAgreements[2].User!.FirstName,
                UserLastName = userAgreements[2].User!.LastName,
                UserFullName = ToFullName.Reverse(userAgreements[2].User!.FirstName, userAgreements[2].User!.LastName),
                Position = userAgreements[2].User!.Position,
                DepartmentId = "pmo",
                Department = "PMO",
                AvatarFileId = userAgreements[2].User!.AvatarFileId
            },
        };

        var queryParameters = new Dictionary<string, string?>
        {
            [nameof(ODataRawQueryOptions.Top)] = "50",
            [nameof(ODataRawQueryOptions.Skip)] = "0",
            [nameof(ODataRawQueryOptions.Filter)] = "contains(userFullName,'Lando')",
        };

        var queryOptions = ODataQueryOptionsMock.BuildMock<UserAgreementAssignCoreDto>(queryParameters);

        var query = new GetAssignsByAgreementRequest.Query(agreementId, queryOptions);

        //Act
        var result = await _handler.Handle(query, _token);

        //Assert
        result.Items.Count.Should().Be(1);
        result.Items.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    public async Task HandleWhenUserHasFutureAssignShouldReturnFuture()
    {
        //Arrange
        const int agreementId = 1;
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        var userAgreements = new List<UserAgreementEntity>()
        {
            new()
            {
                InquiryAgreementTemplateId = agreementId,
                User = new InquiriesUserEntity()
                {
                    UserId = 1,
                    LastName = "Charles",
                    FirstName = "Leclerc",
                    AvatarFileId = null,
                    Email = $"{Guid.CreateVersion7().ToAlphabetic()}@modsen-software.com",
                    DepartmentId = "js",
                    Position = PortalRoleNames.Employee,
                    IsActive = true,
                },
                DateFrom = today.AddDays(10),
                DateTo = null
            },
            
        };

        _dbContextMock
            .Setup(context => context.UserAgreements)
            .Returns(userAgreements.BuildMockDbSet().Object);

        var agreements = new List<InquiryTemplateAgreementEntity>() { new() { Id = agreementId } };
        _dbContextMock
            .Setup(context => context.InquiryTemplateAgreements)
            .Returns(agreements.BuildMockDbSet().Object);

        _identityContractMock
            .Setup(context => context.GetDepartmentsClassificator(_token))
            .ReturnsAsync(
            [
                new DepartmentClassificationDto("js", "JavaScript", "JavaScript"),
                new DepartmentClassificationDto("net", ".NET", ".NET"),
                new DepartmentClassificationDto("pmo", "PMO", "PMO")
            ]);

        _clockServiceMock
            .Setup(x => x.TodayUtc())
            .Returns(today);

        var expectedResult = new List<UserAgreementAssignCoreDto>
        {
            new UserAgreementAssignCoreDto
            {
                AgreementTeamplateId = agreementId,
                UserId = userAgreements[0].UserId,
                UserFirstName = userAgreements[0].User!.FirstName,
                UserLastName = userAgreements[0].User!.LastName,
                UserFullName = ToFullName.Reverse(userAgreements[0].User!.FirstName, userAgreements[0].User!.LastName),
                Position = userAgreements[0].User!.Position,
                DepartmentId = "js",
                Department = "JavaScript",
                AvatarFileId = userAgreements[0].User!.AvatarFileId,
                DateFrom = today.AddDays(10),
                DateTo = null
            },
        };

        var queryParameters = new Dictionary<string, string?>
        {
            [nameof(ODataRawQueryOptions.Top)] = "50",
            [nameof(ODataRawQueryOptions.Skip)] = "0"
        };

        var queryOptions = ODataQueryOptionsMock.BuildMock<UserAgreementAssignCoreDto>(queryParameters);

        var query = new GetAssignsByAgreementRequest.Query(agreementId, queryOptions);

        //Act
        var result = await _handler.Handle(query, _token);

        //Assert
        result.Items.Count.Should().Be(1);
        result.Items.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    public async Task HandleWhenUserHasCurrentAndFutureAssignShouldReturnCurrent()
    {
        //Arrange
        const int agreementId = 1;
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        var userAgreements = new List<UserAgreementEntity>()
        {
            new()
            {
                InquiryAgreementTemplateId = agreementId,
                UserId = 1,
                User = new InquiriesUserEntity()
                {
                    UserId = 1,
                    LastName = "Charles",
                    FirstName = "Leclerc",
                    AvatarFileId = null,
                    Email = $"{Guid.CreateVersion7().ToAlphabetic()}@modsen-software.com",
                    DepartmentId = "js",
                    Position = PortalRoleNames.Employee,
                    IsActive = true,
                },
                DateFrom = today.AddDays(-1),
                DateTo = today.AddDays(2)
            },
            new()
            {
                InquiryAgreementTemplateId = agreementId,
                UserId = 1,
                User = new InquiriesUserEntity()
                {
                    UserId = 1,
                    LastName = "Charles",
                    FirstName = "Leclerc",
                    AvatarFileId = null,
                    Email = $"{Guid.CreateVersion7().ToAlphabetic()}@modsen-software.com",
                    DepartmentId = "js",
                    Position = PortalRoleNames.Employee,
                    IsActive = true,
                },
                DateFrom = today.AddDays(10),
                DateTo = null
            },

        };

        _dbContextMock
            .Setup(context => context.UserAgreements)
            .Returns(userAgreements.BuildMockDbSet().Object);

        var agreements = new List<InquiryTemplateAgreementEntity>() { new() { Id = agreementId } };
        _dbContextMock
            .Setup(context => context.InquiryTemplateAgreements)
            .Returns(agreements.BuildMockDbSet().Object);

        _identityContractMock
            .Setup(context => context.GetDepartmentsClassificator(_token))
            .ReturnsAsync(
            [
                new DepartmentClassificationDto("js", "JavaScript", "JavaScript"),
                new DepartmentClassificationDto("net", ".NET", ".NET"),
                new DepartmentClassificationDto("pmo", "PMO", "PMO")
            ]);

        _clockServiceMock
            .Setup(x => x.TodayUtc())
            .Returns(today);

        var expectedResult = new List<UserAgreementAssignCoreDto>
        {
            new UserAgreementAssignCoreDto
            {
                AgreementTeamplateId = agreementId,
                UserId = userAgreements[0].UserId,
                UserFirstName = userAgreements[0].User!.FirstName,
                UserLastName = userAgreements[0].User!.LastName,
                UserFullName = ToFullName.Reverse(userAgreements[0].User!.FirstName, userAgreements[0].User!.LastName),
                Position = userAgreements[0].User!.Position,
                DepartmentId = "js",
                Department = "JavaScript",
                AvatarFileId = userAgreements[0].User!.AvatarFileId,
                DateFrom = today.AddDays(-1),
                DateTo = today.AddDays(2)
            },
        };

        var queryParameters = new Dictionary<string, string?>
        {
            [nameof(ODataRawQueryOptions.Top)] = "50",
            [nameof(ODataRawQueryOptions.Skip)] = "0"
        };

        var queryOptions = ODataQueryOptionsMock.BuildMock<UserAgreementAssignCoreDto>(queryParameters);

        var query = new GetAssignsByAgreementRequest.Query(agreementId, queryOptions);

        //Act
        var result = await _handler.Handle(query, _token);

        //Assert
        result.Items.Count.Should().Be(1);
        result.Items.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    public async Task HandleWhenUserAssignedToSeveralAgreementsAtSameDateFromReturnValid()
    {
        //Arrange
        const int agreementId = 1;
        const int agreementId2 = 2;
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        var userAgreements = new List<UserAgreementEntity>()
        {
            new()
            {
                InquiryAgreementTemplateId = agreementId,
                UserId = 1,
                User = new InquiriesUserEntity()
                {
                    UserId = 1,
                    LastName = "Charles",
                    FirstName = "Leclerc",
                    AvatarFileId = null,
                    Email = $"{Guid.CreateVersion7().ToAlphabetic()}@modsen-software.com",
                    DepartmentId = "js",
                    Position = PortalRoleNames.Employee,
                    IsActive = true,
                },
                DateFrom = today.AddDays(-1),
                DateTo = today.AddDays(2)
            },
            new()
            {
                InquiryAgreementTemplateId = agreementId2,
                UserId = 1,
                User = new InquiriesUserEntity()
                {
                    UserId = 1,
                    LastName = "Charles",
                    FirstName = "Leclerc",
                    AvatarFileId = null,
                    Email = $"{Guid.CreateVersion7().ToAlphabetic()}@modsen-software.com",
                    DepartmentId = "js",
                    Position = PortalRoleNames.Employee,
                    IsActive = true,
                },
                DateFrom = today.AddDays(-1),
                DateTo = null
            },

        };

        _dbContextMock
            .Setup(context => context.UserAgreements)
            .Returns(userAgreements.BuildMockDbSet().Object);

        var agreements = new List<InquiryTemplateAgreementEntity>() { new() { Id = agreementId } };
        _dbContextMock
            .Setup(context => context.InquiryTemplateAgreements)
            .Returns(agreements.BuildMockDbSet().Object);

        _identityContractMock
            .Setup(context => context.GetDepartmentsClassificator(_token))
            .ReturnsAsync(
            [
                new DepartmentClassificationDto("js", "JavaScript", "JavaScript")
            ]);

        _clockServiceMock
            .Setup(x => x.TodayUtc())
            .Returns(today);

        var expectedResult = new List<UserAgreementAssignCoreDto>
        {
            new UserAgreementAssignCoreDto
            {
                AgreementTeamplateId = agreementId,
                UserId = userAgreements[0].UserId,
                UserFirstName = userAgreements[0].User!.FirstName,
                UserLastName = userAgreements[0].User!.LastName,
                UserFullName = ToFullName.Reverse(userAgreements[0].User!.FirstName, userAgreements[0].User!.LastName),
                Position = userAgreements[0].User!.Position,
                DepartmentId = "js",
                Department = "JavaScript",
                AvatarFileId = userAgreements[0].User!.AvatarFileId,
                DateFrom = userAgreements[0].DateFrom,
                DateTo = userAgreements[0].DateTo
            },
        };

        var queryParameters = new Dictionary<string, string?>
        {
            [nameof(ODataRawQueryOptions.Top)] = "50",
            [nameof(ODataRawQueryOptions.Skip)] = "0"
        };

        var queryOptions = ODataQueryOptionsMock.BuildMock<UserAgreementAssignCoreDto>(queryParameters);

        var query = new GetAssignsByAgreementRequest.Query(agreementId, queryOptions);

        //Act
        var result = await _handler.Handle(query, _token);

        //Assert
        result.Items.Count.Should().Be(1);
        result.Items.Should().BeEquivalentTo(expectedResult);
    }
}

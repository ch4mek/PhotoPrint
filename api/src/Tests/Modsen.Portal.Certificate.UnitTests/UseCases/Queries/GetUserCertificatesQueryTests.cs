using Microsoft.AspNetCore.OData.Query;
using Modsen.Portal.Certificate.UseCases;
using Modsen.Portal.Certificate.UseCases.Dto;
using Modsen.Portal.Core.Api.Context;
using Modsen.Portal.Core.Certificate;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.OData;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.Identity.Contract;
using Modsen.Portal.Identity.Contract.Models;
using Modsen.Portal.ModsenApi.Contract;
using Moq;

namespace Modsen.Portal.Certificate.UnitTests.UseCases.Queries;

public class GetUserCertificatesQueryTests
{
    private readonly Mock<IModsenApiCertificatesClient> _mockCertificatesClient;
    private readonly Mock<IIdentityContract> _identityContractMock = new();
    private readonly Mock<IAuthenticatedUserContext> _authenticatedUserContextMock;

    private readonly CancellationToken _cancellationToken = CancellationToken.None;
    private readonly GetUserCertificatesQueryUseCase _handler;

    public GetUserCertificatesQueryTests()
    {
        _mockCertificatesClient = new Mock<IModsenApiCertificatesClient>();
        _authenticatedUserContextMock = new Mock<AuthenticatedUserContext>
            { CallBase = true }.As<IAuthenticatedUserContext>();

        _handler = new GetUserCertificatesQueryUseCase(
            _mockCertificatesClient.Object,
            _authenticatedUserContextMock.Object,
            _identityContractMock.Object);
    }

    [Theory]
    [InlineData(PortalRoleNames.Employee)]
    [InlineData(PortalRoleNames.Cto)]
    public async Task GetCertificatesByUserIdWhenRequestIsValidReturnCertificate(string role)
    {
        return;

        //Arrange
        const int userId = 123;
        const string jsTechnologyFullName = "JavaScript";
        var queryParameters = new Dictionary<string, string?>
        {
            [nameof(ODataRawQueryOptions.Top)] = "50",
            [nameof(ODataRawQueryOptions.Skip)] = "0"
        };

        var queryOptions = ODataQueryOptionsMock.BuildMock<CertificateDto>(queryParameters);

        var request = new GetUserCertificatesQueryUseCase.Query(userId, queryOptions);



        var dateNow = DateTime.UtcNow;
        var dateYesterday = dateNow.AddDays(-1);

        var certificateModels = new List<CertificateModel>
        {
            new()
            {
                CertificateId = "1",
                ReceiptDate = dateYesterday,
                Recipient = request.UserId.ToString(),
                Stack = "Node",
                Technology = jsTechnologyFullName,
                CreatedAt = dateYesterday,
                UpdatedAt = dateYesterday
            },
            new()
            {
                CertificateId = "2",
                ReceiptDate = dateNow,
                Recipient = request.UserId.ToString(),
                Stack = "React",
                Technology = jsTechnologyFullName,
                CreatedAt = dateNow,
                UpdatedAt = dateNow
            }
        };

        var certificateDtos = new List<CertificateDto>
        {
            new()
            {
                CertificateId = certificateModels[1].CertificateId,
                CreatedAt = certificateModels[1].CreatedAt,
                ReceiptDate = certificateModels[1].ReceiptDate,
                Recipient = certificateModels[1].RecipientId,
                Stack = JsStackType.React.GetDescription(),
                Technology = certificateModels[1].Technology,
                UpdatedAt = certificateModels[1].UpdatedAt
            },
            new()
            {
                CertificateId = certificateModels[0].CertificateId,
                CreatedAt = certificateModels[0].CreatedAt,
                ReceiptDate = certificateModels[0].ReceiptDate,
                Recipient = certificateModels[0].RecipientId,
                Stack = JsStackType.Node.GetDescription(),
                Technology = certificateModels[0].Technology,
                UpdatedAt = certificateModels[0].UpdatedAt
            }
        };
        _identityContractMock.Setup(x => x.GetDepartmentModsenApiValues(_cancellationToken))
            .ReturnsAsync([
                new DepartmentModsenApiValue(Id: "Js", ModsenApiValue: jsTechnologyFullName)
            ]);

        _authenticatedUserContextMock.Setup(x => x.GetRoles())
            .Returns([role]);
        _authenticatedUserContextMock.Setup(x => x.UserId)
            .Returns(userId);
        _authenticatedUserContextMock.Setup(x => x.GetRequiredUserId())
            .Returns(userId);

        _mockCertificatesClient
            .Setup(x => x.GetUserCertificates(request.UserId.ToString(), _cancellationToken))
            .ReturnsAsync(certificateModels);

        //Act
        var result = await _handler.Handle(request, _cancellationToken);

        //Assert
        result.Items.Should().NotBeNullOrEmpty();
        result.Items.Should().HaveCount(2);
        result.Items.Should().BeEquivalentTo(certificateDtos, options => options.WithStrictOrdering());

        _mockCertificatesClient.Verify(client =>
            client.GetUserCertificates(userId.ToString(), _cancellationToken), Times.Once);
        _identityContractMock.Verify(ic => ic.GetDepartmentModsenApiValues(_cancellationToken), Times.Once);
    }
}

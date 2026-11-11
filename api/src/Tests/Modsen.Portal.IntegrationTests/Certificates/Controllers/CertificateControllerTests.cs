using System.Net;
using System.Text.Json;
using Modsen.Portal.Core.Certificate;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Identity;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
namespace Modsen.Portal.IntegrationTests.Certificates.Controllers;

[Collection("GeneralCollection")]
public class CertificateControllerTests
{
    private readonly IntegrationTestFactory _factory;
    private readonly JsonSerializerOptions _jsonOptions;
    public CertificateControllerTests(IntegrationTestFactory factory)
    {
        _factory = factory;
        _jsonOptions = factory.JsonOptions;
    }

    [Theory(Skip = "NA")]
    [InlineData("Net", null)]
    [InlineData("Python", null)]
    [InlineData("QA", null)]
    [InlineData("Java", null)]
    [InlineData("Js", JsStackType.React)]
    [InlineData("Js", JsStackType.Node)]
    [InlineData("Js", JsStackType.ReactNative)]
    public async Task AddingCertificateWhenAuthedAsCeoAndTryToAddUniqueCertificateShouldReturnOk(string departmentId, JsStackType? jsStackType)
    {
        if (!_factory.IsCertificateTestsEnabled())
        {
            true.Should().BeTrue();
        }

        //Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var employee = await client.CreateEmployee(departmentId: departmentId);

        var certificates = await client.GetEmployeeCertificates(employee.Id);

        if (certificates is { Items.Count: > 0 })
        {
            var certificateId = certificates.Items
                .FirstOrDefault(
                    x => x.Technology == departmentId &&
                    (jsStackType == null || x.Stack == jsStackType?.GetDescription()))?
                .CertificateId;

            if (certificateId != null)
            {
                await client.DeleteCertificateAsHttpMessage(certificateId);
            }
        }

        //Act
        var createResponse = await client.CreateCertificateAsHttpMessage(
            recipientId: employee.Id.ToString(),
            technology: departmentId,
            stack: jsStackType);

        //Assert
        createResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Theory(Skip = "NA")]
    [InlineData("Net", null)]
    [InlineData("Python", null)]
    [InlineData("QA", null)]
    [InlineData("Java", null)]
    [InlineData("Js", JsStackType.React)]
    [InlineData("Js", JsStackType.Node)]
    [InlineData("Js", JsStackType.ReactNative)]
    public async Task AddingCertificateWhenAuthedAsCeoAndTryToAddDuplicatedCertificateShouldReturnConflict(string departmentId, JsStackType? jsStackType)
    {
        if (!_factory.IsCertificateTestsEnabled())
        {
            true.Should().BeTrue();
        }

        //Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();
        var employee = await client.CreateEmployee(departmentId: "Net");
        var recipient = $"{Guid.CreateVersion7().ToAlphabetic()} {Guid.CreateVersion7().ToAlphabetic()}";

        await client.CreateCertificateAsHttpMessage(
            recipientId: employee!.Id.ToString(),
            technology: departmentId,
            recipient: recipient,
            stack: jsStackType);

        //Act
        var certificateResponse = await client.CreateCertificateAsHttpMessage(
            recipientId: employee.Id.ToString(),
            technology: departmentId,
            recipient: recipient,
            stack: jsStackType);

        //Assert
        certificateResponse.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact(Skip = "NA")]
    public async Task DeleteCertificateWhenAuthedAsCeoAndTryToAddUniqueCertificateShouldReturnOk()
    {
        if (!_factory.IsCertificateTestsEnabled())
        {
            true.Should().BeTrue();
        }

        //Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var employee = await client.CreateEmployee();
        var employeeId = employee.Id;

        var certificates = await client.GetEmployeeCertificates(employeeId);

        if (certificates is { Items.Count: > 0 })
        {
            var certificateId = certificates.Items.First().CertificateId;

            await client.DeleteCertificateAsHttpMessage(certificateId);
        }

        var createdCertificate = await client.CreateCertificate(recipientId: employeeId.ToString());

        //Act
        var deleteResponse = await client.DeleteCertificateAsHttpMessage(createdCertificate.CertificateId);

        //Assert
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Theory(Skip = "NA")]
    [InlineData("PHP", null)]
    public async Task AddingCertificateWhenAuthedAsCeoAndTryToAddCertificateFromUnavailableDepartmentShouldReturnConflict(string departmentId, JsStackType? jsStackType)
    {
        if (!_factory.IsCertificateTestsEnabled())
        {
            true.Should().BeTrue();
        }

        //Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var employee = await client.CreateEmployee(departmentId: "Net");

        //Act
        var certificateResponse = await client.CreateCertificateAsHttpMessage(
            recipientId: employee.Id.ToString(),
            technology: departmentId,
            stack: jsStackType);

        //Assert
        certificateResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}

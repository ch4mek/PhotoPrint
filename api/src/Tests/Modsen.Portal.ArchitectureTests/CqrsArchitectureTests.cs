using System.Reflection;
using FluentValidation;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Identity.Requests.Users.Requests;
using Modsen.Portal.Storage.Requests.Files.Requests;
using Modsen.Portal.TimeTrack.UseCases.Reports.Core;
using Modsen.Portal.TimeTrack.UseCases.Reports.Requests;
using NetArchTest.Rules;

namespace Modsen.Portal.ArchitectureTests;

public class CqrsArchitectureTests
{
    [Fact]
    public void CommandsShouldBeSealed()
    {
        var failingTypes = Types.InAssemblies(GetAssemblies())
            .That()
            .ImplementInterface(typeof(ICommand<>))
            .Should()
            .BeSealed()
            .GetResult()
            .FailingTypes;

        failingTypes
            .Where(x => x.ReflectionType != typeof(UploadFileRequest.InternalCommand))
            .Should().BeEmpty();
    }

    [Fact]
    public void CommandsShouldImplementTransactionalRequest()
    {
        var failingTypes = Types.InAssemblies(GetAssemblies())
            .That()
            .ImplementInterface(typeof(ICommand<>))
            .Should()
            .ImplementInterface<ITransactionalRequest>()
            .GetResult()
            .FailingTypes;

        failingTypes = failingTypes.Where(type =>
            type.ReflectionType != typeof(ProcessReportRequest.Command) &&
            type.ReflectionType != typeof(UpdateUserGeneralInfoRequest.InternalCommand) &&
            type.ReflectionType != typeof(UploadFileRequest.InternalCommand)
        ).ToList();

        // todo: add interface or replace to query or add to ignore list
        failingTypes.Should().BeEmpty();
    }

    [Fact]
    public void CommandShouldHaveNameEndingWithCommand()
    {
        var failingTypes = Types.InAssemblies(GetAssemblies())
            .That()
            .ImplementInterface(typeof(ICommand<>))
            .Should()
            .HaveNameEndingWith("Command")
            .GetResult()
            .FailingTypes;

        failingTypes.Should().BeEmpty();
    }

    [Fact]
    public void CommandHandlerShouldNotBePublic()
    {
        var failingTypes = Types.InAssemblies(GetAssemblies())
           .That()
            .ImplementInterface(typeof(ICommandHandler<,>))
            .Should()
            .NotBePublic()
            .GetResult()
            .FailingTypes;

        failingTypes.Should().BeEmpty();
    }

    [Fact]
    public void CommandHandlerShouldBeSealed()
    {
        var failingTypes = Types.InAssemblies(GetAssemblies())
            .That()
            .ImplementInterface(typeof(ICommandHandler<,>))
            .Should()
            .BeSealed()
            .GetResult()
            .FailingTypes;

        failingTypes.Should().BeEmpty();
    }

    [Fact]
    public void QueriesShouldBeSealed()
    {
        var failingTypes = Types.InAssemblies(GetAssemblies())
            .That()
            .ImplementInterface(typeof(IQuery<>))
            .Should()
            .BeSealed()
            .GetResult()
            .FailingTypes;

        failingTypes.Should().BeEmpty();
    }



    [Fact]
    public void QueriesShouldHaveNameEndingWithQuery()
    {
        var failingTypes = Types.InAssemblies(GetAssemblies())
            .That()
            .ImplementInterface(typeof(IQuery<>))
            .Should()
            .HaveNameEndingWith("Query")
            .GetResult()
            .FailingTypes;

        failingTypes.Should().BeEmpty();
    }

    [Fact]
    public void QueryHandlerShouldNotBePublic()
    {
        var failingTypes = Types.InAssemblies(GetAssemblies())
            .That()
            .ImplementInterface(typeof(IQueryHandler<,>))
            .Should()
            .NotBePublic()
            .GetResult()
            .FailingTypes;

        failingTypes.Should().BeEmpty();
    }

    [Fact]
    public void QueryHandlerShouldBeSealed()
    {
        var failingTypes = Types.InAssemblies(GetAssemblies())
            .That()
            .ImplementInterface(typeof(IQueryHandler<,>))
            .Should()
            .BeSealed()
            .GetResult()
            .FailingTypes;

        failingTypes.Should().BeEmpty();
    }

    [Fact]
    public void ValidatorShouldNotBePublic()
    {
        var failingTypes = Types.InAssemblies(GetAssemblies())
            .That()
            .Inherit(typeof(AbstractValidator<>))
            .Should()
            .NotBePublic()
            .GetResult()
            .FailingTypes;

        failingTypes.Should().BeEmpty();
    }

    [Fact]
    public void ValidatorShouldBeSealed()
    {
        var failingTypes = Types.InAssemblies(GetAssemblies())
            .That()
            .Inherit(typeof(AbstractValidator<>))
            .Should()
            .BeSealed()
            .GetResult()
            .FailingTypes;

        failingTypes = failingTypes.Where(type =>
            type.ReflectionType != typeof(PeriodValidator<>)
        ).ToList();

        failingTypes.Should().BeEmpty();
    }

    [Fact]
    public void ValidatorShouldHaveNameEndingWithValidator()
    {
        var failingTypes = Types.InAssemblies(GetAssemblies())
            .That()
            .Inherit(typeof(AbstractValidator<>))
            .Should()
            .HaveNameEndingWith("Validator")
            .GetResult()
            .FailingTypes;

        failingTypes.Should().BeEmpty();
    }


    private static IEnumerable<Assembly> GetAssemblies()
    {
        return TestAssemblies.GetAssemblies();
    }
}

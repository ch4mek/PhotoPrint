using System.Reflection;
using MassTransit;
using Modsen.Portal.Core.Communication.ExchangeDefinition;
using NetArchTest.Rules;

namespace Modsen.Portal.ArchitectureTests;

public class CommunicationArchitectureTests
{
    [Fact]
    public void ExchangeDefinitionShouldBeSealed()
    {
        var failingTypes = Types.InAssemblies(GetAssemblies())
            .That()
            .Inherit(typeof(ExchangeDefinitionBase<>))
            .Should()
            .BeSealed()
            .GetResult()
            .FailingTypes;

        failingTypes.Should().BeEmpty();
    }

    [Fact]
    public void ExchangeDefinitionShouldImplementGenericExchangeDefinition()
    {
        var failingTypes = Types.InAssemblies(GetAssemblies())
            .That()
            .HaveNameEndingWith("ExchangeDefinition")
            .Should()
            .Inherit(typeof(ExchangeDefinitionBase<>))
            .GetResult()
            .FailingTypes;

        failingTypes.Should().BeEmpty();
    }

    [Fact]
    public void ExchangeDefinitionShouldHaveNameEndingWithExchangeDefinition()
    {
        var failingTypes = Types.InAssemblies(GetAssemblies())
            .That()
            .Inherit(typeof(ExchangeDefinitionBase<>))
            .Should()
            .HaveNameEndingWith("ExchangeDefinition")
            .GetResult()
            .FailingTypes;

        failingTypes.Should().BeEmpty();
    }

    [Fact]
    public void ExchangeDefinitionShouldNotBePublic()
    {
        var failingTypes = Types.InAssemblies(GetAssemblies())
            .That()
            .Inherit(typeof(ExchangeDefinitionBase<>))
            .Should()
            .NotBePublic()
            .GetResult()
            .FailingTypes;

        failingTypes.Should().BeEmpty();
    }

    [Fact]
    public void EventConsumerShouldBeSealed()
    {
        var failingTypes = Types.InAssemblies(GetAssemblies())
            .That()
            .ImplementInterface(typeof(IConsumer<>))
            .Should()
            .BeSealed()
            .GetResult()
            .FailingTypes;

        failingTypes.Should().BeEmpty();
    }

    [Fact]
    public void EventConsumerShouldImplementConsumerInterface()
    {
        var failingTypes = Types.InAssemblies(GetAssemblies())
            .That()
            .HaveNameEndingWith("Consumer")
            .Should()
            .ImplementInterface<IConsumer>()
            .GetResult()
            .FailingTypes;

        failingTypes.Should().BeEmpty();
    }

    [Fact]
    public void EventConsumerShouldHaveNameEndingWithCommand()
    {
        var failingTypes = Types.InAssemblies(GetAssemblies())
            .That()
            .ImplementInterface(typeof(IConsumer<>))
            .Should()
            .HaveNameEndingWith("EventConsumer")
            .GetResult()
            .FailingTypes;

        failingTypes.Should().BeEmpty();
    }

    [Fact]
    public void EventConsumerShouldNotBePublic()
    {
        var failingTypes = Types.InAssemblies(GetAssemblies())
            .That()
            .ImplementInterface(typeof(IConsumer<>))
            .Should()
            .NotBePublic()
            .GetResult()
            .FailingTypes;

        failingTypes.Should().BeEmpty();
    }

    [Fact]
    public void ConsumerDefinitionShouldBeSealed()
    {
        var failingTypes = Types.InAssemblies(GetAssemblies())
            .That()
            .Inherit(typeof(ConsumerDefinition<>))
            .Should()
            .BeSealed()
            .GetResult()
            .FailingTypes;

        failingTypes.Should().BeEmpty();
    }


    [Fact]
    public void ConsumerDefinitionShouldImplementConsumerInterface()
    {
        var failingTypes = Types.InAssemblies(GetAssemblies())
            .That()
            .HaveNameEndingWith("ConsumerDefinition")
            .Should()
            .Inherit(typeof(ConsumerDefinition<>))
            .GetResult()
            .FailingTypes;

        failingTypes.Should().BeEmpty();
    }

    [Fact]
    public void ConsumerDefinitionShouldHaveNameEndingWithConsumerDefinition()
    {
        var failingTypes = Types.InAssemblies(GetAssemblies())
            .That()
            .Inherit(typeof(ConsumerDefinition<>))
            .Should()
            .HaveNameEndingWith("ConsumerDefinition")
            .GetResult()
            .FailingTypes;

        failingTypes.Should().BeEmpty();
    }

    [Fact]
    public void ConsumerDefinitionShouldNotBePublic()
    {
        var failingTypes = Types.InAssemblies(GetAssemblies())
            .That()
            .Inherit(typeof(ConsumerDefinition<>))
            .Should()
            .NotBePublic()
            .GetResult()
            .FailingTypes;

        failingTypes.Should().BeEmpty();
    }

    private static IEnumerable<Assembly> GetAssemblies()
    {
        return TestAssemblies.GetAssemblies();
    }
}

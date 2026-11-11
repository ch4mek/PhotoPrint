using System.Linq.Expressions;
using Modsen.Portal.Core.Communication;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.KnowledgeBase.DataAccess.Npgsql;
using Modsen.Portal.KnowledgeBase.Requests.Discounts.Core;
using Modsen.Portal.KnowledgeBase.Requests.Discounts.Core.BusinessRules;
using Modsen.Portal.KnowledgeBase.Requests.Discounts.Core.CoreDto;
using Modsen.Portal.KnowledgeBase.Requests.Discounts.Requests;
using Moq;
using Xunit.Sdk;

namespace Modsen.Portal.KnowledgeBase.UnitTests.Discounts;

public class UpdateDiscountRequestTests
{
    private readonly Mock<IKnowledgeBaseDbContext> _dbContextMock = new();
    private readonly Mock<IHasDiscountCategoryAllDiscountTagsBusinessRule> _hasDiscountCategoryAllDiscountTagsBusinessRuleMock = new();
    private readonly Mock<ICommunicationBus> _icommunicationBusMock = new();
    private readonly Mock<IImageFileIdValidator> _imageFileIdValidatorMock = new();

    private readonly UpdateDiscountRequest _handler;

    public UpdateDiscountRequestTests()
    {
        _handler = new UpdateDiscountRequest(_dbContextMock.Object,
            new DiscountLoader(_dbContextMock.Object),
            _hasDiscountCategoryAllDiscountTagsBusinessRuleMock.Object,
            _icommunicationBusMock.Object,
            _imageFileIdValidatorMock.Object);
    }

    private sealed class UpdateInlineData : DataAttribute
    {
        private static U MatchField<T, U>(T obj, Expression<Func<T, U>> field)
        {
            _ = field.Body as MemberExpression
                ?? (field.Body as UnaryExpression)?.Operand as MemberExpression
                ?? throw new ArgumentException("Expression must represent a field, property or object");

            return field.Compile().Invoke(obj);
        }

        private static Action<DiscountCoreDto> Assertion<U>(Expression<Func<DiscountCoreDto, U>> field, U value)
        {
            return Assertion(field, obj => obj.Should().Be(value, $"`{field}`"));
        }

        private static Action<DiscountCoreDto> Assertion<U>(Expression<Func<DiscountCoreDto, U>> field, Action<U> match)
        {
            return discount => { match(MatchField(discount, field)); };
        }


        public override IEnumerable<object[]> GetData(System.Reflection.MethodInfo testMethod)
        {
            var command = new UpdateDiscountRequest.Command { Id = 1, DiscountCategoryId = 1, Name = Guid.CreateVersion7().ToAlphabetic(), Text = "Text", DiscountCityIds = [1] };

            return
            [
                [command, Assertion(b => b.Id, command.Id)],
                [command with { DiscountCategoryId = 2 }, Assertion(b => b.DiscountCategoryId, 2)],
                [command with { Text = "New" }, Assertion(b => b.Text, "New")],
                [command with { Name = "New" }, Assertion(b => b.Name, "New")],
                [command with { IsActive = true }, Assertion(b => b.IsActive, true)],
                [command with { DiscountPercent = 15 }, Assertion(b => b.DiscountPercent, 15)],
                [
                    command with { Text = "Old", Name = "Old" }, (DiscountCoreDto b) =>
                    {
                        b.Name.Should().Be("Old");
                        b.Text.Should().Be("Old");
                    }
                ],
            ];
        }
    }

    [Theory]
    [UpdateInlineData]
    internal async Task Handle_WhenUpdateWithId_ShouldDoNothing(
        UpdateDiscountRequest.Command command,
        Action<DiscountCoreDto> assert,
        bool skip = false)
    {
        // Arrange
        Shared.MockDbContext(_dbContextMock);

        // Act
        var discount = await _handler.Handle(command, It.IsAny<CancellationToken>());

        // Assert
        if (!skip)
        {
            assert.Invoke(discount);
        }
    }
}

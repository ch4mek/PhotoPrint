using Modsen.Portal.Core;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Merch;
using Modsen.Portal.Core.Services;
using Modsen.Portal.Merch.DataAccess.Npgsql;
using Modsen.Portal.Merch.DataAccess.Npgsql.Entities;
using Modsen.Portal.Merch.Requests.Items.Core.BusinessRules;
using Modsen.Portal.Merch.Requests.Items.Core.Composers;
using Modsen.Portal.Merch.Requests.Items.Core.DbQueries;
using Modsen.Portal.Merch.Requests.Items.Core.Validators;
using Modsen.Portal.Merch.Requests.Items.Requests;
using Modsen.Portal.Merch.Requests.Items.Requests.RequestDto;
using Moq;

namespace Modsen.Portal.Merch.UnitTests.Items.Requests;

public class UpdateMerchItemRequestTests
{
    private readonly Mock<IIsItemNameUniqueBusinessRule> _isNameUniqueBusinessRuleMock = new();
    private readonly Mock<IItemSizeMinAmountValidator> _itemSizeMinAmountValidatorMock = new();
    private readonly Mock<IItemSizeMinAmountComposer> _sizeMinAmountComposerMock = new();
    private readonly Mock<IAuthenticatedUserContext> _authenticatedUserContextMock = new();
    private readonly Mock<IGetMerchItemByIdDbQuery> _getMerchItemByIdMock = new();
    private readonly Mock<IMerchDbContext> _dbContextMock = new();
    private readonly Mock<IClockService> _clockServiceMock = new();

    private readonly UpdateMerchItemRequest _handler;

    private readonly CancellationToken _token = CancellationToken.None;

    public UpdateMerchItemRequestTests()
    {
        _handler = new UpdateMerchItemRequest(
            _isNameUniqueBusinessRuleMock.Object,
            _itemSizeMinAmountValidatorMock.Object,
            _sizeMinAmountComposerMock.Object,
            _authenticatedUserContextMock.Object,
            _getMerchItemByIdMock.Object,
            _dbContextMock.Object,
            _clockServiceMock.Object);
    }

    [Fact]
    public async Task HandleWhenMerchItemNotFoundThrowsNotFoundException()
    {
        //Arrange
        var command = new UpdateMerchItemRequest.Command(new UpdateItemDto(1, "", null, null, null, false, null));

        _getMerchItemByIdMock
            .Setup(x => x.ExecuteAsync(command.RequestDto.Id, _token))
            .Returns(null);

        //Act
        var action = () => _handler.Handle(command, _token);

        //Assert
        await action.Should().ThrowAsync<EntityNotFoundException>()
            .WithMessage(PortalMessages.MerchItemNotFound);
    }


    [Fact]
    public async Task HandleWhenMerchItemArchivedThrowsInvalidDataAppException()
    {
        //Arrange
        var existing = new ItemEntity
        {
            Id = 1,
            Name = "Test",
            Status = MerchItemStatus.Archived
        };

        var command = new UpdateMerchItemRequest.Command(new UpdateItemDto(existing.Id, "", null, null, null, false, null));

        _getMerchItemByIdMock
            .Setup(x => x.ExecuteAsync(command.RequestDto.Id, _token))
            .Returns(new ValueTask<ItemEntity?>(existing));

        //Act
        var action = () => _handler.Handle(command, _token);

        //Assert
        await action.Should()
            .ThrowAsync<InvalidDataAppException>()
            .WithMessage(PortalMessages.MerchItemIsNotAvailable);
    }
}

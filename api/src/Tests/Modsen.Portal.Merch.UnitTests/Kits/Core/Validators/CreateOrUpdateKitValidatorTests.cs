using Modsen.Portal.Core;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Services.BusinessRules;
using Modsen.Portal.Merch.Requests.Kits.Core.BusinessRules;
using Modsen.Portal.Merch.Requests.Kits.Core.CoreDto;
using Modsen.Portal.Merch.Requests.Kits.Core.Validators;
using Modsen.Portal.Merch.Requests.Kits.Requests.RequestDto;
using Moq;

namespace Modsen.Portal.Merch.UnitTests.Kits.Core.Validators;

public class CreateOrUpdateKitValidatorTests
{
    private readonly Mock<IAllKitMinAmountsListedBusinessRule> _allKitMinAmountsListedBusinessRuleMock = new();
    private readonly Mock<IAreCompoundsValidBusinessRule> _areCompoundsValidBusinessRuleMock = new();
    private readonly Mock<IKitNameUniqueBusinessRule> _kitNameUniqueBusinessRuleMock = new();

    private readonly CreateOrUpdateKitValidator _validator;
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public CreateOrUpdateKitValidatorTests()
    {
        _validator = new CreateOrUpdateKitValidator(
            _allKitMinAmountsListedBusinessRuleMock.Object,
            _areCompoundsValidBusinessRuleMock.Object,
            _kitNameUniqueBusinessRuleMock.Object);
    }

    [Fact]
    public async Task ValidateAsyncWhenNameIsNotUniqueThrowsInvalidDataAppExceptionWithMessage()
    {
        //Arrange
        _kitNameUniqueBusinessRuleMock
            .Setup(rule => rule.IsCorrespondsToBusiness(It.IsAny<string>(), It.IsAny<int?>(), _cancellationToken))
            .ReturnsAsync(false);

        var request = new CreateOrUpdateKitValidatorCoreDto(
            "name",
            null,
            new List<AddKitCompoundDto>(),
            new List<KitMinAmountDto>());

        //Act
        var action = () => _validator.ValidateAsync(request, _cancellationToken);

        //Assert
        await action.Should().ThrowAsync<InvalidDataAppException>(PortalMessages.MerchKitNameExists);

    }

    [Fact]
    public async Task ValidateAsyncWhenCompoundsAreNotValidThrowsInvalidDataAppExceptionWithMessage()
    {
        //Arrange
        _kitNameUniqueBusinessRuleMock
            .Setup(rule => rule.IsCorrespondsToBusiness(It.IsAny<string>(), It.IsAny<int?>(), _cancellationToken))
            .ReturnsAsync(true);

        _areCompoundsValidBusinessRuleMock
            .Setup(rule => rule.IsCorrespondsToBusiness(It.IsAny<List<AddKitCompoundDto>>(), _cancellationToken))
            .ReturnsAsync(BusinessRuleResult.Fail(PortalMessages.OnlyOneClothingInKit));

        var request = new CreateOrUpdateKitValidatorCoreDto(
            "name",
            null,
            new List<AddKitCompoundDto>(),
            new List<KitMinAmountDto>());

        //Act
        var action = () => _validator.ValidateAsync(request, _cancellationToken);

        //Assert
        await action.Should().ThrowAsync<InvalidDataAppException>(PortalMessages.OnlyOneClothingInKit);

    }

    [Fact]
    public async Task ValidateAsyncWhenNotAllStoragesAreListedThrowsInvalidDataAppExceptionWithMessage()
    {
        //Arrange
        _kitNameUniqueBusinessRuleMock
            .Setup(rule => rule.IsCorrespondsToBusiness(It.IsAny<string>(), It.IsAny<int?>(), _cancellationToken))
            .ReturnsAsync(true);

        _areCompoundsValidBusinessRuleMock
            .Setup(rule => rule.IsCorrespondsToBusiness(It.IsAny<List<AddKitCompoundDto>>(), _cancellationToken))
            .ReturnsAsync(BusinessRuleResult.Success);

        _allKitMinAmountsListedBusinessRuleMock
            .Setup(rule => rule.IsCorrespondsToBusiness(It.IsAny<List<KitMinAmountDto>>(), _cancellationToken))
            .ReturnsAsync(false);

        var request = new CreateOrUpdateKitValidatorCoreDto(
            "name",
            null,
            new List<AddKitCompoundDto>(),
            new List<KitMinAmountDto>());

        //Act
        var action = () => _validator.ValidateAsync(request, _cancellationToken);

        //Assert
        await action.Should().ThrowAsync<InvalidDataAppException>(PortalMessages.NotAllStoragesAreListed);

    }
}

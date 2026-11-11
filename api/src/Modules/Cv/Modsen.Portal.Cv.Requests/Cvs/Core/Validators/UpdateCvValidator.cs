using Modsen.Portal.Core.Context;
using Modsen.Portal.Cv.DataAccess.Npgsql;
using Modsen.Portal.Cv.Requests.Cvs.Core.BusinessRules;
using Modsen.Portal.Cv.Requests.Cvs.Core.CoreDto;

namespace Modsen.Portal.Cv.Requests.Cvs.Core.Validators;

internal interface IUpdateCvValidator
{
    Task ValidateAsync(UpdateCvValidatorCoreDto coreDto, CancellationToken cancellationToken);
}

internal class UpdateCvValidator : BaseCvValidator, IUpdateCvValidator
{
    public UpdateCvValidator(
        ICvNameUniqueWithinUserBusinessRule cvNameUniqueWithinUserBusinessRule,
        IAuthenticatedUserContext authenticatedUserContext,
        ICvDbContext cvDbContext)
        : base(cvNameUniqueWithinUserBusinessRule, authenticatedUserContext, cvDbContext) { }

    public Task ValidateAsync(UpdateCvValidatorCoreDto coreDto, CancellationToken cancellationToken)
    {
        return BaseValidate(coreDto, coreDto.Id, cancellationToken);
    }
}

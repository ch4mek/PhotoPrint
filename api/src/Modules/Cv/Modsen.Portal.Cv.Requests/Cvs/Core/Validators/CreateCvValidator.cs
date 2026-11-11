using Modsen.Portal.Core.Context;
using Modsen.Portal.Cv.DataAccess.Npgsql;
using Modsen.Portal.Cv.Requests.Cvs.Core.BusinessRules;
using Modsen.Portal.Cv.Requests.Cvs.Core.CoreDto;

namespace Modsen.Portal.Cv.Requests.Cvs.Core.Validators;

internal interface ICreateCvValidator
{
    Task ValidateAsync(CreateCvValidatorCoreDto coreDto, CancellationToken cancellationToken);
}

internal class CreateCvValidator : BaseCvValidator, ICreateCvValidator
{
    public CreateCvValidator(
        ICvNameUniqueWithinUserBusinessRule cvNameUniqueWithinUserBusinessRule,
        IAuthenticatedUserContext authenticatedUserContext,
        ICvDbContext cvDbContext)
        : base(cvNameUniqueWithinUserBusinessRule, authenticatedUserContext, cvDbContext) { }

    public Task ValidateAsync(CreateCvValidatorCoreDto coreDto, CancellationToken cancellationToken)
    {
        return BaseValidate(coreDto, cvId: null, cancellationToken);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modsen.Portal.Finance.UnitTests.BusinessRules;

public class CanFinanceUserEditRateBusinessRuleTests
{
    private readonly Mock<IAuthenticatedUserContext> _authenticatedUserContext;
    private readonly Mock<IIdentityContract> _identityContract;
    private readonly Mock<IFinanceDbContext> _dbContext;
}

using Modsen.Portal.Core;

namespace Modsen.Portal.Identity.UnitTests.Users.Validators.TheoryDataProviders.Email;

public class ValidEmailsProvider : TheoryData<string>
{
    public ValidEmailsProvider()
    {
        var domain = "@modsen-software.com";
        const int minLength = PortalSharedConstants.EmailMinLength;
        const int maxLength = PortalSharedConstants.EmailMaxLength;

        var emailWithMinLength = new string('a', minLength - domain.Length) + domain;
        var emailWithMaxLength = new string('a', maxLength - domain.Length) + domain;
        var emailWithAllowedSymbols = $"_-.ab{domain}";

        Add(emailWithMaxLength);
        Add(emailWithMinLength);
        Add(emailWithAllowedSymbols);
    }
}

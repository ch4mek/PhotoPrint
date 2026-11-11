using Modsen.Portal.Core;

namespace Modsen.Portal.Identity.UnitTests.Users.Validators.TheoryDataProviders.Email;

public class InvalidEmailsProvider : TheoryData<string>
{
    public InvalidEmailsProvider()
    {
        const string emailWithWrongDomain = "email@modasen-software.com";
        const string emailWithInvalidStructure = "email";
        const string emailWithNonEnglishLetters = "почта@modsen-software.com";
        const string emailWithMoreThanOneAt = "email@@modsen-software.com";

        var domain = "@modsen-software.com";
        const int minLength = PortalSharedConstants.EmailMinLength;
        const int maxLength = PortalSharedConstants.EmailMaxLength;

        var emailWithLengthLessThanMin = new string('a', minLength - domain.Length - 1) + domain;
        var emailWithLengthMoreThanMax = new string('a', maxLength - domain.Length + 1) + domain;

        Add(emailWithWrongDomain);
        Add(emailWithInvalidStructure);
        Add(emailWithNonEnglishLetters);
        Add(emailWithMoreThanOneAt);
        Add(emailWithLengthLessThanMin);
        Add(emailWithLengthMoreThanMax);
    }
}

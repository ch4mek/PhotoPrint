using System.Globalization;
using System.Text.RegularExpressions;
using FluentValidation;
using Microsoft.AspNetCore.OData.Query;
using Modsen.Portal.Core.Utils.Extensions;

namespace Modsen.Portal.Core.UseCases.ValidationRules;

public static partial class ValidationRulesExtensions
{
    private const string s_emailDomain = "@modsen-software.com";

    /// <summary>
    /// Text_Type_1 in validation documentation
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="ruleBuilder"></param>
    /// <returns></returns>
    public static IRuleBuilderOptions<T, string> AlphabeticWithHyphensSpaces<T>(this IRuleBuilder<T, string?> ruleBuilder)
    {
        return ruleBuilder
            .Length(2, 250)
            .Matches(@"^[A-Za-zА-ЯЁа-яё\- ]+$")
            .WithMessage("The string must contain only letters of the Russian and English alphabets, uppercase/lowercase letters, hyphens and spaces");
    }

    /// <summary>
    /// Text_Type_1.1 in validation documentation
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="ruleBuilder"></param>
    /// <returns></returns>
    public static IRuleBuilderOptions<T, string> LongAlphabeticWithHyphensSpaces<T>(this IRuleBuilder<T, string?> ruleBuilder)
    {
        return ruleBuilder
            .Length(5, 501)
            .Matches(@"^[A-Za-zА-ЯЁа-яё\- ]+$")
            .WithMessage("The string must contain only letters of the Russian and English alphabets, uppercase/lowercase letters, hyphens and spaces");
    }

    /// <summary>
    /// Text_Type_1.2 in validation documentation
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="ruleBuilder"></param>
    /// <returns></returns>
    public static IRuleBuilderOptions<T, string> CyrilicAlphabeticWithHyphensSpaces<T>(this IRuleBuilder<T, string?> ruleBuilder)
    {
        return ruleBuilder
            .Length(2, 250)
            .Matches(@"^[А-ЯЁа-яё\- ]+$")
            .WithMessage("The string must contain only letters of the Russian alphabet, uppercase/lowercase letters, hyphens and spaces");
    }

    /// <summary>
    /// Text_Type_2 in validation documentation
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="ruleBuilder"></param>
    /// <returns></returns>
    public static IRuleBuilderOptions<T, string> AlphabeticWithPunctuationChars<T>(this IRuleBuilder<T, string?> ruleBuilder)
    {
        return ruleBuilder
            .Length(2, 250)
            .Matches(@"^[A-Za-zА-ЯЁа-яё\- ,.\:\/|&;_'\(\)\[\]<>{}\\]+$")
            .WithMessage("The string must contain only letters of the Russian and English alphabets, uppercase and lowercase letters, hyphens, spaces and the following characters: , . : ; _ ' ( ) [ ] { } < > & / \\ |");
    }

    /// <summary>
    /// Text_Type_3 in validation documentation
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="ruleBuilder"></param>
    /// <returns></returns>
    public static IRuleBuilderOptions<T, string> AlphanumericWithSpecialChars<T>(this IRuleBuilder<T, string?> ruleBuilder)
    {
        return ruleBuilder
            .Length(2, 250)
            .Matches(@"^[A-Za-zА-ЯЁа-яё\- ,.\:;_'()\[\]0-9\+\@\#<>{}]+$")
            .WithMessage("The string must contain only letters of the Russian and English alphabets, uppercase and lowercase letters, hyphens, spaces, punctuation marks, numbers and the following characters: + @ # ( ) [ ] { } < >");
    }


    public static IRuleBuilderOptions<T, string?> SpacesOrPunctuationSymbolsRule<T>(this IRuleBuilder<T, string?> ruleBuilder)
    {
        return ruleBuilder
            .Must(x => x.IsNullOrEmpty() || x.Any(char.IsLetterOrDigit))
            .WithMessage("The string cannot contain only spaces or/and punctuation symbols");
    }
    /// <summary>
    /// Text_Type_4 in validation documentation
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="ruleBuilder"></param>
    /// <returns></returns>
    public static IRuleBuilderOptions<T, string> ShortTextType<T>(this IRuleBuilder<T, string?> ruleBuilder)
    {
        return ruleBuilder
            .Length(2, 250);
    }

    /// <summary>
    /// Long_text_Type_1 in validation documentation
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="ruleBuilder"></param>
    /// <returns></returns>
    public static IRuleBuilderOptions<T, string> LongTextType<T>(this IRuleBuilder<T, string?> ruleBuilder)
    {
        return ruleBuilder
            .Length(2, 1000)
            .WithMessage("The string length must be between 2 and 1000 characters");
    }

    /// <summary>
    /// Text tupe for Theses, Employee's theses, Agreements for Conversations
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="ruleBuilder"></param>
    /// <returns></returns>
    public static IRuleBuilderOptions<T, string> ConversationThesesEmployeesThesesAgreementsTextType<T>(this IRuleBuilder<T, string?> ruleBuilder)
    {
        return ruleBuilder
            .Length(2, 5000)
            .WithMessage("The string length must be between 2 and 5000 characters");
    }

    /// <summary>
    /// Phone_number_Type_1 in validation documentation
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="ruleBuilder"></param>
    /// <returns></returns>
    public static IRuleBuilderOptions<T, string> PhoneNumberType<T>(this IRuleBuilder<T, string?> ruleBuilder)
    {
        return ruleBuilder
            .Matches(@"^\+\d{1,14}$")
            .WithMessage("Phone number should have only + and positive integers. Maximum length of phone number is 15 characters");
    }

    /// <summary>
    /// Numbers_type_1 in validation documentation
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="ruleBuilder"></param>
    /// <returns></returns>
    public static IRuleBuilderOptions<T, int> PositiveDigits<T>(this IRuleBuilder<T, int> ruleBuilder)
    {
        return ruleBuilder
            .GreaterThan(0)
            .LessThan(10000);
    }

    /// <summary>
    /// Numbers_type_3 in validation documentation
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="ruleBuilder"></param>
    /// <returns></returns>
    public static IRuleBuilderOptions<T, int> Digits<T>(this IRuleBuilder<T, int> ruleBuilder)
    {
        return ruleBuilder
            .GreaterThanOrEqualTo(0)
            .LessThan(10000);
    }

    /// <summary>
    /// Link in validation documentation
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="ruleBuilder"></param>
    /// <returns></returns>
    public static IRuleBuilderOptions<T, string?> Link<T>(this IRuleBuilder<T, string?> ruleBuilder)
    {
        return ruleBuilder
            .Must(value =>
                    value?.Length is <= 550 and >= 1 &&
                    Uri.TryCreate(value, UriKind.Absolute, out _))
            .WithMessage("Invalid link format");
    }

    public static IRuleBuilderOptions<T, string?> SpacesRule<T>(this IRuleBuilder<T, string?> ruleBuilder)
    {
        return ruleBuilder
            .Must(x => x.IsNullOrEmpty() || !x.All(char.IsWhiteSpace))
            .WithMessage("Word contains only spaces!");
    }

    public static IRuleBuilderOptions<T, string?> LongSpaceRule<T>(this IRuleBuilder<T, string?> ruleBuilder)
    {
        return ruleBuilder
        .Must(x =>
        {
            if (x.IsNullOrWhiteSpace())
            {
                return false;
            }

            var trimmedValue = x.Trim();

            return trimmedValue.Length is >= 1 and <= 1000;
        })
        .WithMessage("The string length must be 1 and 1000 characters and not contain only spaces");
    }

    public static IRuleBuilderOptions<T, string?> HyphensSpacesRule<T>(this IRuleBuilder<T, string?> ruleBuilder)
    {
        return ruleBuilder
            .Must(value => value == null || !Regex.IsMatch(value, "^[ -]*$", RegexOptions.None, TimeSpan.FromSeconds(1)))
            .WithMessage("Word contains only spaces/Hypnoses!");
    }

    public static IRuleBuilderOptions<T, string?> BeEmptyOrInLength<T>(this IRuleBuilder<T, string?> ruleBuilder, int minLength, int maxLength)
    {
        return ruleBuilder
            .Must(x => x.IsNullOrEmpty() || x.Length >= minLength && x.Length <= maxLength);
    }

    public static IRuleBuilderOptions<T, string?> BeEmptyOrBeLongTextType<T>(this IRuleBuilder<T, string?> ruleBuilder)
    {
        return ruleBuilder
            .BeEmptyOrInLength(2, 1000)
            .WithMessage("The string length must be empty or between 2 and 1000 characters");
    }

    public static IRuleBuilderOptions<T, string?> EmailRuleWithDomain<T>(this IRuleBuilder<T, string?> ruleBuilder)
    {
        return ruleBuilder
            .EmailRule()
            .Must(x => x?.EndsWith(s_emailDomain, StringComparison.CurrentCulture) == true);
    }

    public static IRuleBuilderOptions<T, string?> EmailRule<T>(this IRuleBuilder<T, string?> ruleBuilder)
    {
        return ruleBuilder
            .Length(PortalSharedConstants.EmailMinLength, PortalSharedConstants.EmailMaxLength)
            .SpacesOrPunctuationSymbolsRule()
            .EmailAddress()
            .Matches("^(?!.*\\.{2})(?!.*@{2})(?!.*\\s).*$");
    }

    public static IRuleBuilderOptions<T, string?> EmailRuleWithCustomLength<T>(this IRuleBuilder<T, string?> ruleBuilder, int minLength, int maxLength)
    {
        return ruleBuilder
            .Length(minLength, maxLength)
            .SpacesOrPunctuationSymbolsRule()
            .EmailAddress()
            .Matches("^[A-Za-z0-9.@_-]+$")
            .Matches("^(?!.*\\.{2})(?!.*@{2})(?!.*\\s).*$");
    }

    /// <summary>
    /// Email_Type_2 in validation documentation
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="ruleBuilder"></param>
    /// <returns></returns>
    public static IRuleBuilderOptions<T, string?> EmailType2Rule<T>(this IRuleBuilder<T, string?> ruleBuilder)
    {
        return ruleBuilder
            .Length(PortalSharedConstants.EmailType2MinLength, PortalSharedConstants.EmailMaxLength)
            .SpacesOrPunctuationSymbolsRule()
            .EmailAddress()
            .Matches(@"^[^@\s]+@[^@\s]+\.[^@\s]+$") // domain must contain a dot
            .Matches("^[A-Za-z0-9.@_-]+$")
            .Matches("^(?!.*\\.{2})(?!.*@{2})(?!.*\\s).*$");
    }

    public static IRuleBuilderOptions<T, string> EmailSubstringBeforeAtRule<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder
            .Matches("^[A-Za-z0-9.@_-]+$")
            .WithMessage("""Email substring before "@" symbol must contain only letters of the English alphabet, uppercase/lowercase letters, digits, dots, '@', '_' and '-' symbols""");
    }

    public static IRuleBuilderOptions<T, string?> HexColorFormatRule<T>(this IRuleBuilder<T, string?> ruleBuilder)
    {
        return ruleBuilder
            .Must(color =>
                color.IsNullOrWhiteSpace() ||
                GetHexColorRegex().IsMatch(color)
            )
            .WithMessage("Incorrect hex color format!");
    }

    public static IRuleBuilderOptions<T, string?> TimeZoneRule<T>(this IRuleBuilder<T, string?> ruleBuilder)
    {
        return ruleBuilder.Must(x =>
        {
            if (x.IsNullOrWhiteSpace())
            {
                return false;
            }

            try
            {
                _ = new RegionInfo(x);
                return true;
            }
            catch (ArgumentException)
            {
                return false;
            }
        }).WithMessage("Incorrect timezone code!");
    }

    public static IRuleBuilderOptions<T, decimal> Money<T>(this IRuleBuilder<T, decimal> ruleBuilder)
    {
        return ruleBuilder.Must(number =>
        {
            if (decimal.IsNegative(number))
            {
                return false;
            }

            if ((double)number >= 1E+10)
            {
                return false;
            }

            if ((long)(number * 100) != (long)Math.Ceiling(number * 100))
            {
                return false;
            }

            return true;
        }).WithMessage("Number is not in valid money format");
    }

    public static void DateRule<T>(this IRuleBuilder<T, DateTime> ruleBuilder)
    {
        ruleBuilder
            .Must(date => date.TimeOfDay.Equals(TimeSpan.Zero))
            .WithMessage("Time must be zero");
    }

    public static IRuleBuilderOptions<T, string?> LinkRule<T>(this IRuleBuilder<T, string?> ruleBuilder)
    {
        var pattern = @"^https?:\/\/[^\/\s]+\.[^\/\s]+.*$";

        return ruleBuilder
            .Must(url => url is null || Regex.IsMatch(url, pattern, RegexOptions.None, TimeSpan.FromSeconds(1)))
            .WithMessage("Link should be started with http(s)://<example>.<example>");
    }

    public static IRuleBuilderOptions<T, ODataRawQueryOptions> DivisiblePaginationRule<T>(
        this IRuleBuilder<T, ODataRawQueryOptions> ruleBuilder)
    {
        return ruleBuilder
            .Must(options =>
        {
            if (!int.TryParse(options.Top, out var top) ||
                !int.TryParse(options.Skip, out var skip))
            {
                return false;
            }

            return top > 0 && skip > -1 && skip % top == 0;
        }).WithMessage("Invalid pagination");
    }

    public static IRuleBuilderOptions<T, ODataRawQueryOptions> PaginationRule<T>(
        this IRuleBuilder<T, ODataRawQueryOptions> ruleBuilder)
    {
        return ruleBuilder.Must(options =>
        {
            if (!int.TryParse(options.Top, out var top) ||
                !int.TryParse(options.Skip, out var skip))
            {
                return false;
            }

            return top > 0 && skip > -1;
        }).WithMessage("Invalid pagination");
    }

    [GeneratedRegex("^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{3})$")]
    private static partial Regex GetHexColorRegex();

    public static IRuleBuilderOptions<T, string?> BeNullOrInLength<T>(this IRuleBuilder<T, string?> ruleBuilder, int minLength, int maxLength)
    {
        return ruleBuilder
            .Must(x => x == null || (x.Length >= minLength && x.Length <= maxLength));
    }

    public static IRuleBuilderOptions<T, string?> BeNullOrBeLongTextType<T>(this IRuleBuilder<T, string?> ruleBuilder)
    {
        return ruleBuilder
            .BeNullOrInLength(2, 1000)
            .WithMessage("The string must be null or have a length between 2 and 1000 characters.");
    }

    public static IRuleBuilderOptions<T, string?> BeEmptyNullOrBeLongTextType<T>(this IRuleBuilder<T, string?> ruleBuilder)
    {
        return ruleBuilder
            .Must(x => x.IsNullOrEmpty() || x.Length is >= 2 and <= 1000)
            .WithMessage("The string must be null or Empty or have a length between 2 and 1000 characters.");
    }

    public static IRuleBuilderOptions<T, string?> BeEmptyNullOrBeConversationThesesEmployeesThesesAgreementsTextType<T>(this IRuleBuilder<T, string?> ruleBuilder)
    {
        return ruleBuilder
            .Must(x => x.IsNullOrEmpty() || x.Length is >= 2 and <= 5000)
            .WithMessage("The string must be null or Empty or have a length between 2 and 5000 characters.");
    }

    public static IRuleBuilderOptions<T, string?> NotOnlyPunctuationMarks<T>(this IRuleBuilder<T, string?> ruleBuilder)
    {
        return ruleBuilder
            .Must(text =>
                !text.IsNullOrWhiteSpace() &&
                !Regex.IsMatch(text, @"^[\s\p{P}|]+$", RegexOptions.None, TimeSpan.FromSeconds(1))
            );
    }


    /// <summary>
    /// Long_text_Type_1 in validation documentation
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="ruleBuilder"></param>
    /// <returns></returns>
    public static IRuleBuilderOptions<T, string?> LongTextType1<T>(this IRuleBuilder<T, string?> ruleBuilder, bool required)
    {
        var onlyPunctuationPattern = @"^[\s.,:;_\-'\[\](){}<>«»\""\/\\]+$";

        var builder = ruleBuilder;

        if (required)
        {
            builder = builder
                .NotEmpty()
                .WithMessage("Please enter a valid string");
        }

        return builder
            .Must(value =>
                value == null ||
                (value.Length >= 2 && value.Length <= 1000))
            .WithMessage("The string length must be between 2 and 1000 characters")
            .Must(value =>
                value == null ||
                !Regex.IsMatch(value, onlyPunctuationPattern, RegexOptions.None, TimeSpan.FromSeconds(1)))
            .WithMessage("Please enter a valid string");
    }
}

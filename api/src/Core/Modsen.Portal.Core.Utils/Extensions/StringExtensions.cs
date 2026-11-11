using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Modsen.Portal.Core.Utils.Extensions;

public static class StringExtensions
{
    public static string? UpperInvariantNormalize(this string? value)
    {
        return value?.Normalize().ToUpperInvariant();
    }

    public static bool IsNullOrEmpty([NotNullWhen(false)] this string? value)
    {
        return string.IsNullOrEmpty(value);
    }

    public static bool IsNullOrWhiteSpace([NotNullWhen(false)] this string? value)
    {
        return string.IsNullOrWhiteSpace(value);
    }

    public static string WithFirstCharToUpper(this string? value)
    {
        if (value?.Length == 1)
        {
            return value.ToUpperInvariant();
        }

        return value?.Length > 0
            ? value[0].ToString(CultureInfo.InvariantCulture).ToUpperInvariant() + value[1..]
            : string.Empty;
    }

    public static string? GetValueOrNullIfEmpty(this string? value)
    {
        return value.IsNullOrEmpty() ? null : value;
    }
}

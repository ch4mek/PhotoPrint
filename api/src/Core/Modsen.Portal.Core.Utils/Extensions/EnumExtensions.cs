using System.ComponentModel;
using System.Reflection;

namespace Modsen.Portal.Core.Utils.Extensions;

public static class EnumExtensions
{
    public static string GetDescription(this Enum enumValue)
    {
        ArgumentNullException.ThrowIfNull(enumValue);

        var enumValueStr = enumValue.ToString();
        var fieldInfo = enumValue.GetType().GetField(enumValueStr);

        if (fieldInfo == null)
        {
            if (!enumValueStr.IsNullOrEmpty())
            {
                return !int.TryParse(enumValueStr, out _)
                    ? enumValueStr
                    : string.Empty;
            }

            return string.Empty;
        }

        var descriptionAttributes = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(
            typeof(DescriptionAttribute),
            inherit: false);

        return descriptionAttributes.Length > 0
            ? descriptionAttributes[0].Description
            : enumValue.ToString();
    }

    public static TEnum ParseEnumValueByDescription<TEnum>(string description) where TEnum : Enum
    {
        foreach (TEnum enumValue in Enum.GetValues(typeof(TEnum)))
        {
            var fieldInfo = enumValue.GetType().GetField(enumValue.ToString())
                ?? throw new ArgumentException($"No enum value found for type {typeof(TEnum).Name}");

            var descriptionAttribute = fieldInfo.GetCustomAttribute<DescriptionAttribute>();

            if (descriptionAttribute != null &&
                descriptionAttribute.Description == description)
            {
                return enumValue;
            }
        }

        throw new ArgumentException($"No enum value found with the description '{description}' for type {typeof(TEnum).Name}");
    }

    public static IReadOnlyCollection<EnumModel> GetValues<T>() where T : Enum
    {
        return GetValues(Array.Empty<T>());
    }

    public static IReadOnlyCollection<EnumModel> GetValues<T>(params T[] exceptValues) where T : Enum
    {
        return Enum.GetValues(typeof(T))
            .Cast<T>()
            .Except(exceptValues)
            .Select(itemType => new EnumModel
            {
                Id = itemType.ToString(),
                Name = Enum.GetName(typeof(T), itemType),
                Description = itemType.GetDescription()
            }).ToList();
    }
}

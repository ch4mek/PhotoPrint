using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Modsen.Portal.Core.Utils.Extensions;

namespace Modsen.Portal.Core.Utils;

public sealed class EnumConverter<TEnum> : ValueConverter<TEnum, string>
    where TEnum : Enum
{
    public EnumConverter(ConverterMappingHints? mappingHints = null)
        : base(
            enumValue => enumValue.GetDescription(),
            stringValue => EnumExtensions.ParseEnumValueByDescription<TEnum>(stringValue),
            mappingHints)
    {
    }
}

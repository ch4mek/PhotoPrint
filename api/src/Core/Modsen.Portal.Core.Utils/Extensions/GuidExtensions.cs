namespace Modsen.Portal.Core.Utils.Extensions;

public static class GuidExtensions
{
    public static string ToAlphabetic(this Guid value)
    {
        var bytes = value.ToByteArray();
        var result = new char[32];

        for (var i = 0; i < 16; i++)
        {
            result[i * 2] = (char)('A' + (bytes[i] >> 4));
            result[i * 2 + 1] = (char)('A' + (bytes[i] & 0x0F));
        }

        return new string(result);
    }
}

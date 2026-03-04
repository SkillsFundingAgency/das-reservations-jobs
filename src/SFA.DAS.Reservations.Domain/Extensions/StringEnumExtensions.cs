using System;

namespace SFA.DAS.Reservations.Domain.Extensions;

public static class StringEnumExtensions
{
    public static TEnum? ToEnumValue<TEnum>(this string? value) where TEnum : struct, Enum
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        var trimmedValue = value.Trim();
        
        if (Enum.TryParse<TEnum>(trimmedValue, ignoreCase: true, out var result))
            return result;

        throw new ArgumentOutOfRangeException(nameof(value), value, $"Unknown {typeof(TEnum).Name} value");
    }
    
    public static string? ToEnumString<TEnum>(this TEnum? value) where TEnum : struct, Enum
    {
        return value?.ToString();
    }
}
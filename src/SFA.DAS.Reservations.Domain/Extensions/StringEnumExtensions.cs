using System;

namespace SFA.DAS.Reservations.Domain.Extensions;

public static class StringEnumExtensions
{
    /// <summary>
    /// Converts a string to the specified enum type. Returns null if the string is null or empty.
    /// Throws ArgumentOutOfRangeException if the string doesn't match any enum value.
    /// </summary>
    public static TEnum? ToEnumValue<TEnum>(this string? value) where TEnum : struct, Enum
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        var trimmedValue = value.Trim();
        
        if (Enum.TryParse<TEnum>(trimmedValue, ignoreCase: true, out var result))
            return result;

        throw new ArgumentOutOfRangeException(nameof(value), value, $"Unknown {typeof(TEnum).Name} value");
    }

    /// <summary>
    /// Converts an enum to its string representation. Returns null if the enum is null.
    /// </summary>
    public static string? ToEnumString<TEnum>(this TEnum? value) where TEnum : struct, Enum
    {
        return value?.ToString();
    }
}
using System;

namespace MechEngineer.Helper;

internal static class EnumHelper
{
    internal static bool TryParse<TEnum>(string name, out TEnum result, bool ignoreCase = false) where TEnum : struct
    {
        try
        {
            result = GetValue<TEnum>(name, ignoreCase);
        }
        catch
        {
            result = default;
            return false;
        }

        return true;
    }

    internal static TEnum GetValue<TEnum>(string name, bool ignoreCase = false)
    {
        return (TEnum)Enum.Parse(typeof(TEnum), name, ignoreCase);
    }
}
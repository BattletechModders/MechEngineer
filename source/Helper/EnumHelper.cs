using System;

namespace MechEngineer
{
    internal static class EnumHelper
    {
        internal static bool TryParse<TEnum>(string value, out TEnum result, bool ignoreCase = false) where TEnum : struct
        {
            try
            {
                result = (TEnum)Enum.Parse(typeof(TEnum), value, ignoreCase); 
            }
            catch
            {
                result = default;
                return false;
            }

            return true;
        }
    }
}
using System.Collections.Generic;
using System.Text;

namespace MechEngineer.Misc;

internal static class DictionarySerializer
{
    internal static string ToString(Dictionary<string, string> dictionary)
    {
        var sb = new StringBuilder();
        foreach (var keyvalue in dictionary)
        {
            sb.Append(keyvalue.Key);
            sb.Append("=");
            sb.Append(keyvalue.Value);
            sb.Append("&");
        }

        return sb.Length == 0 ? "" : sb.ToString(0, sb.Length - 1);
    }

    internal static Dictionary<string, string> FromString(string values)
    {
        var dictionary = new Dictionary<string, string>();
        foreach (var keyvalueText in values.Split('&'))
        {
            var keyvalueArray = keyvalueText.Split('=');
            if (keyvalueArray.Length < 2)
            {
                continue;
            }
            var key = keyvalueArray[0];
            var value = keyvalueArray[1];
            dictionary.Add(key, value);
        }

        return dictionary;
    }
}
using System.Linq;
using System.Text;

namespace MechEngineer.Misc;

internal static class ToStringBuilder
{
    internal static string ToString(object obj)
    {
        var sb = new StringBuilder();
        sb.Append("[");
        var type = obj.GetType();
        foreach (var field in type.GetFields().Where(f => f.IsDefined(typeof(ToStringAttribute), false)))
        {
            sb.Append(field.Name);
            sb.Append("=");
            sb.Append(field.GetValue(obj));
        }
        foreach (var property in type.GetProperties().Where(f => f.CanRead && f.IsDefined(typeof(ToStringAttribute), false)))
        {
            sb.Append(property.Name);
            sb.Append("=");
            sb.Append(property.GetValue(obj));
        }
        sb.Append("]");
        return sb.ToString();
    }
}
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace MechEngineer.Features.OverrideStatTooltips.Helper;

internal static class MethodFinder
{
    internal static T FindStatic<T>(Type? type, string name, T fallback) where T: Delegate
    {
        if (type == null)
        {
            return fallback;
        }

        var delegateMethod = typeof(T).GetMethod("Invoke")!;
        var parameterTypes = delegateMethod.GetParameters().Select(p => p.ParameterType).ToArray();

        var method = type.GetMethod(
            name,
            BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic,
            null,
            parameterTypes,
            Array.Empty<ParameterModifier>()
        );

        if (method == null || method.ReturnType != delegateMethod.ReturnType)
        {
            Log.Main.Warning?.Log($"Could not find matching method {name} with signature {delegateMethod} in type {type.FullName}");
            return fallback;
        }

        Log.Main.Trace?.Log($"Found method {name} with signature {delegateMethod} in type {type.FullName}");

        var parameterExpressions = delegateMethod.GetParameters()
            .Select(p => Expression.Parameter(p.ParameterType, p.Name))
            .ToList();

        return Expression.Lambda<T>
            (
                Expression.Call(null, method, parameterExpressions),
                parameterExpressions
            )
            .Compile();
    }
}
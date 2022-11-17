using System;
using HBS.Collections;
using MechEngineer.Features.AutoFix;

namespace MechEngineer.Helper;

public static class TagSetExtensions
{
    public static bool IgnoreAutofix(this TagSet set)
    {
        if (set == null)
        {
            Log.Main.Error?.Log("Found null tagset!");
            throw new NullReferenceException();
        }
        return set.ContainsAny(AutoFixerFeature.Shared.IgnoreAutofixTags);
    }
}

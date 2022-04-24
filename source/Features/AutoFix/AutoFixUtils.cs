using HBS.Collections;

namespace MechEngineer.Features.AutoFix;

internal class AutoFixUtils
{
    internal static bool IsIgnoredByTags(TagSet tags, string[] ignoredTags)
    {
        if (tags == null || ignoredTags == null)
        {
            return false;
        }
        var otherTags = new TagSet(ignoredTags);
        return tags.ContainsAny(otherTags, false);
    }
}
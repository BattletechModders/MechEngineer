using BattleTech.UI;
using CustomComponents;
using UnityEngine;

namespace MechEngineer.Helper;

internal static class ColorExtensions
{
    // TODO move to CC
    internal static void SetColorFromString(this UIColorRefTracker tracker, string value)
    {
        if (EnumHelper.TryParse<UIColor>(value, out var uiColor, true))
        {
            tracker.SetUIColor(uiColor);
        }
        else if (ColorUtility.TryParseHtmlString(value, out var customColor))
        {
            // TODO remove UIColor.Custom hardcoded parameter from CC
            tracker.SetCustomColor(UIColor.Custom, customColor);
        }
    }
}
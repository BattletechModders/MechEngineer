using BattleTech.UI;
using UnityEngine;
using UnityEngine.UI;

namespace MechEngineer.Features.MechLabSlots;

internal static class MechLabAutoZoom
{
    internal static void LoadMech(MechLabPanel mechLabPanel)
    {
        var finder = new MechLabLayoutFinder(mechLabPanel);

        var mechRectTransform = finder.ObjMech.GetComponent<RectTransform>();
        // Unity (?) does not handle layout propagation properly, so we need to force several layout passes here
        // also allows us to calculate stuff for auto zoom without waiting for regular layout passes
        LayoutRebuilder.ForceRebuildLayoutImmediate(mechRectTransform);
        LayoutRebuilder.ForceRebuildLayoutImmediate(mechRectTransform);
        LayoutRebuilder.ForceRebuildLayoutImmediate(mechRectTransform);

        mechRectTransform.anchorMin = new(mechRectTransform.anchorMin.x, 1);
        mechRectTransform.anchorMax = new(mechRectTransform.anchorMin.x, 1);
        mechRectTransform.pivot = new(mechRectTransform.pivot.x, 1);

        mechRectTransform.position = new(finder.ObjMech.position.x, finder.ObjActions.position.y, finder.ObjMech.position.z);

        {
            var confirmRectTransform = finder.ObjCancelConfirm.GetComponent<RectTransform>();

            var mechSize = mechRectTransform.sizeDelta.y;
            var targetSize = mechRectTransform.localPosition.y
                - 40 // repair button height
                - confirmRectTransform.localPosition.y + confirmRectTransform.sizeDelta.y; // save button bottom

            var scale = Mathf.Min(MechLabSlotsFeature.settings.ZoomMaximumScale, targetSize / mechSize);
            mechRectTransform.localScale = new(scale, scale, 1);

            Log.Main.Debug?.Log($"AutoZoom scale={scale} mechSize={mechSize} targetSize={targetSize}");
        }
    }
}

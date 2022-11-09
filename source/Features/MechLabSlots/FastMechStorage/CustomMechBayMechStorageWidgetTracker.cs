#nullable disable
using System.Collections.Generic;
using BattleTech.UI;

namespace MechEngineer.Features.MechLabSlots.FastMechStorage;

internal static class CustomMechBayMechStorageWidgetTracker
{
    private static readonly Dictionary<MechBayMechStorageWidget, CustomMechBayMechStorageWidget> Widgets = new();
    private static readonly Dictionary<UnityEngine.UI.ScrollRect, CustomMechBayMechStorageWidget> ScrollRects = new();
    internal static bool TryGet(MechBayMechStorageWidget widget, out CustomMechBayMechStorageWidget customWidget)
    {
        // DebugMechSelectorElement "uixPrfPanl_LC_mechUnit-Element"
        if (widget.itemPrefabName == DebugMechSelectorElement.MECH_ELEMENT_PREFAB)
        {
            customWidget = null;
            return false;
        }

        // MechBayPanel "uixPrfPanl_storageMechUnit-Element"
        // LanceConfigurationPanel "uixPrfPanl_LC_MechSlot"
        // SkirmishMechBay "uixPrfPanl_LC_mechUnit-Element"
        if (!Widgets.TryGetValue(widget, out customWidget))
        {
            customWidget = new(widget);
            Widgets[widget] = customWidget;
            ScrollRects[customWidget.GetScrollRect()] = customWidget;
        }
        return true;
    }
    internal static CustomMechBayMechStorageWidget Get(UnityEngine.UI.ScrollRect scrollRect)
    {
        return ScrollRects.TryGetValue(scrollRect, out var customWidget) ? customWidget : null;
    }
}
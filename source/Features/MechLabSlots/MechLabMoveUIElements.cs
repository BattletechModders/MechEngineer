using BattleTech.UI;
using UnityEngine;
using UnityEngine.UI;

namespace MechEngineer.Features.MechLabSlots;

internal static class MechLabMoveUIElements
{
    internal static void MoveMechUIElements(MechLabPanel panel)
    {
        MoveMechRoleInfo(panel);
        MoveViewMechButton(panel);

        var finder = new MechLabLayoutFinder(panel);
        if (MechLabSlotsFeature.Shared.Settings.HideHelpButton)
        {
            finder.ObjHelpBttn.gameObject.SetActive(false);
        }

        if (MechLabSlotsFeature.Shared.Settings.HideECMButton)
        {
            finder.ObjEcmBttn.gameObject.SetActive(false);
        }
    }

    internal static void MoveMechRoleInfo(MechLabPanel panel)
    {
        var armWidget = panel.rightArmWidget;

        var finder = new MechLabLayoutFinder(panel);
        var layout_details = finder.LayoutDetails ?? armWidget.transform.Find("layout_details");
        if (layout_details == null)
        {
            return;
        }

        var go = layout_details.gameObject;
        go.SetActive(!panel.IsSimGame);

        if (layout_details.parent == armWidget.transform)
        {
            return;
        }

        var arm = armWidget.transform.parent;
        MechLabLayoutUtils.EnableLayout(arm.gameObject);
        {
            var component = go.GetComponent<RectTransform>();
            component.pivot = new(0, 1);
            component.anchorMin = new(0, 0);
            component.anchorMax = new(0, 0);
            component.anchoredPosition = new(0, -40);
        }
        {
            var component = go.GetComponent<LayoutElement>() ?? go.AddComponent<LayoutElement>();
            component.ignoreLayout = true;
            component.enabled = true;
        }
        layout_details.transform.SetParent(arm.transform, false);
    }

    internal static void MoveViewMechButton(MechLabPanel panel)
    {
        var vb = panel.btn_mechViewerButton;
        var rect = vb.GetComponent<RectTransform>();
        // below works similar to OBJ_value
        rect.anchorMin = new(1, 1);
        rect.anchorMax = new(1, 1);
        rect.pivot = new(1, 1);
        rect.anchoredPosition = new(-32, -780);
    }
}

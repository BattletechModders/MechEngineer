using BattleTech.UI;
using UnityEngine;
using UnityEngine.UI;

namespace MechEngineer.Features.MechLabSlots;

internal class MechLabMoveUIElements
{
    internal static void MoveMechUIElements(MechLabPanel panel)
    {
        MoveMechRoleInfo(panel);
        MoveViewMechButton(panel);

        var Representation = panel.transform.GetChild("Representation");
        if (MechLabSlotsFeature.Shared.Settings.HideHelpButton)
        {
            Representation.GetChild("OBJ_helpBttn").gameObject.SetActive(false);
        }

        if (MechLabSlotsFeature.Shared.Settings.HideECMButton)
        {
            Representation.GetChild("OBJ_ECMBttn").gameObject.SetActive(false);
        }
    }

    internal static void MoveMechRoleInfo(MechLabPanel panel)
    {
        var armWidget = panel.rightArmWidget;

        var layout_details = panel.transform
                                 .GetChild("Representation")
                                 .GetChild("OBJ_mech")
                                 .GetChild("Centerline")
                                 .GetChild("layout_details")
                             ?? armWidget.transform.GetChild("layout_details");
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
        MechLabFixWidgetLayouts.EnableLayout(arm.gameObject);
        {
            var component = go.GetComponent<RectTransform>();
            component.pivot = new Vector2(0, 1);
            component.anchorMin = new Vector2(0, 0);
            component.anchorMax = new Vector2(0, 0);
            component.anchoredPosition = new Vector2(0, -40);
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
        rect.anchorMin = new Vector2(1, 1);
        rect.anchorMax = new Vector2(1, 1);
        rect.pivot = new Vector2(1, 1);
        rect.anchoredPosition = new Vector2(-32, -780);
    }
}
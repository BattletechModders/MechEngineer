using BattleTech.UI;
using UnityEngine;
using UnityEngine.UI;

namespace MechEngineer.Features.MechLabSlots
{
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
            var layout_details = panel.transform
                .GetChild("Representation")
                .GetChild("OBJ_mech")
                .GetChild("Centerline")
                .GetChild("layout_details");
            if (layout_details == null)
            {
                return;
            }

            var leftArmWidget = panel.leftArmWidget;
            var leftArm = leftArmWidget.gameObject.transform.parent;
            MechLabFixWidgetLayouts.EnableLayout(leftArm.gameObject);

            var vlg = leftArm.GetComponent<VerticalLayoutGroup>();
            vlg.padding = new RectOffset(0, 0, MechLabSlotsFeature.settings.MechLabArmTopPadding, 0);

            layout_details.transform.SetParent(leftArmWidget.transform, false);

            {
                var go = layout_details.gameObject;

                MechLabFixWidgetLayouts.EnableLayout(go);

                go.GetComponent<LayoutElement>().ignoreLayout = true;

                var rect = go.GetComponent<RectTransform>();
                //rect.localPosition = new Vector3(0, 0);
                rect.pivot = new Vector2(0, 0);
                rect.anchoredPosition = new Vector2(0, 0);
            }
        }
        
        internal static void MoveViewMechButton(MechLabPanel panel)
        {
            var adapter = new MechLabPanelAdapter(panel);
            var vb = adapter.btn_mechViewerButton;
            var rect = vb.GetComponent<RectTransform>();
            // below works similar to OBJ_value
            rect.anchorMin = new Vector2(1, 1);
            rect.anchorMax = new Vector2(1, 1);
            rect.pivot = new Vector2(1, 1);
            rect.anchoredPosition = new Vector2(-32, -780);
        }
    }
}

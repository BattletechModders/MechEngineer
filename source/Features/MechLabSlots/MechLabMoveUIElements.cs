using BattleTech.UI;
using MechEngineer.Helper;
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
            ChangeHardpointDirection(panel);

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

        internal static void ChangeHardpointDirection(MechLabPanel panel)
        {
            var layout_hardpoints = panel.transform
                .GetChild("Representation")
                .GetChild("OBJGROUP_LEFT")
                .GetChild("OBJ_meta")
                .GetChild("OBJ_status")
                .GetChild("layout_hardpoints");
            if (layout_hardpoints == null)
            {
                return;
            }

            var go = layout_hardpoints.gameObject;

            {
                var hlg = go.GetComponent<HorizontalLayoutGroup>();
                if (hlg != null)
                {
                    Object.DestroyImmediate(hlg);
                }
                else
                {
                    return;
                }
            }

            var vlg = go.GetComponent<VerticalLayoutGroup>() ?? go.AddComponent<VerticalLayoutGroup>();
            if (vlg != null)
            {
                vlg.spacing = 15;
                vlg.childControlHeight = true;
                vlg.childControlWidth = true;
                vlg.padding = new RectOffset(10, 10, 15, 15);
                vlg.childAlignment = TextAnchor.UpperRight;
                vlg.enabled = true;
            }

            foreach (var hlg in layout_hardpoints.GetComponentsInChildren<HorizontalLayoutGroup>())
            {
                hlg.childAlignment = TextAnchor.MiddleRight;
                hlg.enabled = true;
                foreach (var glg in hlg.transform.GetComponentsInChildren<GridLayoutGroup>())
                {
                    if (glg.gameObject.name == "TextContainer")
                    {
                        glg.cellSize = new Vector2(40, 20);
                        glg.enabled = true;
                    }
                }
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
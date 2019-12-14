using BattleTech.UI;
using UnityEngine;
using UnityEngine.UI;

namespace MechEngineer.Features.MechLabSlots
{
    internal class MechLabMoveMechRoleInfo
    {
        private static bool applied;
        internal static void MoveMechRoleInfo(MechLabPanel mechLabPanel)
        {
            if (applied)
            {
                return;
            }
            applied = true;

            var Representation = mechLabPanel.transform.GetChild("Representation");
            var OBJ_mech = Representation.GetChild("OBJ_mech");

            var layout_details = OBJ_mech
                .GetChild("Centerline")
                .GetChild("layout_details");

            if (layout_details != null)
            {
                var go = layout_details.gameObject;
                go.EnableLayout();
                go.GetComponent<LayoutElement>().ignoreLayout = true;

                var leftArm = OBJ_mech.GetChild("LeftArm");
                var vlg = leftArm.GetComponent<VerticalLayoutGroup>();
                vlg.padding = new RectOffset(0, 0, MechLabSlotsFeature.settings.MechLabArmTopPadding, 0);
                //layout_details.parent = leftArm;
                //layout_details.SetAsFirstSibling();

                var leftArmWidget = leftArm.GetChild(0);
                layout_details.SetParent(leftArmWidget, false);
                var rect = go.GetComponent<RectTransform>();
                rect.pivot = new Vector2(0, 0);
                rect.localPosition = new Vector3(0, 0);
            }
        }
    }
}

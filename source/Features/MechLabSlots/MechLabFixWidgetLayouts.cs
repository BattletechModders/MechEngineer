using BattleTech.UI;
using UnityEngine;
using UnityEngine.UI;

namespace MechEngineer.Features.MechLabSlots
{
    internal class MechLabFixWidgetLayouts
    {
        private static bool applied = false;
        internal static void FixWidgetLayouts(MechLabPanel mechLabPanel)
        {
            if (applied)
            {
                return;
            }
            applied = true;

            var Representation = mechLabPanel.transform.GetChild("Representation");
            var OBJ_mech = Representation.GetChild("OBJ_mech");

            foreach (Transform container in OBJ_mech)
            {
                {
                    var go = container.gameObject;
                    go.EnableLayout();
                    var component = go.GetComponent<ContentSizeFitter>() ?? go.AddComponent<ContentSizeFitter>();
                    component.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
                    component.enabled = true;
                }

                foreach (Transform widget in container)
                {
                    if (widget.GetComponent<MechLabLocationWidget>() == null)
                    {
                        continue;
                    }

                    widget.gameObject.EnableLayout();
                    widget.GetChild("layout_slots").gameObject.EnableLayout();
                }
            }

            {
                var go = OBJ_mech.gameObject;
                go.EnableLayout();
                var component = go.GetComponent<ContentSizeFitter>() ?? go.AddComponent<ContentSizeFitter>();
                component.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
                component.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
                component.enabled = true;
            }
        }
    }
}

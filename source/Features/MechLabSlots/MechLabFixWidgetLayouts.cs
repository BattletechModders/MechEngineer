using BattleTech.UI;
using UnityEngine;
using UnityEngine.UI;

namespace MechEngineer.Features.MechLabSlots
{
    internal class MechLabFixWidgetLayouts
    {
        internal static void FixMechLabLayouts(MechLabPanel panel)
        {
            // MechLabFixInfoWidgetLayout.FixInfoWidget(panel);
            FixMechLabLocationWidgetLayouts(panel);
        }

        private static void FixMechLabLocationWidgetLayouts(MechLabPanel mechLabPanel)
        {
            var Representation = mechLabPanel.transform.GetChild("Representation");
            var OBJ_mech = Representation.GetChild("OBJ_mech");

            foreach (Transform container in OBJ_mech)
            {
                {
                    var go = container.gameObject;
                    EnableLayout(go);
                    var component = go.GetComponent<ContentSizeFitter>() ?? go.AddComponent<ContentSizeFitter>();
                    component.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
                    component.enabled = true;
                    {
                        var clg = go.GetComponent<VerticalLayoutGroup>();
                        clg.padding = new RectOffset(0, 0, 0, 0);
                        clg.spacing = 56;
                    }
                }

                foreach (Transform widget in container)
                {
                    var widgetComponent = widget.GetComponent<MechLabLocationWidget>();
                    if (widgetComponent == null || CustomWidgetsFixMechLab.IsCustomWidget(widgetComponent))
                    {
                        continue;
                    }

                    EnableLayout(widget.gameObject);
                    EnableLayout(widget.GetChild("layout_slots").gameObject);

                    // fix different distances for lower bracket
                    var rect = widget
                        .GetChild("layout_bg")
                        .GetChild("bracket_btm")
                        .GetComponent<RectTransform>();
                    rect.anchoredPosition = new Vector2(0, 0);
                    rect.offsetMin = new Vector2(0, -2);
                    rect.offsetMax = new Vector2(0, 2);
                }
            }

            {
                var go = OBJ_mech.gameObject;
                EnableLayout(go);
                var component = go.GetComponent<ContentSizeFitter>() ?? go.AddComponent<ContentSizeFitter>();
                component.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
                component.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
                component.enabled = true;
            }
        }

        internal static void EnableLayout(GameObject gameObject)
        {
            {
                var component = gameObject.GetComponent<LayoutElement>() ?? gameObject.AddComponent<LayoutElement>();
                component.ignoreLayout = false;
                component.enabled = true;
            }

            {
                var component = gameObject.GetComponent<HorizontalLayoutGroup>();
                if (component != null)
                {
                    component.childForceExpandHeight = false;
                    component.childAlignment = TextAnchor.UpperCenter;
                    component.enabled = true;
                }
            }

            {
                var component = gameObject.GetComponent<VerticalLayoutGroup>();
                if (component != null)
                {
                    component.enabled = true;
                    component.childForceExpandHeight = false;
                    component.childForceExpandWidth = false;
                }
            }

            {
                var component = gameObject.GetComponent<GridLayoutGroup>();
                if (component != null)
                {
                    component.enabled = true;
                }
            }

            {
                var component = gameObject.GetComponent<ContentSizeFitter>();
                if (component != null)
                {
                    component.enabled = true;
                }
            }
        }
    }
}
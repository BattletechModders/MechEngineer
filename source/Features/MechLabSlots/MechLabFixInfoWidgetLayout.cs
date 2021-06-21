using BattleTech.UI;
using BattleTech.UI.TMProWrapper;
using FluffyUnderware.DevTools.Extensions;
using MechEngineer.Helper;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MechEngineer.Features.MechLabSlots
{
    public static class MechLabFixInfoWidgetLayout
    {
        public static void FixInfoWidget(MechLabPanel panel)
        {
            FixMechLabMechInfoWidgetLayout(panel);
            ChangeHardpointDirection(panel);
        }

        private static void FixMechLabMechInfoWidgetLayout(MechLabPanel panel)
        {
            var panelAdapter = new MechLabPanelAdapter(panel);
            var widgetAdapter = new MechLabMechInfoWidgetAdapter(panelAdapter.mechInfoWidget);
            var hardpoints = widgetAdapter.hardpoints;
            {
                var container = hardpoints[3].transform.parent;
                container.parent.GetChild("OBJ_stockBttn")?.gameObject?.Destroy();
                {
                    var jjparent = hardpoints[4].transform.parent;
                    if (jjparent != container)
                    {
                        hardpoints[4].transform.parent = container;
                        jjparent.gameObject.Destroy();
                    }
                }

                FixElementsContainerLayout(container.gameObject);
            }

            static void FixElementsContainerLayout(GameObject go)
            {
                go.GetComponent<Image>().sprite = null;
                go.GetComponent<RectTransform>().pivot = new Vector2(0, 1);
                var hlg = FixHorizontalLayoutGroup(go);
                hlg.spacing = 15;
                hlg.padding = new RectOffset(20, 15, 10, 10);
                FixContentSizeFitter(go);
            }

            static void FixElementLayout(MechLabHardpointElement element)
            {
                {
                    var go = element.gameObject;
                    go.GetComponent<Image>()?.Destroy();
                    FixRect(go);
                    FixHorizontalLayoutGroup(go).spacing = 3;
                }

                var adapter = new MechLabHardpointElementAdapter(element);
                {
                    var go = adapter.hardpointText.gameObject;
                    var cgo = new GameObject("TextContainer");
                    cgo.transform.parent = go.transform.parent;
                    go.transform.parent = cgo.transform;

                    var height = 20f;
                    var width = go.name.Contains("jump") ? 40f : 20f;
                    FixRect(go, height);
                    FixRect(cgo, height);

                    {
                        var component = go.GetComponent<LocalizableText>();
                        component.autoSizeTextContainer = true;
                        component.alignment = TextAlignmentOptions.MidlineRight;
                    }

                    {
                        var component = cgo.GetComponent<GridLayoutGroup>() ?? cgo.AddComponent<GridLayoutGroup>();
                        component.cellSize = new Vector2(width, height);
                        component.enabled = true;
                    }
                }

                {
                    var go = adapter.hardpointIcon.gameObject;
                    var cgo = new GameObject("SVGImageContainer");
                    cgo.transform.parent = go.transform.parent;
                    go.transform.parent = cgo.transform;

                    var size = 20f;
                    FixRect(go, size);
                    FixRect(cgo, size);
                    {
                        var component = cgo.GetComponent<GridLayoutGroup>() ?? cgo.AddComponent<GridLayoutGroup>();
                        component.cellSize = new Vector2(size, size);
                        component.enabled = true;
                    }
                }
            }

            static ContentSizeFitter FixContentSizeFitter(GameObject go)
            {
                var component = go.GetComponent<ContentSizeFitter>() ?? go.AddComponent<ContentSizeFitter>();
                component.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
                component.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
                component.enabled = true;
                return component;
            }

            static HorizontalLayoutGroup FixHorizontalLayoutGroup(GameObject go)
            {
                var component = go.GetComponent<HorizontalLayoutGroup>() ?? go.AddComponent<HorizontalLayoutGroup>();
                component.childControlWidth = true;
                component.childControlHeight = true;
                component.childForceExpandWidth = false;
                component.childForceExpandHeight = false;
                component.childAlignment = TextAnchor.MiddleLeft;
                component.spacing = 0;
                component.enabled = true;
                return component;
            }

            static RectTransform FixRect(GameObject go, float? size = null)
            {
                var rect = go.GetComponent<RectTransform>() ?? go.AddComponent<RectTransform>();
                rect.anchorMin = new Vector2(0, 1);
                rect.anchorMax = new Vector2(0, 1);
                rect.pivot = new Vector2(0, 1);
                if (size.HasValue)
                {
                    rect.offsetMin = new Vector2(0, -size.Value);
                    rect.offsetMax = new Vector2(size.Value, 0);
                }

                rect.localPosition = new Vector3(0, 0, 0);
                return rect;
            }

            foreach (var hardpoint in hardpoints)
            {
                FixElementLayout(hardpoint);
            }
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
    }
}
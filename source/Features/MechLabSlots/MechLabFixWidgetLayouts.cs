using BattleTech.UI;
using BattleTech.UI.TMProWrapper;
using FluffyUnderware.DevTools.Extensions;
using SVGImporter;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MechEngineer.Features.MechLabSlots
{
    internal class MechLabFixWidgetLayouts
    {
        internal static void FixMechLabLayouts(MechLabPanel panel)
        {
            FixMechLabMechInfoWidgetLayout(panel);
            FixMechLabLocationWidgetLayouts(panel);
        }

        static void FixMechLabMechInfoWidgetLayout(MechLabPanel panel)
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

        static void FixMechLabLocationWidgetLayouts(MechLabPanel mechLabPanel)
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
                    go.GetComponent<VerticalLayoutGroup>().spacing = 56;
                }

                foreach (Transform widget in container)
                {
                    if (widget.GetComponent<MechLabLocationWidget>() == null)
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

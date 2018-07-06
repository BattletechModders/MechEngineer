using System;
using System.Collections.Generic;
using System.Linq;
using BattleTech;
using BattleTech.Save.SaveGameStructure;
using BattleTech.UI;
using Harmony;
using HBS.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace MechEngineer
{
    [HarmonyPatch(typeof(MechLabPanel), "LoadMech")]
    public static class MechLabPanelLoadMechPatch
    {
        public static void Postfix(MechLabPanel __instance)
        {
            try
            {
                //GUIModUtils.LogHierarchy(__instance.transform);

                var Representation = __instance.transform.GetChild("Representation");
                var OBJ_mech = Representation.GetChild("OBJ_mech");

                var Centerline = OBJ_mech.GetChild("Centerline");
                {
                    var layout_details = Centerline.GetChild("layout_details");
                    if (layout_details == null)
                    {
                        layout_details = Representation.GetChild("layout_details");
                    }

                    if (layout_details != null)
                    {
                        var OBJ_value = Representation.GetChild("OBJ_value");
                        var LeftArmWidget = OBJ_mech.GetChild("LeftArm").GetChild("uixPrfPanl_ML_location-Widget-MANAGED");

                        var v = new Vector3[4];
                        LeftArmWidget.Rect().GetWorldCorners(v);
                        var armtop = v[1].y;
                        var armleft = v[2].x;
                        var armright = v[3].x;
                        var armcenter_x = armleft + (armleft - armright) / 2;

                        layout_details.SetParent(Representation, true);
                        layout_details.Rect().pivot = new Vector2(1.0f, 1.0f);
                        layout_details.position = new Vector3(
                            armcenter_x,
                            armtop + layout_details.Rect().sizeDelta.y + 10,
                            OBJ_value.position.z
                        );
                    }
                }

                const float space = 20;

                {
                    var headWidget = Centerline.GetChild("uixPrfPanl_ML_location-Widget-MANAGED");
                    var centerTorsoWidget = Centerline.GetChild("uixPrfPanl_ML_location-Widget-MANAGED", 1);

                    centerTorsoWidget.SetTop(headWidget.Bottom() - space);
                }

                {
                    var RightTorsoLeg = OBJ_mech.GetChild("RightTorsoLeg");
                    var RightTorsoWidget = RightTorsoLeg.GetChild("uixPrfPanl_ML_location-Widget-MANAGED");
                    var RightLegWidget = RightTorsoLeg.GetChild("uixPrfPanl_ML_location-Widget-MANAGED", 1);

                    RightLegWidget.SetTop(RightTorsoWidget.Bottom() - space);
                }

                {
                    var LeftTorsoLeg = OBJ_mech.GetChild("LeftTorsoLeg");
                    var LeftTorsoWidget = LeftTorsoLeg.GetChild("uixPrfPanl_ML_location-Widget-MANAGED");
                    var LeftLegWidget = LeftTorsoLeg.GetChild("uixPrfPanl_ML_location-Widget-MANAGED", 1);

                    LeftLegWidget.SetTop(LeftTorsoWidget.Bottom() - space);
                }
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
        }
    }

    [HarmonyPatch(typeof(MechLabLocationWidget), "SetData")]
    public static class MechLabLocationWidgetSetDataPatch
    {
        private const int SlotHeight = 32;

        public static void Postfix(MechLabLocationWidget __instance, int ___maxSlots, LocationLoadoutDef ___loadout)
        {
            try
            {
                // we can't reduce to zero
                if (___maxSlots < 1)
                {
                    return;
                }

                var widgetLayout = new WidgetLayout(__instance, ___loadout.Location);
                if (widgetLayout.layout_slots == null)
                {
                    return;
                }

                ModifySlotCount(widgetLayout, ___maxSlots);
                AddFillersToSlots(widgetLayout);
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
        }

        private static void ModifySlotCount(WidgetLayout layout, int maxSlots)
        {
            var slots = layout.slots;
            var changedSlotCount = maxSlots - slots.Count;

            if (changedSlotCount == 0)
            {
                return;
            }

            var templateSlot = slots[0];

            // add missing
            int index = slots[0].GetSiblingIndex();
            for (var i = slots.Count; i < maxSlots; i++)
            {
                var newSlot = UnityEngine.Object.Instantiate(templateSlot, layout.layout_slots);
                newSlot.localPosition = new Vector3(0, -(1 + i * SlotHeight), 0);
                newSlot.SetSiblingIndex(index + i);
                newSlot.name = "slot (" + i + ")";
                slots.Add(newSlot);
            }

            // remove abundant
            while (slots.Count > maxSlots)
            {
                var slot = slots.Last();
                slots.RemoveAt(slots.Count - 1);
                UnityEngine.Object.Destroy(slot.gameObject);
            }

            var changedHeight = changedSlotCount * SlotHeight;

            layout.widget.transform.AdjustHeight(changedHeight);
            layout.layout_slots.AdjustHeight(changedHeight);
        }

        public static Dictionary<ChassisLocations, List<Image>> FillerImageCache = new Dictionary<ChassisLocations, List<Image>>();

        private static void AddFillersToSlots(WidgetLayout layout)
        {
            var images = new List<Image>();

            foreach (var slot in layout.slots)
            {
                Image image;

                var go = slot.gameObject.FindFirstChildNamed("Filler");
                if (go == null)
                {
                    go = new GameObject("Filler");

                    var rect = go.AddComponent<RectTransform>();
                    rect.pivot = new Vector2(0.5f, 0.5f);
                    rect.anchorMin = new Vector2(0, 0);
                    rect.anchorMax = new Vector2(1, 1);
                    rect.anchoredPosition = Vector2.zero;
                    rect.sizeDelta = new Vector2(-6, -6);
                    go.AddComponent<CanvasRenderer>();

                    image = go.AddComponent<Image>();

                    rect.SetParent(slot, false);
                }
                else
                {
                    image = go.GetComponent<Image>();
                }

                go.SetActive(false);
                image.color = Color.red;
                images.Add(image);
            }

            FillerImageCache[layout.location] = images;
        }

        private class WidgetLayout
        {
            internal WidgetLayout(MechLabLocationWidget widget, ChassisLocations location)
            {
                this.location = location;
                this.widget = widget;
                layout_slots = widget.transform.GetChild("layout_slots");
                if (layout_slots == null)
                {
                    return;
                }
                slots = layout_slots.GetChildren()
                    .Where(x => x.name.StartsWith("slot"))
                    .OrderByDescending(x => x.localPosition.y)
                    .ToList();
            }

            internal ChassisLocations location { get; }
            internal MechLabLocationWidget widget { get; }
            internal Transform layout_slots { get; }
            internal List<Transform> slots { get; }
        }
    }

    public static class Utils
    {
        public static void AdjustHeight(this Transform transform, int changedHeight)
        {
            var rect = transform.GetComponent<RectTransform>();
            if (rect == null)
            {
                return;
            }

            var vector = rect.sizeDelta;
            vector.y += changedHeight;
            rect.sizeDelta = vector;
        }

        public static RectTransform Rect(this Transform transform)
        {
            return transform.GetComponent<RectTransform>();
        }

        public static float Top(this Transform transform)
        {
            return transform.localPosition.y;
        }

        public static void SetTop(this Transform transform, float y)
        {
            var position = transform.localPosition;
            position.y = y;
            transform.localPosition = position;
        }

        public static float Bottom(this Transform transform)
        {
            var rect = transform.GetComponent<RectTransform>();
            return transform.localPosition.y - rect.sizeDelta.y;
        }

        public static void SetBottom(this Transform transform, float y)
        {
            var rect = transform.GetComponent<RectTransform>();
            var position = transform.localPosition;
            position.y = y + rect.sizeDelta.y;
            transform.localPosition = position;
        }

        public static float Right(this Transform transform)
        {
            var rect = transform.GetComponent<RectTransform>();
            return transform.localPosition.x + rect.sizeDelta.x;
        }

        public static void SetRight(this Transform transform, float x)
        {
            var rect = transform.GetComponent<RectTransform>();
            var position = transform.localPosition;
            position.x = x - rect.sizeDelta.x;
            transform.localPosition = position;
        }

        public static IEnumerable<Transform> GetChildren(this Transform @this)
        {
            foreach (Transform current in @this)
            {
                yield return current;
            }
        }

        public static Transform GetChild(this Transform @this, string name, int index = 0)
        {
            return @this.GetChildren().Where(x => x.name == name).Skip(index).FirstOrDefault();
        }

        public static void LogTransform(Transform transform)
        {
            Control.mod.Logger.LogDebug("");
            Control.mod.Logger.LogDebug("name=" + transform.name);
            Control.mod.Logger.LogDebug("parent=" + transform.parent);
            Control.mod.Logger.LogDebug("position=" + transform.position);
            Control.mod.Logger.LogDebug("localPosition=" + transform.localPosition);
            var rect = transform.GetComponent<RectTransform>();
            Control.mod.Logger.LogDebug("rect.anchoredPosition=" + rect.anchoredPosition);
            //Control.mod.Logger.LogDebug("rect.anchorMax=" + rect.anchorMax);
            //Control.mod.Logger.LogDebug("rect.anchorMin=" + rect.anchorMin);
            //Control.mod.Logger.LogDebug("rect.offsetMax=" + rect.offsetMax);
            //Control.mod.Logger.LogDebug("rect.offsetMin=" + rect.offsetMin);
            Control.mod.Logger.LogDebug("rect.pivot=" + rect.pivot);
            Control.mod.Logger.LogDebug("rect.rect=" + rect.rect);
        }

        public static void LogHierarchy(Transform transform, int level = 0)
        {
            var text = "";
            for (var i = 0; i < level; i++)
            {
                text += "  ";
            }

            var rectText = "";
            {
                var rect = transform.GetComponent<RectTransform>();
                if (rect != null)
                {
                    rectText = "rect=" + rect.rect + " ancho=" + rect.anchoredPosition;
                }
            }

            Control.mod.Logger.LogDebug(text + transform.gameObject.name + " world=" + transform.position + " local=" + transform.localPosition + " " + rectText);
            level++;
            foreach (Transform current in transform)
            {
                LogHierarchy(current, level);
            }
        }
    }
}
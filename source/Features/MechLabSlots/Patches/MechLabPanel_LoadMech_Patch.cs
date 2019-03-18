using System;
using BattleTech.UI;
using Harmony;
using UnityEngine;
using UnityEngine.UI;

namespace MechEngineer
{
    [HarmonyPatch(typeof(MechLabPanel), "LoadMech")]
    public static class MechLabPanel_LoadMech_Patch
    {
        public static void Postfix(MechLabPanel __instance)
        {
            try
            {
                var mechLabPanel = __instance;
                var Representation = mechLabPanel.transform.GetChild("Representation");
                var OBJ_mech = Representation.GetChild("OBJ_mech");

                void ProcessChild(GameObject go)
                {
                    go.EnableLayout();
                    var component = go.GetComponent<ContentSizeFitter>() ?? go.AddComponent<ContentSizeFitter>();
                    component.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
                    component.enabled = true;
                }
                ProcessChild(OBJ_mech.GetChild("RightArm").gameObject);
                ProcessChild(OBJ_mech.GetChild("RightTorsoLeg").gameObject);
                ProcessChild(OBJ_mech.GetChild("Centerline").gameObject);
                ProcessChild(OBJ_mech.GetChild("LeftTorsoLeg").gameObject);
                ProcessChild(OBJ_mech.GetChild("LeftArm").gameObject);

                OBJ_mech.gameObject.EnableLayout();
                {
                    var go = OBJ_mech.gameObject;
                    var component = go.GetComponent<ContentSizeFitter>() ?? go.AddComponent<ContentSizeFitter>();
                    component.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
                    component.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
                    component.enabled = true;
                }

                var mechRectTransform = OBJ_mech.GetComponent<RectTransform>();
                // Unity (?) does not handle layout propagation properly, so we need to force several layout passes here
                // also allows us to calculate stuff for auto zoom without waiting for regular layout passes
                LayoutRebuilder.ForceRebuildLayoutImmediate(mechRectTransform);
                LayoutRebuilder.ForceRebuildLayoutImmediate(mechRectTransform);
                LayoutRebuilder.ForceRebuildLayoutImmediate(mechRectTransform);

                mechRectTransform.anchorMin = new Vector2(mechRectTransform.anchorMin.x, 1);
                mechRectTransform.anchorMax = new Vector2(mechRectTransform.anchorMin.x, 1);
                mechRectTransform.pivot = new Vector2(mechRectTransform.pivot.x, 1);
                
                var OBJ_actions = Representation.GetChild("OBJ_actions");
                mechRectTransform.position = new Vector3(OBJ_mech.position.x, OBJ_actions.position.y, OBJ_mech.position.z);

                {
                    var OBJ_cancelconfirm = Representation.GetChild("OBJ_cancelconfirm");
                    var confirmRectTransform = OBJ_cancelconfirm.GetComponent<RectTransform>();

                    var mechSize = mechRectTransform.sizeDelta.y;
                    var targetSize = mechRectTransform.localPosition.y
                                     - 40 // repair button height
                                     - confirmRectTransform.localPosition.y + confirmRectTransform.sizeDelta.y; // save button bottom

                    var scale = Mathf.Min(1, targetSize / mechSize);
                    mechRectTransform.localScale = new Vector3(scale, scale, 1);
                
                    Control.mod.Logger.LogDebug($"AutoZoom scale={scale} mechSize={mechSize} targetSize={targetSize}");
                }
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }

            try
            {
                var mechLabPanel = __instance;
                //GUILogUtils.LogHierarchy(mechLabPanel.transform);

                //mechLabPanel.transform.localScale = new Vector3(0.5f, 0.5f);
                var Representation = mechLabPanel.transform.GetChild("Representation");
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
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
        }
    }
}
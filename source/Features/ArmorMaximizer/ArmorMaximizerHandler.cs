using BattleTech;
using BattleTech.UI;
using UnityEngine;
using MechEngineer.Features.OverrideTonnage;
using System;

namespace MechEngineer.Features.ArmorMaximizer;

public static class ArmorMaximizerHandler
{
    public static void OnMaxArmor(MechLabPanel __instance, MechLabMechInfoWidget ___mechInfoWidget, MechLabItemSlotElement ___dragItem)
    {
        var settings = ArmorMaximizerFeature.Shared.Settings;
        //            var logger = HBS.Logging.Logger.GetLogger("Sysinfo");
        //Store the Mods.Settings.Unchanged value.
        bool originalSetting = settings.HeadPointsUnChanged;
        var hk = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        //If Shift is held while click the button it will flip the setting for HeadPointsUnchanged
        // TODO fix this, should never change settings!
        if (hk)
        {
            settings.HeadPointsUnChanged = !settings.HeadPointsUnChanged;
        }
        //Sets all the variables for the mech at the time the button is clicked
        ArmorState state = new(__instance.activeMechDef);
        if (!__instance.Initialized)
        {
            return;
        }
        if (___dragItem != null)
        {
            return;
        }
        if (__instance.headWidget.IsDestroyed || __instance.centerTorsoWidget.IsDestroyed || __instance.leftTorsoWidget.IsDestroyed || __instance.rightTorsoWidget.IsDestroyed || __instance.leftArmWidget.IsDestroyed || __instance.rightArmWidget.IsDestroyed || __instance.leftLegWidget.IsDestroyed || __instance.rightLegWidget.IsDestroyed)
        {
            return;
        }
        //Will only maximize if enough free tonnage + assigned armor is available.
        if (state.CanMaxArmor)
        {
            float availableArmor = state.AvailableArmorPoints;
            float h_MaxAP = state.H_MaxAP;
            float ct_MaxAP = state.CT_MaxAP;
            float lt_MaxAP = state.LT_MaxAP;
            float rt_MaxAP = state.RT_MaxAP;
            float la_MaxAP = state.LA_MaxAP;
            float ra_MaxAP = state.RA_MaxAP;
            float ll_MaxAP = state.LL_MaxAP;
            float rl_MaxAP = state.RL_MaxAP;
            float h_AssignedAP = state.H_AssignedAP;
            float ct_AssignedAP = state.CT_AssignedAP;
            float lt_AssignedAP = state.LT_AssignedAP;
            float rt_AssignedAP = state.RT_AssignedAP;
            float la_AssignedAP = state.LA_AssignedAP;
            float ra_AssignedAP = state.RA_AssignedAP;
            float ll_AssignedAP = state.LL_AssignedAP;
            float rl_AssignedAP = state.RL_AssignedAP;
            float assignedPoints = h_AssignedAP + ct_AssignedAP + lt_AssignedAP + rt_AssignedAP + la_AssignedAP + ra_AssignedAP + ll_AssignedAP + rl_AssignedAP;
            float remainingPoints = availableArmor - assignedPoints;
            //If there are leftover points due to rounding down, distribute them.
            if (assignedPoints < availableArmor)
            {
                bool pass = false;
                float maxTorso = 4;
                if (settings.HeadPointsUnChanged)
                {
                    maxTorso = 3;
                }
                float torsoPoints = 0;
                float extremityPoints = 0;

                bool h_Full = false;
                bool ct_Full = false;
                bool lt_Full = false;
                bool rt_Full = false;
                bool la_Full = false;
                bool ra_Full = false;
                bool ll_Full = false;
                bool rl_Full = false;

                while (remainingPoints > 0)
                {
                    if (!h_Full)
                    {
                        if (!settings.HeadPointsUnChanged)
                        {
                            if (h_AssignedAP < h_MaxAP)
                            {
                                if (remainingPoints <= 0) break;
                                h_AssignedAP++;
                                remainingPoints--;
                            }
                            else
                            {
                                h_Full = true;
                                torsoPoints++;
                            }
                        }
                        else
                        {
                            if (torsoPoints == maxTorso && extremityPoints == 4)
                            {
                                if (h_AssignedAP < h_MaxAP)
                                {
                                    if (remainingPoints <= 0) break;
                                    h_AssignedAP++;
                                    remainingPoints--;
                                }
                                else
                                {
                                    h_Full = true;
                                    torsoPoints++;
                                }
                            }
                        }
                    }
                    if (!ct_Full)
                    {
                        if (ct_AssignedAP < ct_MaxAP)
                        {
                            if (remainingPoints <= 0) break;
                            ct_AssignedAP++;
                            remainingPoints--;
                        }
                        else
                        {
                            ct_Full = true;
                            torsoPoints++;
                        }
                    }
                    if (!lt_Full)
                    {
                        if (lt_AssignedAP < lt_MaxAP)
                        {
                            if (remainingPoints <= 0) break;
                            lt_AssignedAP++;
                            remainingPoints--;
                        }
                        else
                        {
                            lt_Full = true;
                            torsoPoints++;
                        }
                    }
                    if (!rt_Full)
                    {
                        if (rt_AssignedAP < rt_MaxAP)
                        {
                            if (remainingPoints <= 0) break;
                            rt_AssignedAP++;
                            remainingPoints--;
                        }
                        else
                        {
                            rt_Full = true;
                            torsoPoints++;
                        }
                    }
                    if (pass)
                    {
                        if (!la_Full)
                        {
                            if (la_AssignedAP < la_MaxAP)
                            {
                                if (remainingPoints <= 0) break;
                                la_AssignedAP++;
                                remainingPoints--;
                            }
                            else
                            {
                                la_Full = true;
                                extremityPoints++;
                            }
                        }
                        if (!ra_Full)
                        {
                            if (ra_AssignedAP < ra_MaxAP)
                            {
                                if (remainingPoints <= 0) break;
                                ra_AssignedAP++;
                                remainingPoints--;
                            }
                            else
                            {
                                ra_Full = true;
                                extremityPoints++;
                            }
                        }
                        if (!ll_Full)
                        {
                            if (ll_AssignedAP < ll_MaxAP)
                            {
                                if (remainingPoints <= 0) break;
                                ll_AssignedAP++;
                                remainingPoints--;
                            }
                            else
                            {
                                ll_Full = true;
                                extremityPoints++;
                            }
                        }
                        if (!rl_Full)
                        {
                            if (rl_AssignedAP < rl_MaxAP)
                            {
                                if (remainingPoints <= 0) break;
                                rl_AssignedAP++;
                                remainingPoints--;
                            }
                            else
                            {
                                rl_Full = true;
                                extremityPoints++;
                            }
                        }
                    }
                    pass = !pass;
                }
            }
            //Set the armor points variables for the front and rear torso positions based on the settings in the json.
            float ct_Front = Mathf.Ceil(ct_AssignedAP * settings.CenterTorsoRatio);
            if (ArmorUtils.IsDivisible(ct_AssignedAP, 5.0f)) ct_Front = PrecisionUtils.RoundUp(ct_Front, 5);
            float ct_Rear = ct_AssignedAP - ct_Front;
            float lt_Front = Mathf.Ceil(lt_AssignedAP * settings.LeftTorsoRatio);
            if (ArmorUtils.IsDivisible(lt_AssignedAP, 5.0f)) lt_Front = PrecisionUtils.RoundUp(lt_Front, 5);
            float lt_Rear = lt_AssignedAP - lt_Front;
            float rt_Front = Mathf.Ceil(rt_AssignedAP * settings.RightTorsoRatio);
            if (ArmorUtils.IsDivisible(rt_AssignedAP, 5.0f)) rt_Front = PrecisionUtils.RoundUp(rt_Front, 5);
            float rt_Rear = rt_AssignedAP - rt_Front;
            //If rear armor is 0 add one point to it, but only if the front has 2 points or more available.
            if (ct_Rear == 0)
            {
                if (ct_Front > 1)
                {
                    ct_Front--;
                    ct_Rear++;
                }
            }
            if (lt_Rear == 0)
            {
                if (lt_Front > 1)
                {
                    lt_Front--;
                    lt_Rear++;
                }
            }
            if (rt_Rear == 0)
            {
                if (rt_Front > 1)
                {
                    rt_Front--;
                    rt_Rear++;
                }
            }

            __instance.headWidget.SetArmor(false, h_AssignedAP, true);
            __instance.centerTorsoWidget.SetArmor(false, ct_Front, true);
            __instance.centerTorsoWidget.SetArmor(true, ct_Rear, true);
            __instance.leftTorsoWidget.SetArmor(false, lt_Front, true);
            __instance.leftTorsoWidget.SetArmor(true, lt_Rear, true);
            __instance.rightTorsoWidget.SetArmor(false, rt_Front, true);
            __instance.rightTorsoWidget.SetArmor(true, rt_Rear, true);
            __instance.leftArmWidget.SetArmor(false, la_AssignedAP, true);
            __instance.rightArmWidget.SetArmor(false, ra_AssignedAP, true);
            __instance.leftLegWidget.SetArmor(false, ll_AssignedAP, true);
            __instance.rightLegWidget.SetArmor(false, rl_AssignedAP, true);
            ___mechInfoWidget.RefreshInfo(false);
            __instance.FlagAsModified();
            __instance.ValidateLoadout(false);
        }
        //Change the HeadPointsUnChanged variable back to what it was before clicking.
        if (originalSetting != settings.HeadPointsUnChanged)
        {
            settings.HeadPointsUnChanged = !settings.HeadPointsUnChanged;
        }
    }
    public static bool handleArmorUpdate(MechLabLocationWidget widget, bool isRearArmor, float amount)
    {
        var mechDef = widget.mechLab.activeMechDef;
        float originalAmount = amount;
        float tonsPerPoint = ArmorUtils.TonPerPoint(mechDef);
        float freeTonnage = WeightsUtils.CalculateFreeTonnage(mechDef);
        freeTonnage = ArmorUtils.RoundUp(freeTonnage, 0.0005f);
        float armorWeight = amount * tonsPerPoint;
        var ratio = widget.loadout.Location == ChassisLocations.Head ? 3 : 2;
        float enforcedArmor = widget.chassisLocationDef.InternalStructure;
        enforcedArmor = ArmorUtils.RoundDown(enforcedArmor, 5);
        enforcedArmor *= ratio;
        float currentArmor = widget.currentArmor;
        float currentRearArmor = widget.currentRearArmor;
        float maxArmor = enforcedArmor - currentArmor - currentRearArmor;
        var hk = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        if (isRearArmor)
        {
            currentArmor = widget.currentRearArmor;
        }
        if (hk)
        {
           amount *= 5f;
            bool divisible = ArmorUtils.IsDivisible(currentArmor, amount);
            if (!divisible)
            {
                if (amount > 0)
                {
                    if (freeTonnage < tonsPerPoint) return false;
                    float multWeight = armorWeight * amount;
                    if(multWeight > freeTonnage)
                    {
                        float freePoints = freeTonnage / tonsPerPoint;
                        freePoints = ArmorUtils.RoundDown(freePoints,1);
                        if (freePoints > 0)
                        {
                            currentArmor += freePoints;
                            if(freePoints > maxArmor)
                            {
                                currentArmor += maxArmor;
                            }
                            widget.SetArmor(isRearArmor, currentArmor, true);
                            return false;
                        }
                        return false;
                    }
                    currentArmor = ArmorUtils.RoundUp(currentArmor, amount);
                    widget.SetArmor(isRearArmor, currentArmor, true);
                    return false;
                }
                if (amount < 0)
                {
                    if (currentArmor <= 0) return false;
                    currentArmor = ArmorUtils.RoundDown(currentArmor, amount);
                    if(currentArmor < 0)
                    {
                        currentArmor = 0;
                    }
                    widget.SetArmor(isRearArmor, currentArmor, true);
                    return false;
                }
            }
        }
        if (amount > 0)
        {
            if (freeTonnage < tonsPerPoint) return false;
            if (maxArmor <= 0) return false;
            float multWeight = armorWeight * amount;
            if (multWeight > freeTonnage)
            {
                float freePoints = freeTonnage / tonsPerPoint;
                freePoints = ArmorUtils.RoundDown(freePoints, 1);
                if (freePoints > 0)
                {
                    currentArmor += freePoints;
                    if (freePoints > maxArmor)
                    {
                        currentArmor += maxArmor;
                    }
                    widget.SetArmor(isRearArmor, currentArmor, true);
                    return false;
                }
                return false;
            }
        }
        if (amount < 0)
        {
            if (currentArmor <= 0) return false;
        }
        widget.ModifyArmor(isRearArmor, amount, true);
        return false;
    }
}
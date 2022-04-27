using BattleTech;
using BattleTech.UI;
using UnityEngine;
using MechEngineer.Features.OverrideTonnage;

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
                float h_points = h_MaxAP - h_MaxAP;
                float ct_points = ct_MaxAP - ct_AssignedAP;
                float lt_points = lt_MaxAP - lt_AssignedAP;
                float rt_points = rt_MaxAP - rt_AssignedAP;
                float la_points = la_MaxAP - la_AssignedAP;
                float ra_points = ra_MaxAP - ra_AssignedAP;
                float ll_points = ll_MaxAP - ll_AssignedAP;
                float rl_points = rl_MaxAP - rl_AssignedAP;
                float torsoPoints = h_points + ct_points + lt_points + rt_points;
                float extremityPoints = la_points + ra_points + ll_points + rl_points;
                while (remainingPoints > 0)
                {
                    if (torsoPoints > 0)
                    {
                        if (!settings.HeadPointsUnChanged)
                        {
                            if (h_points > 0)
                            {
//                                    logger.Log("Added Head");
                                h_AssignedAP++;
                                torsoPoints--;
                                remainingPoints--;
                                if (remainingPoints <= 0)
                                {
                                    break;
                                }
                            }
                        }
                        if (ct_points > 0)
                        {
//                                logger.Log("Added CenterTorso");
                            ct_AssignedAP++;
                            torsoPoints--;
                            remainingPoints--;
                            if (remainingPoints <= 0)
                            {
                                break;
                            }
                        }
                        if (lt_points > 0)
                        {
//                                logger.Log("Added LeftTorso");
                            lt_AssignedAP++;
                            torsoPoints--;
                            remainingPoints--;
                            if (remainingPoints <= 0)
                            {
                                break;
                            }
                        }
                        if (rt_points > 0)
                        {
//                                logger.Log("Added RightTorso");
                            rt_AssignedAP++;
                            torsoPoints--;
                            remainingPoints--;
                            if (remainingPoints <= 0)
                            {
                                break;
                            }
                        }
                    }
                    if (torsoPoints <= 0 && extremityPoints <= 0 && settings.HeadPointsUnChanged)
                    {
//                            logger.Log("Added Head");
                        h_AssignedAP++;
                        remainingPoints--;
                        if (remainingPoints <= 0)
                        {
                            break;
                        }
                    }
                    if (pass)
                    {
                        if (extremityPoints > 0)
                        {
                            if (la_points > 0)
                            {
//                                    logger.Log("Added LeftArm");
                                ll_AssignedAP++;
                                extremityPoints--;
                                remainingPoints--;
                                if (remainingPoints <= 0)
                                {
                                    break;
                                }
                            }

                        }
                        if (extremityPoints > 0)
                        {
                            if (ra_points > 0)
                            {
//                                    logger.Log("Added RightArm");
                                ra_AssignedAP++;
                                extremityPoints--;
                                remainingPoints--;
                                if (remainingPoints <= 0)
                                {
                                    break;
                                }
                            }

                        }
                        if (extremityPoints > 0)
                        {
                            if (ll_points > 0)
                            {
//                                    logger.Log("Added LeftLeg");
                                ll_AssignedAP++;
                                extremityPoints--;
                                remainingPoints--;
                                if (remainingPoints <= 0)
                                {
                                    break;
                                }
                            }

                        }
                        if (extremityPoints > 0)
                        {
                            if (rl_points > 0)
                            {
//                                    logger.Log("Added RightLeg");
                                rl_AssignedAP++;
                                extremityPoints--;
                                remainingPoints--;
                                if (remainingPoints <= 0)
                                {
                                    break;
                                }
                            }

                        }
                    }
                    pass = !pass;
                }
            }
            //Set the armor points variables for the front and rear torso positions based on the settings in the json.
            float ct_Front = Mathf.Ceil(ct_AssignedAP * settings.CenterTorsoRatio);
            float ct_Rear = ct_AssignedAP - ct_Front;
            float lt_Front = Mathf.Ceil(lt_AssignedAP * settings.LeftTorsoRatio);
            float lt_Rear = lt_AssignedAP - lt_Front;
            float rt_Front = Mathf.Ceil(rt_AssignedAP * settings.RightTorsoRatio);
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
            //Set the armor points based on the defined variables.
//                assignedPoints = h_AssignedAP + ct_AssignedAP + lt_AssignedAP + rt_AssignedAP +la_AssignedAP + ra_AssignedAP + ll_AssignedAP + rl_AssignedAP;
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
/*                logger.Log("availableAP: " + availableArmor);
                logger.Log("assignedAP: " + assignedPoints);
                logger.Log("h_assignedAP: " + h_AssignedAP);
                logger.Log("ct_Front: " + ct_Front);
                logger.Log("ct_Rear: " + ct_Rear);
                logger.Log("lt_Front: " + lt_Front);
                logger.Log("lt_Rear: " + lt_Rear);
                logger.Log("rt_Front: " + rt_Front);
                logger.Log("rt_Rear: " + rt_Rear);
                logger.Log("la_assignedAP: " + la_AssignedAP);
                logger.Log("ra_assignedAP: " + ra_AssignedAP);
                logger.Log("ll_assignedAP: " + ll_AssignedAP);
                logger.Log("rl_assignedAP: " + rl_AssignedAP);
                logger.Log("");
*/
            ___mechInfoWidget.RefreshInfo(false);
            //Flag as modified in the GUI
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
        var location = widget.loadout.Location;
        var ratio = location == ChassisLocations.Head ? 3 : 2;
        var mechDef = Globals.Global.ActiveMechDef;
        var enforcedArmor = mechDef.Chassis.GetLocationDef(location).InternalStructure;
        enforcedArmor = PrecisionUtils.RoundDown(enforcedArmor, 5);
        enforcedArmor *= ratio;
        float currentArmor = widget.currentArmor;
        float currentRearArmor = widget.currentRearArmor;
        float maxArmor = enforcedArmor - currentArmor - currentRearArmor;
        float originalAmount = amount;
        var hk = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        if (isRearArmor)
        {
            currentArmor = widget.currentRearArmor;
        }
        if (hk)
        {
            bool isDivisible = ArmorUtils.IsDivisible(currentArmor, 5f);
            amount *= 5f;
            if (!isDivisible)
            {
                if (amount > 0)
                {
                    if (amount > maxArmor)
                    {
                        if (originalAmount <= maxArmor)
                        {
                            widget.ModifyArmor(isRearArmor, originalAmount, true);
                            return false;
                        }
                    }
                    currentArmor = PrecisionUtils.RoundUp(currentArmor, amount);
                    widget.SetArmor(isRearArmor, currentArmor, true);
                    return false;
                }
                if(amount < 0)
                {
                    if(amount < 0)
                    {
                        if (originalAmount > 0)
                        {
                            widget.ModifyArmor(isRearArmor, originalAmount, true);
                            return false;
                        }
                    }
                    float flippedAmount = amount * -1;
                    currentArmor = OverrideTonnage.PrecisionUtils.RoundDown(currentArmor, flippedAmount);
                    widget.SetArmor(isRearArmor, currentArmor, true);
                    return false;
                }
            }
            if(amount > 0)
            {
                if (amount > maxArmor) return false;
            }
            widget.ModifyArmor(isRearArmor, amount, true);
            return false;
        }
        if (amount < 0)
        {
            if(currentArmor - amount < 0) return false;
        }
        if (amount > 0)
        {
            if(amount > maxArmor) return false;
        }
        widget.ModifyArmor(isRearArmor, amount, true);
        return false;
    }
}
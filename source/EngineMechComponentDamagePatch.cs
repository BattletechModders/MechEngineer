using System;
using BattleTech;
using Harmony;

namespace MechEngineMod
{
    [HarmonyPatch(typeof(MechComponent), "DamageComponent")]
    public static class EngineMechComponentDamagePatch
    {
        // crit engine reduces speed
        // destroyed engine destroys CT
        public static void Postfix(MechComponent __instance, WeaponHitInfo hitInfo, ComponentDamageLevel damageLevel, bool applyEffects)
        {
            try
            {
                if (__instance.componentDef == null || !(__instance.parent is Mech))
                {
                    return;
                }

                if (!Control.IsEnginePart(__instance.componentDef))
                {
                    return;
                }

                var mech = (Mech)__instance.parent;
                if (damageLevel == ComponentDamageLevel.Penalized)
                {
                    if (!mech.IsLocationDestroyed(ChassisLocations.CenterTorso))
                    {
                        Control.mod.Logger.LogDebug("Penalized=" + __instance.Location);

                        var walkSpeed = mech.StatCollection.GetStatistic("WalkSpeed");
                        var runSpeed = mech.StatCollection.GetStatistic("RunSpeed");
                        mech.StatCollection.Float_Multiply(walkSpeed, Control.settings.SpeedMultiplierPerDamagedEnginePart);
                        mech.StatCollection.Float_Multiply(runSpeed, Control.settings.SpeedMultiplierPerDamagedEnginePart);
                    }
                }
                else if (damageLevel == ComponentDamageLevel.Destroyed)
                {
                    Control.mod.Logger.LogDebug("Destroyed=" + __instance.Location);

                    if (!mech.IsLocationDestroyed(ChassisLocations.CenterTorso))
                    {
                        var onUnitSphere = UnityEngine.Random.onUnitSphere;
                        mech.NukeStructureLocation(hitInfo, __instance.Location, ChassisLocations.CenterTorso, onUnitSphere, false);
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
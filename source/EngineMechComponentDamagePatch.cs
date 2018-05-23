using System;
using System.Linq;
using BattleTech;
using Harmony;

namespace MechEngineMod
{
    [HarmonyPatch(typeof(MechComponent), "DamageComponent")]
    public static class EngineMechComponentDamagePatch
    {
        // crit engine reduces speed
        // destroyed engine destroys CT
        public static bool Prefix(MechComponent __instance, WeaponHitInfo hitInfo, ComponentDamageLevel damageLevel, bool applyEffects)
        {
            try
            {
                if (!applyEffects) // fake test through AI calculations or went through crit calculations already
                {
                    return true;
                }

                if (damageLevel != ComponentDamageLevel.Destroyed) //we only care about destructions
                {
                    return true;
                }

                if (__instance.componentDef == null || !(__instance.parent is Mech))
                {
                    return true;
                }

                if (!__instance.componentDef.IsEnginePart())
                {
                    return true;
                }

                var mech = (Mech)__instance.parent;
                if (mech.IsLocationDestroyed(ChassisLocations.CenterTorso)) // implies destroyed LT, CT, RT
                {
                    return true;
                }

                switch (__instance.StatCollection.GetStatistic("DamageLevel").Value<ComponentDamageLevel>())
                {
                    case ComponentDamageLevel.Functional: // 1. CRIT
                        damageLevel = ComponentDamageLevel.Misaligned;
                        break;
                    case ComponentDamageLevel.Misaligned: // 2. CRIT
                        damageLevel = ComponentDamageLevel.Penalized;
                        break;
                    case ComponentDamageLevel.Penalized: // 3. CRIT
                        damageLevel = ComponentDamageLevel.Destroyed;
                        break;
                }

                if (mech.IsLocationDestroyed((ChassisLocations) __instance.Location))
                {
                    damageLevel = ComponentDamageLevel.Destroyed;
                }

                // do on CRIT
                if (damageLevel < ComponentDamageLevel.NonFunctional)
                {
                    Control.mod.Logger.LogDebug("CRIT on " + __instance.Name);

                    var walkSpeed = mech.StatCollection.GetStatistic("WalkSpeed");
                    var runSpeed = mech.StatCollection.GetStatistic("RunSpeed");
                    mech.StatCollection.Float_Multiply(walkSpeed, Control.settings.SpeedMultiplierPerDamagedEnginePart);
                    mech.StatCollection.Float_Multiply(runSpeed, Control.settings.SpeedMultiplierPerDamagedEnginePart);

                    var heatSink = mech.StatCollection.GetStatistic("HeatSinkCapacity");
                    mech.StatCollection.Int_Add(heatSink, Control.settings.HeatSinkCapacityPerDamagedEnginePart);

                    __instance.StatCollection.ModifyStat(
                        hitInfo.attackerId,
                        hitInfo.stackItemUID,
                        "DamageLevel",
                        StatCollection.StatOperation.Set,
                        damageLevel);
                }
                else
                {
                    // destory all other engine component parts
                    foreach (var component in mech.allComponents.Where(c => c != null && c.componentDef != null && c.componentDef.IsEnginePart()))
                    {
                        if (component.DamageLevel == ComponentDamageLevel.Destroyed)
                        {
                            continue;
                        }
                        component.DamageComponent(hitInfo, damageLevel, false);
                    }
                }

                return false;
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }

            return true;
        }
    }
}
using System.Collections.Generic;
using System.Linq;
using BattleTech;

namespace MechEngineMod
{
    internal static class EngineCrits
    {
        internal static bool ProcessWeaponHit(MechComponent mechComponent, WeaponHitInfo hitInfo, ComponentDamageLevel damageLevel, bool applyEffects)
        {
            if (!Control.settings.EngineCritsEnabled)
            {
                return true;
            }

            if (damageLevel != ComponentDamageLevel.Destroyed) // only care about destructions, other things dont get through anyway
            {
                return true;
            }
            
            if (mechComponent == null)
            {
                return true;
            }

            if (mechComponent.DamageLevel == ComponentDamageLevel.Destroyed) // already destroyed
            {
                return true;
            }

            if (!(mechComponent.parent is Mech))
            {
                return true;
            }

            var componentRef = mechComponent.mechComponentRef;

            if (componentRef == null || componentRef.Def == null)
            {
                return true;
            }

            if (!componentRef.Def.IsEnginePart())
            {
                return true;
            }

            var mech = (Mech)mechComponent.parent;
            var mainEngineComponent = mech.allComponents.FirstOrDefault(c => c != null && c.componentDef != null && c.componentDef.IsMainEngine());
            if (mainEngineComponent == null) // no main engine left
            {
                return true;
            }

            var location = (ChassisLocations)mechComponent.Location;

            var crits = 1;
            if (mech.IsLocationDestroyed(location))
            {
                damageLevel = ComponentDamageLevel.Destroyed;
                crits = mechComponent.componentDef.InventorySize;

                mechComponent.StatCollection.ModifyStat(
                    hitInfo.attackerId,
                    hitInfo.stackItemUID,
                    "DamageLevel",
                    StatCollection.StatOperation.Set,
                    damageLevel);
            }

            for (var i = 0; i < crits; i++)
            {
                switch (mainEngineComponent.StatCollection.GetStatistic("DamageLevel").Value<ComponentDamageLevel>())
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
                    default:
                        continue;
                }

                mainEngineComponent.StatCollection.ModifyStat(
                    hitInfo.attackerId,
                    hitInfo.stackItemUID,
                    "DamageLevel",
                    StatCollection.StatOperation.Set,
                    damageLevel);

                var heatSink = mech.StatCollection.GetStatistic("HeatSinkCapacity");
                mech.StatCollection.Int_Add(heatSink, Control.settings.HeatSinkCapacityAdjustmentPerCrit);
            }

            if (damageLevel >= ComponentDamageLevel.NonFunctional)
            {
                Control.mod.Logger.LogDebug(mainEngineComponent.Name + " " + damageLevel);

                foreach (var component in mech.allComponents.Where(c => c != null && c.componentDef != null && c.componentDef.IsEnginePart()))
                {
                    component.StatCollection.ModifyStat(
                        hitInfo.attackerId,
                        hitInfo.stackItemUID,
                        "DamageLevel",
                        StatCollection.StatOperation.Set,
                        damageLevel);
                }

                mech.FlagForDeath(
                    "Engine destroyed: " + mainEngineComponent.Description.Name,
                    DeathMethod.EngineDestroyed,
                    mainEngineComponent.Location,
                    hitInfo.stackItemUID,
                    hitInfo.attackerId,
                    false);
            }

            return false;
        }
    }
}
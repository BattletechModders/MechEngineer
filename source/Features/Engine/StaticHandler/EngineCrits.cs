using System.Collections.Generic;
using System.Linq;
using BattleTech;
using CustomComponents;

namespace MechEngineer
{
    internal static class EngineCrits
    {
        internal static bool ProcessWeaponHit(
            MechComponent mechComponent,
            CombatGameState combat,
            WeaponHitInfo hitInfo,
            ComponentDamageLevel damageLevel,
            bool applyEffects,
            List<MessageAddition> messages)
        {
            if (Control.settings.EngineCriticalHitStates == null)
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

            if (!(mechComponent.parent is Mech mech))
            {
                return true;
            }

            var componentRef = mechComponent.mechComponentRef;
            if (!componentRef.Def.IsEnginePart())
            {
                return true;
            }
            
            var mainEngineComponent = mech.allComponents.FirstOrDefault(c => c?.componentDef?.GetComponent<EngineCoreDef>() != null);
            if (mainEngineComponent == null) // no main engine left
            {
                return true;
            }

            var hits = 1;
            var location = (ChassisLocations) mechComponent.Location;
            var mechLocationDestroyed = mech.IsLocationDestroyed(location);
            if (mechLocationDestroyed)
            {
                hits = componentRef.Def.InventorySize;
            }

            if (CirticalHitStatesHandler.Shared.ProcessWeaponHit(
                mainEngineComponent,
                combat,
                hitInfo,
                damageLevel,
                applyEffects,
                messages,
                Control.settings.EngineCriticalHitStates,
                hits
                ))
            {
                return true;
            }

            damageLevel = mainEngineComponent.DamageLevel;

            if (mechLocationDestroyed)
            {
                mechComponent.StatCollection.ModifyStat(
                    hitInfo.attackerId,
                    hitInfo.stackItemUID,
                    "DamageLevel",
                    StatCollection.StatOperation.Set,
                    ComponentDamageLevel.Destroyed);
            }

            foreach (var component in mech.allComponents.Where(c => c.componentDef.IsEnginePart()))
            {
                if (component.DamageLevel >= damageLevel)
                {
                    continue;
                }

                component.StatCollection.ModifyStat(
                    hitInfo.attackerId,
                    hitInfo.stackItemUID,
                    "DamageLevel",
                    StatCollection.StatOperation.Set,
                    damageLevel);
            }

            return false;
        }
    }
}
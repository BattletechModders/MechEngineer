using System.Collections.Generic;
using System.Linq;
using BattleTech;

namespace MechEngineer
{
    internal static class EngineCrits
    {
        internal static bool ProcessWeaponHit(MechComponent mechComponent, WeaponHitInfo hitInfo, ComponentDamageLevel damageLevel, bool applyEffects, List<MessageAddition> messages)
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

            if (!(componentRef?.Def is IEnginePart))
            {
                return true;
            }

            var mech = (Mech) mechComponent.parent;
            var mainEngineComponent = mech.allComponents.FirstOrDefault(c => c?.componentDef is EngineCoreDef);
            if (mainEngineComponent == null) // no main engine left
            {
                return true;
            }

            var location = (ChassisLocations) mechComponent.Location;

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
                mech.StatCollection.Int_Add(heatSink, Control.settings.EngineHeatSinkCapacityAdjustmentPerCrit);
            }

            if (damageLevel >= ComponentDamageLevel.NonFunctional)
            {
                Control.mod.Logger.LogDebug(mainEngineComponent.Name + " " + damageLevel);

                foreach (var component in mech.allComponents.Where(c => c?.componentDef is IEnginePart))
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

                // FlagForDeath already outputs a message
                //messages.Add(new MessageAddition { Nature = FloatieMessage.MessageNature.ComponentDestroyed, Text = mainEngineComponent.UIName + " DESTROYED" });
            }
            else
            {
                var text = crits == 1 ? "ENGINE CRIT" : "ENGINE CRIT X" + crits;
                messages.Add(new MessageAddition {Nature = FloatieMessage.MessageNature.ComponentDestroyed, Text = text});
            }

            return false;
        }
    }
}
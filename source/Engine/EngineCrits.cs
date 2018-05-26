using System.Linq;
using BattleTech;

namespace MechEngineMod
{
    internal static class EngineCrits
    {
        internal static bool ProcessWeaponHit(MechComponent mechComponent, WeaponHitInfo hitInfo, ComponentDamageLevel damageLevel, bool applyEffects)
        {
            if (!applyEffects) // fake test through AI calculations or went through crit calculations already
            {
                return true;
            }

            if (damageLevel != ComponentDamageLevel.Destroyed) //we only care about destructions
            {
                return true;
            }

            if (mechComponent.componentDef == null || !(mechComponent.parent is Mech))
            {
                return true;
            }

            if (!mechComponent.componentDef.IsEnginePart())
            {
                return true;
            }

            var mech = (Mech)mechComponent.parent;
            if (mech.IsLocationDestroyed(ChassisLocations.CenterTorso)) // implies destroyed LT, CT, RT
            {
                return true;
            }

            switch (mechComponent.StatCollection.GetStatistic("DamageLevel").Value<ComponentDamageLevel>())
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

            if (mech.IsLocationDestroyed((ChassisLocations)mechComponent.Location))
            {
                damageLevel = ComponentDamageLevel.Destroyed;
            }

            // do on CRIT
            if (damageLevel < ComponentDamageLevel.NonFunctional)
            {
                Control.mod.Logger.LogDebug("CRIT on " + mechComponent.Name);

                var walkSpeed = mech.StatCollection.GetStatistic("WalkSpeed");
                var runSpeed = mech.StatCollection.GetStatistic("RunSpeed");
                mech.StatCollection.Float_Multiply(walkSpeed, Control.settings.SpeedMultiplierPerDamagedEnginePart);
                mech.StatCollection.Float_Multiply(runSpeed, Control.settings.SpeedMultiplierPerDamagedEnginePart);

                var heatSink = mech.StatCollection.GetStatistic("HeatSinkCapacity");
                mech.StatCollection.Int_Add(heatSink, Control.settings.HeatSinkCapacityPerDamagedEnginePart);

                mechComponent.StatCollection.ModifyStat(
                    hitInfo.attackerId,
                    hitInfo.stackItemUID,
                    "DamageLevel",
                    StatCollection.StatOperation.Set,
                    damageLevel);
            }
            else
            {
                // destory all other engine component parts
                foreach (var component in mech.allComponents.Where(c => c != null && c.componentDef != null && Extensions.IsEnginePart(c.componentDef)))
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
    }
}